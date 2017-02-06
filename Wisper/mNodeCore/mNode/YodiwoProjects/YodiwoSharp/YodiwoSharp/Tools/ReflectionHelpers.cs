using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class ReflectionHelpers
    {
        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }
}
