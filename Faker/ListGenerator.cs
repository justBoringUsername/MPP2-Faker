using System;
using System.Collections.Generic;

namespace Faker
{
    public class ListGenerator<T> : IGenericGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            var list = new List<T>();
            var size = Random.Next(1, 30);
            var faker = new Faker();
            for (var i = 0; i < size; i++)
            {
                var item = (T)faker.Create<T>();
                list.Add(item);
            }

            return list;
        }

        public Type GetGenerationType()
        {
            return typeof(List<T>);
        }
    }
}