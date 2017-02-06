using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class TypeCache
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public static readonly HashSetTS<Assembly> EntryAssemblies = new HashSetTS<Assembly>();
        //------------------------------------------------------------------------------------------------------------------------
        static Dictionary<string, Type> cache = new Dictionary<string, Type>()
        {
            { "", null }
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        static TypeCache()
        {
#if NETFX
            //add entry assembly
            var EntryAssembly = Assembly.GetEntryAssembly();
            if (EntryAssembly != null)
                EntryAssemblies.Add(EntryAssembly);

            //Add as many assemblies as we can
            //add from stack
            try
            {
                var frameAssemblies = new System.Diagnostics.StackTrace().GetFrames().Select(t => t.GetMethod().Module.Assembly).ToHashSet();
                foreach (var entry in frameAssemblies)
                    EntryAssemblies.Add(entry);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Unhandled exception during Stack Frame assembly examination"); }

            //add from domain
            try
            {
                EntryAssemblies.AddFromSource(AppDomain.CurrentDomain.GetAssemblies());
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Unhandled exception during AppDomain assembly examination"); }
#elif UNIVERSAL
            EntryAssemblies.Add(Windows.UI.Xaml.Application.Current.GetType().GetTypeInfo().Assembly);
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public static void AddEntryAssembly(Type fromType)
        {
#if NETFX
            EntryAssemblies.Add(fromType.Assembly);
#elif UNIVERSAL
            EntryAssemblies.Add(fromType.GetTypeInfo().Assembly);
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static Type GetType(string name, bool DeFriendlify = true)
        {
            //sanity check
            if (string.IsNullOrWhiteSpace(name))
                return null;

            //find type
            Type result = null;
            lock (cache)
                if (cache.TryGetValue(name, out result) == false)
                {
                    try
                    {
                        //resolve type
                        result = Type.GetType(name);

                        //If null go for fallback mechanisms
                        if (result == null)
                        {
                            //This is now in fallback. do not keep lock since what follows are expensive operations
                            Monitor.Exit(cache);
                            try
                            {
                                //if not found and not fully qualified search in all assemblies
                                if (result == null)
                                {
                                    string strippedName;
                                    //process name (strip assembly and recersivly extract generic types)
                                    if (name.Contains('['))
                                    {
                                        var lefovers = name.RightOf("[");
                                        var generics = new List<string>();
                                        int i = 0;
                                        int start = 0;
                                        for (int n = 0; n < lefovers.Length; n++)
                                        {
                                            var c = lefovers[n];
                                            if (c == '[')
                                            {
                                                if (i == 0)
                                                    start = n + 1;
                                                i++;
                                            }
                                            else if (c == ']')
                                            {
                                                i--;
                                                if (i == 0)
                                                {
                                                    generics.Add(lefovers.Substring(start, n - start));
                                                    start = n + 1;
                                                }
                                            }
                                        }
                                        //get types for each generic
                                        var genericsTypes = new List<Type>();
                                        foreach (var entry in generics)
                                        {
                                            var gt = GetType(entry);
                                            if (gt == null)
                                                return null;
                                            genericsTypes.Add(gt);
                                        }
                                        //process found generics recursively
                                        strippedName = name.LeftOf("`") + "`" + generics.Count + "[" + string.Join(", ", genericsTypes.Select(t => "[" + t.AssemblyQualifiedName + "]")) + "]";
                                        //try a fast re-check of processed name
                                        result = Type.GetType(strippedName);
                                    }
                                    else
                                        strippedName = name.LeftOf(",");

                                    //search assemblies
                                    if (result == null)
                                        foreach (var entry in GetAssemblies())
                                        {
                                            result = entry.GetType(strippedName);
                                            if (result != null)
                                                break;
                                        }
                                }

                                //Try to find friendly named types
                                if (result == null && DeFriendlify)
                                {
                                    try { result = Type.GetType(Extensions.DeFriendlifyName(name)); }
                                    catch (Exception ex) { DebugEx.Assert(ex, "DeFriendlifyName failed"); }
                                }
                            }
                            catch (Exception ex) { DebugEx.Assert(ex, "Caught unhandled exception during type resolve"); }
                            finally { Monitor.Enter(cache); } //re-aquire lock before continuing
                        }
                    }
                    catch (Exception ex) { DebugEx.Assert(ex, "Unhandled excpetion while trying to resolve type"); }

                    //cache it
                    cache.ForceAdd(name, result);
                }

            //done and done
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------
        static int entriesCount = -1;
        static Assembly[] _assemblyCache = new Assembly[0];
        public static IEnumerable<Assembly> GetAssemblies()
        {
#if NETFX
            lock (_assemblyCache)
            {
                if (_assemblyCache.Length == 0 || entriesCount != EntryAssemblies.Count)
                {
                    //mark for cache invalidation
                    entriesCount = EntryAssemblies.Count;

                    //declares
                    var visitedAssemblies = new HashSet<Assembly>();
                    var visitedAssemblyNames = new HashSet<string>();
                    var stack = new Stack<Assembly>();

                    //initialize
                    DebugEx.Assert(EntryAssemblies.Count > 0, "EntryAssemblies are empty");
                    foreach (var entry in EntryAssemblies)
                    {
                        stack.Push(entry);
                        visitedAssemblies.Add(entry);
                        try { visitedAssemblyNames.Add(entry.FullName); } catch { }
                    }

                    //consume stack
                    do
                    {
                        var asm = stack.Pop();
                        foreach (var reference in asm.GetReferencedAssemblies())
                            try
                            {
                                if (!visitedAssemblyNames.Contains(reference.FullName))
                                {
                                    var assembly = Assembly.Load(reference);
                                    if (assembly != null)
                                    {
                                        stack.Push(assembly);
                                        visitedAssemblies.Add(assembly);
                                        try { visitedAssemblyNames.Add(reference.FullName); } catch { }
                                    }
                                }
                            }
                            catch { }
                    }
                    while (stack.Count > 0);

                    //cache them
                    _assemblyCache = visitedAssemblies.ToArray();
                }

                //return cached assemblies
                return _assemblyCache;
            }
#elif UNIVERSAL
            return EntryAssemblies;
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<Type> GetAllTypes()
        {
            foreach (var asm in TypeCache.GetAssemblies())
            {
                Type[] types = null;
                try { types = asm.GetTypes(); } catch { }
                if (types != null)
                    foreach (var t in types)
                        if (!t.Name.StartsWith("__"))
                            yield return t;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string StripAssemblies(string name, bool recersive = true)
        {
            //check
            if (string.IsNullOrWhiteSpace(name))
                return name;

            //is generic?
            if (name.Contains('['))
            {
                var lefovers = name.RightOf("[");
                var generics = new List<string>();
                int i = 0;
                int start = 0;
                for (int n = 0; n < lefovers.Length; n++)
                {
                    var c = lefovers[n];
                    if (c == '[')
                    {
                        if (i == 0)
                            start = n + 1;
                        i++;
                    }
                    else if (c == ']')
                    {
                        i--;
                        if (i == 0)
                        {
                            generics.Add(lefovers.Substring(start, n - start));
                            start = n + 1;
                        }
                    }
                }
                //process found generics recursively
                name = name.LeftOf("`") + "`" + generics.Count + "[" + string.Join(", ", generics.Select(t => "[" + StripAssemblies(t))) + "]" + "]";
            }
            else
                name = name.LeftOf(",");
            //return final string
            return name;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}