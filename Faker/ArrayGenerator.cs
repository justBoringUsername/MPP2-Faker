using System;

namespace Faker
{
    class ArrayGenerator<T> : IGenericGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public object Generate()
        {
            var type = typeof(T);
            var arrLength = Random.Next(1, 30);
            var result = Array.CreateInstance(type, arrLength);
            var faker = new Faker();
            for (var i = 0; i < arrLength; i++)
            {
                var item = (T)faker.Create<T>();
                result.SetValue(item, i);
            }
            return result;
        }

        public Type GetGenerationType()
        {
            return typeof(Array);
        }
    }
}
