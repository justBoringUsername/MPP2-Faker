using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Faker.Tests
{
    [TestClass]
    public class FakerTests
    {
        private readonly Faker _faker = new Faker();
        [TestMethod]
        public void ListTest()
        {
            var faker = new Faker();

            var list = faker.Create<List<int>>() as List<int>;

            Assert.IsNotNull(list);
            Assert.AreNotEqual(list, default);
        }

        private enum MyEnum
        {
            One,
            Two,
            Three
        }

        [TestMethod]
        public void InnerClass()
        {
            var myObject = _faker.Create<MyClass>() as MyClass;

            Assert.IsNotNull(myObject);
            Assert.IsNull(myObject.InnerClass.MyClass);
            Assert.AreNotEqual(myObject.PublicField, default);
            Assert.AreNotEqual(myObject.PublicProperty, default);
        }

        [TestMethod]
        public void ClassTest()
        {
            var myObject = _faker.Create<MyClass>() as MyClass;

            Assert.IsNotNull(myObject);
            Assert.AreNotEqual(myObject, default);
        }

        [TestMethod]
        public void DefaultInt()
        {
            var int32Value = _faker.Create<int>();
            Assert.AreNotEqual(int32Value, default);
        }

        [TestMethod]
        public void DefaultBool()
        {
            var boolValue = _faker.Create<bool>();
            Assert.AreNotEqual(boolValue, default);
        }

        [TestMethod]
        public void DefaultDouble()
        {
            var doubleValue = _faker.Create<double>();
            Assert.AreNotEqual(doubleValue, default);
        }

        [TestMethod]
        public void DefaultEnum()
        {
            var enumValue = _faker.Create<MyEnum>();
            Assert.AreNotEqual(enumValue, default);
        }

        [TestMethod]
        public void DefaultLong()
        {
            var longValue = _faker.Create<long>();
            Assert.AreNotEqual(longValue, default);
        }

        [TestMethod]
        public void DefaultString()
        {
            var stringValue = _faker.Create<string>();
            Assert.AreNotEqual(stringValue, default);
        }

        [TestMethod]
        public void Config()
        {
            var fakerConfig = new FakerConfig();
            fakerConfig.Add<Foo, string, FooStringGenerator>(Foo => Foo.Field);
            var faker = new Faker(fakerConfig);
            var foo = faker.Create<Foo>() as Foo;
            Assert.AreEqual(foo.Field, "TEST");
        }

        [TestMethod]
        public void ConfigInt()
        {
            var fakerConfig = new FakerConfig();
            fakerConfig.Add<Foo, int, FooIntGenerator>(Foo => Foo.FieldInt);
            var faker = new Faker(fakerConfig);
            var foo = faker.Create<Foo>() as Foo;
            Assert.AreEqual(foo.FieldInt, 123);
        }

        private class NoConstructorsCls
        {
            private NoConstructorsCls()
            {

            }
        }
            

        [TestMethod]
        public void NoConstructors()
        {
            var faker = new Faker();
            var result = faker.Create<NoConstructorsCls>();
        }

    }
}
