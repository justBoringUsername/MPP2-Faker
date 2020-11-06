using System;
using Faker;

namespace Generators
{
    public class DateTimeGenerator : IGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public object Generate()
        {
            var buf = new byte[8];
            Random.NextBytes(buf);
            var ticks = BitConverter.ToInt64(buf, 0);
            return new DateTime(Math.Abs(ticks % (DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks)) + DateTime.MinValue.Ticks);
        }

        public Type GetGenerationType()
        {
            return typeof(DateTime);
        }
    }
}
