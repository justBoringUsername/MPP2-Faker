using System;

namespace Faker
{
    public interface IGenericGeneratorFactory
    {
        IGenerator GetGenerator(Type[] genericType);
    }
}