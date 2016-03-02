using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    //Source : https://msdn.microsoft.com/en-us/library/bb763046%28v=vs.110%29.aspx
    //The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another AppDomain and refer to it from the default AppDomain.
    public class Sandboxer : MarshalByRefObject
    {
        public AppDomain newDomain;
        public Sandboxer newDomainInstance;
        Assembly untrustedAssembly;

        public void InitializeSandbox(string TempPath, string AssemblyPath)
        {
            lock (this)
            {
                //deinit if not null
                if (newDomain != null)
                    DeInitializeSandbox();

                //Setting the AppDomainSetup. 
                //It is very important to set the ApplicationBase to a folder other than the one in which the sandboxer resides.
                var adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = Path.GetFullPath(TempPath);

                //Setting the permissions for the AppDomain.
                //We give the permission to execute and to read/discover the location where the untrusted code is loaded.
                var permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

                //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
                //var fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

                //Now we have everything we need to create the AppDomain, so let's create it.
                //var newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
                newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet);

                //Use CreateInstanceFrom to load an instance of the Sandboxer class into the new AppDomain. 
                var handle = Activator.CreateInstanceFrom
                    (
                        newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                        typeof(Sandboxer).FullName
                    );

                //Unwrap the new domain instance into a reference in this domain and use it to execute the  untrusted code.
                newDomainInstance = (Sandboxer)handle.Unwrap();
                newDomainInstance.InitSandboxedInstance(File.ReadAllBytes(AssemblyPath));
            }
        }

        public void DeInitializeSandbox()
        {
            lock (this)
            {
                try
                {
                    if (newDomain != null)
                    {
                        newDomainInstance = null;
                        AppDomain.Unload(newDomain);
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Could not unload sandbox domain");
                }

                //release references
                newDomainInstance = null;
                newDomain = null;
            }
        }

        void InitSandboxedInstance(byte[] AssemblyData)
        {
            untrustedAssembly = Assembly.Load(AssemblyData);

        }

        [Serializable]
        public struct ExecutionResult
        {
            public bool IsValid;
            public object[] Result;
            public bool ThreadAborted;
            public string ExceptionMsg;
        }

        public ExecutionResult ExecuteUntrustedCode(string typeName, string entryPoint, int RunTime, ThreadPriority priority, Object[] parameters)
        {
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            var target = untrustedAssembly.GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.
                object[] result = null;
                string ExceptionMsg = "";
                var thread = new Thread(() =>
                {
                    try
                    {
                        result = target.Invoke(null, new[] { parameters }) as object[];
                    }
                    catch (Exception ex)
                    {
                        //navigate to most inner exception
                        while (ex.InnerException != null)
                            ex = ex.InnerException;
                        //keep msg
                        ExceptionMsg = ex.Message;
                    }
                });
                thread.Priority = priority;
                thread.Start();
                //join thread
                if (thread.Join(RunTime) == false)
                {
                    try { thread.Abort(); } catch { }
                    return new ExecutionResult() { IsValid = false, ThreadAborted = true, ExceptionMsg = ExceptionMsg };
                }
                return new ExecutionResult() { Result = result, IsValid = true, ExceptionMsg = ExceptionMsg };
            }
            catch (SecurityException ex)
            {
                DebugEx.TraceError("SecurityException caught:\n{0}", ex.ToString());
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now} ERROR {ex.Message}");
                return new ExecutionResult() { ExceptionMsg = ex.Message };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now} ERROR {ex.Message}");
                return new ExecutionResult() { ExceptionMsg = ex.Message };
            }
        }

        public ExecutionResult ExecuteUntrustedCode(string typeName, string entryPoint, Object[] parameters)
        {
            var target = untrustedAssembly.GetType(typeName).GetMethod(entryPoint);
            try
            {
                var result = target.Invoke(null, new[] { parameters }) as object[];
                return new ExecutionResult() { Result = result, IsValid = true };
            }
            catch (SecurityException ex)
            {
                DebugEx.TraceError("SecurityException caught:\n{0}", ex.ToString());
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now} ERROR {ex.Message}");
                return new ExecutionResult() { ExceptionMsg = ex.Message };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now} ERROR {ex.Message}");
                return new ExecutionResult() { ExceptionMsg = ex.Message };
            }
        }
    }
}