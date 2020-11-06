using System;

namespace Faker
{
    public class DoubleGenerator : IGenerator
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            double result;
            do
            {
                result = Random.NextDouble();
            } while (Math.Abs(Math.Abs(result - 0)) < 0.0000001);
            return Random.NextDouble();
        }

        public Type GetGenerationType()
        {
            return typeof(double);
        }
    }
}