using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Yodiwo
{
    public class YEventRouter
    {
        /// <summary> Global Event Router </summary>
        public static Yodiwo.YEventRouter EventRouter = new YEventRouter();
        //--------------------------------------------------------------------------------------------------------------------------------------
        #region Variables
        //--------------------------------------------------------------------------------------------------------------------------------------
        public class EvInfo
        {
            public object priv;
            public bool IsDerivedMatch;
        }
        public delegate void EventCb<T>(object sender, EvInfo info, T ev);
        //--------------------------------------------------------------------------------------------------------------------------------------
        class EvHandler
        {
            public object Cb;
            public int Priority;
            public bool ReceiveDerivedTypeEvents;
        }
        private DictionaryTS<Type, SortedSetTS<EvHandler>> _ActiveRoutes = new DictionaryTS<Type, SortedSetTS<EvHandler>>();
        //--------------------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //--------------------------------------------------------------------------------------------------------------------------------------
        public YEventRouter()
        {
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //--------------------------------------------------------------------------------------------------------------------------------------        
        public bool AddEventHandler<T>(EventCb<T> cb, int Priority, bool ReceiveDerivedTypeEvents = true)
        {
            //null check
            if (cb == null)
            {
                DebugEx.Assert("Cannot give Null callback for YEventRouter");
                return false;
            }

            //Get or create route
            var evType = typeof(T);
            SortedSetTS<EvHandler> allRoutesForType;
            lock (_ActiveRoutes)
            {
                allRoutesForType = _ActiveRoutes.TryGetOrDefault(evType);
                if (allRoutesForType == null)
                    allRoutesForType = _ActiveRoutes[evType] = new SortedSetTS<EvHandler>(s_EvHandlerCmp);

                //add to route
                return allRoutesForType.Add(new EvHandler() { Cb = cb, Priority = Priority, ReceiveDerivedTypeEvents = ReceiveDerivedTypeEvents });
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------        
        public bool RemoveEventHandler<T>(EventCb<T> cb)
        {
            //null check
            if (cb == null)
                return false;

            //Get route
            var evType = typeof(T);
            lock (_ActiveRoutes)
            {
                var allRoutesForType = _ActiveRoutes.TryGetOrDefault(evType);

                //check if handlers are registered for this event type
                if (allRoutesForType == null)
                {
                    DebugEx.Assert("no such event found registered");
                    return false;
                }

                //Remove from route
                var removed = allRoutesForType.RemoveWhere(h => Object.Equals(h.Cb, cb)) > 0;

                //check if this event type has any other handlers, otherwise remove the type as well
                if (allRoutesForType.Count == 0)
                    _ActiveRoutes.Remove(evType);

                //return if something has been removed or not
                return removed;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------        
        private void _ExecuteHandler<T>(object sender, T ev)
        {
            //declares
            SortedSetTS<EvHandler> allRoutesForType = null;
            var evType = typeof(T);

            //null check
            if (ev == null)
                return;

            //create event info
            var evInfo = new EvInfo()
            {
                priv = null,
                IsDerivedMatch = false,
            };

            //Execute
            var executedCallbacks = new HashSet<object>();
            while (evType != null && evType != typeof(object))
            {
                //get routes for type
                allRoutesForType = _ActiveRoutes.TryGetOrDefault(evType);
                if (allRoutesForType != null)
                {
                    //Broadcast event
                    foreach (var evhandle in allRoutesForType)
                        if (!evInfo.IsDerivedMatch || evhandle.ReceiveDerivedTypeEvents)
                        {
                            try
                            {
                                //check that we haven't already run this callback
                                if (!executedCallbacks.Contains(evhandle.Cb))
                                {
                                    //use Dynamic callback if derived type; use known type otherwise
                                    if (evInfo.IsDerivedMatch)
                                        ((dynamic)evhandle.Cb)(sender, evInfo, ev);
                                    else
                                        ((EventCb<T>)evhandle.Cb)(sender, evInfo, ev);

                                    //add to hashset so that it's not executed again
                                    executedCallbacks.Add(evhandle.Cb);
                                }
                            }
                            catch (Exception ex)
                            {
                                DebugEx.TraceErrorException(ex);
                            }
                        }
                }

                //now moving up the inheritance tree (toward object)
#if NETFX
                evType = evType.BaseType;
#else
                evType = evType.GetTypeInfo().BaseType;
#endif
                evInfo.IsDerivedMatch = true;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------        
        public Task TriggerEventAsync<T>(object sender, T ev)
        {
            return Task.Run(() => _ExecuteHandler(sender, ev));
        }
        //--------------------------------------------------------------------------------------------------------------------------------------        
        public void TriggerEvent<T>(object sender, T ev)
        {
            _ExecuteHandler(sender, ev);
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region EvHandlerCmp
        static EvHandlerCmp s_EvHandlerCmp = new EvHandlerCmp();
        class EvHandlerCmp : IComparer<EvHandler>
        {
            public int Compare(EvHandler x, EvHandler y)
            {
                return y.Priority - x.Priority;
            }
        }
        #endregion
    }

    #region unit testing
    static class Test
    {
        public class Payload1
        {
            public string a;
            public DateTime ts = DateTime.Now;
            public string c;

            public override string ToString()
            {
                return "a: " + a + " b: " + ts.ToString() + " c: " + c;
            }
        }
        public class Payload2
        {
            public DateTime ts = DateTime.Now;
            public string b;

            public override string ToString()
            {
                return "a: " + ts.ToString() + " b: " + b;
            }
        }
        public class Payload2_2 : Payload2
        {
            public double c;
        }
        public class Payload2_3 : Payload2_2
        {
            public double d;
        }

        public static void Handler1b(object sender, YEventRouter.EvInfo info, object ev)
        {
            DebugEx.TraceLog("HANDLER1b " + ev + " called");
        }

        public static void StartTest()
        {
            var evr = new YEventRouter();
            const int iterations = 1000000;
            //TimeSpan total_derived = TimeSpan.Zero;
            //TimeSpan total_direct = TimeSpan.Zero;

            Stopwatch watch = new Stopwatch();
            int g = 0;

            //add 2 event handlers
            DebugEx.TraceLog("Adding handler1a/b and handler2");
            evr.AddEventHandler<Payload1>((s, i, e) =>
                {
                    g++;
                }, 5);
            //evr.AddEventHandler<Payload1>(Handler1b);
            evr.AddEventHandler<Payload2>((s, i, e) =>
            {
                g++;
            }, 6);

            //evr.AddEventHandler<Payload2>((s, i, e) =>
            //    {
            //        DebugEx.TraceLog("HANDLER2 " + e + " called");

            //        //create new handler for this Type route (altering the internal hashset)
            //        evr.AddEventHandler<Payload2>((x, y, z) => DebugEx.TraceLog("HANDLER2 created from within handler"));

            //        //create new handler for a new Type route
            //        evr.AddEventHandler<Payload2_3>((x, y, z) => DebugEx.TraceLog("HANDLER2_3 created from within handler"));
            //    });
            //evr.AddEventHandler<Payload2_2>((s, i, e) => DebugEx.TraceLog("DERIVED HANDLER2_2 " + e + " called"));

            //scale cpu clock up
            for (int i = 0; i < iterations; i++)
                evr.TriggerEvent<Payload2_2>(null, new Payload2_2() { b = "Payload2_2" });

            //trigger events
            DebugEx.TraceLog("Trigger events");
            watch.Start();
            for (int i = 0; i < iterations; i++)
                evr.TriggerEvent<Payload2_2>(null, new Payload2_2() { b = "Payload2_2" });
            watch.Stop();
            var total_derived = watch.ElapsedMilliseconds;
            DebugEx.TraceLog("Total time for " + g + " derived iterations = " + total_derived);

            g = 0;
            watch.Restart();
            for (int i = 0; i < iterations; i++)
                evr.TriggerEvent<Payload2>(null, new Payload2() { b = "Payload1" });

            watch.Stop();
            var total_direct = watch.ElapsedMilliseconds;
            DebugEx.TraceLog("Total time for " + g + " direct iterations = " + total_direct);


            return;
            evr.TriggerEvent<Payload2>(null, new Payload2() { b = "Payload2" });
            evr.TriggerEvent<Payload1>(null, new Payload1() { a = "Payload1", c = "newtest" });
            Task.Delay(3000).Wait();
            evr.TriggerEvent<Payload1>(null, new Payload1() { a = "Payload1", c = "newtest" });
            evr.TriggerEvent<Payload2>(null, new Payload2() { b = "Payload2" });

            //remove handler1b
            DebugEx.TraceLog("removing handler1b");
            evr.RemoveEventHandler<Payload1>(Handler1b);

            //trigger same events
            DebugEx.TraceLog("Trigger events");
            evr.TriggerEvent<Payload1>(null, new Payload1() { a = "Payload1", c = "newtest" });
            evr.TriggerEvent<Payload2>(null, new Payload2() { b = "Payload2" });
            Task.Delay(3000).Wait();
            evr.TriggerEvent<Payload1>(null, new Payload1() { a = "Payload1", c = "newtest" });
            Task.Delay(3000).Wait();
            evr.TriggerEvent<Payload1>(null, new Payload1() { a = "Payload1", c = "newtest" });
            evr.TriggerEvent<Payload2>(null, new Payload2() { b = "Payload2" });

            DebugEx.TraceLog("END");
        }
    }
    #endregion
}
