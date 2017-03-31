using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public interface IHasGetName
    {
        string Name { get; }
    }

    public interface IHasSetName
    {
        string Name { set; }
    }

    public interface IHasName : IHasGetName, IHasSetName
    {
    }
}
