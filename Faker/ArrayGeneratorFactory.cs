using System;

namespace Faker
{
    class ArrayGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenerator GetGenerator(Type[] genericType)
        {
            var arrayGenericGeneratorType = typeof(ArrayGenerator<>);
            var arrayGeneratorType = arrayGenericGeneratorType.MakeGenericType(genericType[0]);
            return (IGenerator)Activator.CreateInstance(arrayGeneratorType);
        }
    }
}
