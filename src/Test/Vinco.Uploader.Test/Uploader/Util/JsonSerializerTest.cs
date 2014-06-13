using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vinco.Uploader.Util
{
    [TestFixture]
    public class JsonSerializerTest
    {
        [Test]
        public void SerializeToJsonString_Throws_Test()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.SerializeToJsonString(null));
        }

        [Test]
        public void Deserialize_Throws_Test()
        {
            Assert.Throws<ArgumentException>(() => JsonSerializer.Deserialize<Item>(""));
        }

        [Test]
        public void Serialize_Test()
        {
            var item = new Item {Name = "Serialize"};

            var result = JsonSerializer.SerializeToJsonString(item);

            Assert.AreEqual(@"{""Name"":""Serialize""}", result);
        }

        [Test]
        public void Deserialize_Test()
        {
            var result = JsonSerializer.Deserialize<Item>(@"{""Name"":""Deserialize""}");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Item>(result);
            Assert.AreEqual("Deserialize", result.Name);
        }

        public class Item
        {
            public string Name { get; set; }
        }
    }
}
