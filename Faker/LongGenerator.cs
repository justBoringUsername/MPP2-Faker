using System;

namespace Faker
{
    public class LongGenerator : IGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            var buffer = new byte[8];
            long result;
            do
            {
                Random.NextBytes(buffer);
                result = BitConverter.ToInt64(buffer, 0);
            } while (result == 0);

            return result;
        }

        public Type GetGenerationType()
        {
            return typeof(long);
        }
    }
}