using System;

namespace Faker
{
    public class EnumGenerator<T> : IGenericGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            Type type = typeof(T);
            var values = Enum.GetValues(type);
            return values.GetValue(Random.Next(1, values.Length));
        }

        public Type GetGenerationType()
        {
            return typeof(Enum);
        }
    }
}
