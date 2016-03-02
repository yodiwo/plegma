using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public abstract class WeakAction
    {
    }

    public abstract class WeakAction<T> : WeakAction
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected readonly MethodInfo _Method;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        protected WeakAction(MethodInfo method)
        {
            this._Method = method;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(object instance, MethodInfo _method)
        {
            return new WeakActionInstance(instance, _method);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(MethodInfo _method)
        {
            return new WeakActionStatic(_method);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(Action action)
        {
#if NETFX
            return new WeakActionInstance(action.Target, action.Method);
#else
            return new WeakActionInstance(action.Target, action.GetMethodInfo());
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(Action<T> action)
        {
#if NETFX
            return new WeakActionInstance(action.Target, action.Method);
#else
            return new WeakActionInstance(action.Target, action.GetMethodInfo());
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(LambdaExpression expression)
        {
            return new WeakActionStatic(GetMethodInfo(expression));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WeakAction<T> Create(object instance, LambdaExpression expression)
        {
            return new WeakActionInstance(instance, GetMethodInfo(expression));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public abstract void Invoke(T arg);
        //------------------------------------------------------------------------------------------------------------------------
        public abstract bool IsAlive { get; }
        //------------------------------------------------------------------------------------------------------------------------
        private static MethodInfo GetMethodInfo(Expression expression)
        {
            //extract method from Lamba expression
            //get lamda
            var lambda = expression as LambdaExpression;
            if (lambda == null)
                throw new ArgumentException("expression is not LambdaExpression");

            //find outer expression
            var outermostExpression = lambda.Body as MethodCallExpression;
            if (outermostExpression == null)
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");

            //get method
            return outermostExpression.Method;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Helper Classes
        //========================================================================================================================================

        private class WeakActionStatic : WeakAction<T>
        {
            private readonly Type _Owner;
            Action<T> del;

            public WeakActionStatic(MethodInfo method)
                : base(method)
            {
                if (!method.IsStatic)
                {
                    throw new ArgumentException("static method expected", "method");
                }
                _Owner = method.DeclaringType;

                //build delegate
#if NETFX
                del = System.Delegate.CreateDelegate(typeof(Action<T>), null, _Method.Name) as Action<T>;
#else
                del = _Method.CreateDelegate((typeof(Action<T>)), null) as Action<T>;
#endif
            }


            public override bool IsAlive
            {
                get { return true; }
            }

            public override void Invoke(T arg)
            {
                del(arg);
            }
        }

        //========================================================================================================================================

        private class WeakActionInstance : WeakAction<T>
        {
            private readonly WeakReference _WeakRef;

            protected WeakActionInstance(MethodInfo method)
                : base(method)
            {
            }

            public WeakActionInstance(object instance, MethodInfo method)
                : this(method)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance must not be null", "instance");
                }
                _WeakRef = new WeakReference(instance);
            }

            public override bool IsAlive
            {
                get { return _WeakRef.IsAlive; }
            }

            public override void Invoke(T arg)
            {
                if (!IsAlive)
                    return;

                //get target
                object localTarget = _WeakRef.Target;
                if (localTarget == null)
                    return;

                //build delegate
#if NETFX
                var del = System.Delegate.CreateDelegate(typeof(Action<T>), localTarget, _Method.Name) as Action<T>;
#else
                var del = _Method.CreateDelegate((typeof(Action<T>)), localTarget) as Action<T>;
#endif
                //invoke it
                del(arg);
            }
        }

        //========================================================================================================================================
        #endregion
    }
}
