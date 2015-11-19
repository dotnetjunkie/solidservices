using System;
using System.Linq.Expressions;
using System.Reflection;
using Ploeh.AutoFixture;

namespace WebApiService.Code
{
    public static class ExampleObjectCreator
    {
        private static readonly MethodInfo CreateMethod =
            GetMethod(() => SpecimenFactory.Create<int>(new Fixture()))
            .GetGenericMethodDefinition();

        public static object Create(Type type)
        {
            var fixture = new Fixture();
            int index = 1;
            fixture.Register(() => "sample text " + index++);

            return CreateMethod.MakeGenericMethod(type).Invoke(null, new object[] { fixture });
        }

        private static MethodInfo GetMethod(Expression<Func<int>> methodCall) => 
            ((MethodCallExpression)methodCall.Body).Method;
    }
}