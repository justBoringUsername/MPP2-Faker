using System;

namespace Faker
{
    public class Int32Generator : IGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            return Random.Next(1, int.MaxValue);
        }

        public Type GetGenerationType()
        {
            return typeof(int);
        }
    }
}