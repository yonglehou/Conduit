using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Conduit
{
    public delegate T ObjectActivatorHandler<T>(params object[] args);

    public class ObjectActivator
    {
        public static ObjectActivatorHandler<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivatorHandler<T>), newExp, param);

            //compile it
            ObjectActivatorHandler<T> compiled = (ObjectActivatorHandler<T>)lambda.Compile();
            return compiled;
        }

        public static T New<T>()
        {
            System.Reflection.ConstructorInfo ctor = typeof(T).GetConstructors().First();
            ObjectActivatorHandler<T> activator = ObjectActivator.GetActivator<T>(ctor);

            T instance = activator();
            return instance;
        }
    }
}
