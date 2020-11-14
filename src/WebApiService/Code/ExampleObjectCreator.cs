namespace WebApiService.Code
{
    using System;
    using AutoFixture;
    using AutoFixture.Kernel;

    public static class ExampleObjectCreator
    {
        public static object Create(Type type)
        {
            var fixture = new Fixture();

            int index = 1;
            fixture.Register(() => "sample text " + index++);

            return new SpecimenContext(fixture).Resolve(type);
        }
    }
}