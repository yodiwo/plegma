using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public abstract class MarshalByRefObject
    {
        public virtual object InitializeLifetimeService() { return null; }
    }
}
