using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public interface IReadOnlySet<T> : IEnumerable<T>
    {
        bool Contains(T item);
    }
}
