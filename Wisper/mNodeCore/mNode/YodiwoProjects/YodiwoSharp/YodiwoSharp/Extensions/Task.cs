using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetResults<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetResults<T>(this TaskCompletionSource<T> tc)
        {
            return tc.Task.GetResults();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
