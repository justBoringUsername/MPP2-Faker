using System;

namespace Faker
{
    class EnumGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenerator GetGenerator(Type[] genericType)
        {
            var enumGenericGeneratorType = typeof(EnumGenerator<>);
            var enumGeneratorType = enumGenericGeneratorType.MakeGenericType(genericType[0]);
            return (IGenerator)Activator.CreateInstance(enumGeneratorType);
        }
    }
}
