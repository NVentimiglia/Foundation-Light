using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class BufferTests
    {
       
        [TestMethod]
        public void TestInt()
        {
            var buffer = new byte[sizeof(int)];

            BufferHelper.Write(buffer,0, 69);

            var val = BufferHelper.ReadInt32(buffer, 0);

            Assert.AreEqual(val, 69);
        }

        [TestMethod]
        public void TestShort()
        {
            var buffer = new byte[sizeof(ushort)];

            BufferHelper.Write(buffer, 0, (ushort)69);

            var val = BufferHelper.ReadUInt16(buffer, 0);

            Assert.AreEqual(val, 69);
        }

        [TestMethod]
        public void TestFloat()
        {
            var buffer = new byte[sizeof(float)];

            BufferHelper.Write(buffer, 0, 69f);

            var val = BufferHelper.ReadFloat(buffer, 0);

            Assert.AreEqual(val, 69f);
        }


        [TestMethod]
        public void TestString()
        {
            var str = "Hello World BLAH BLAH";

            var buffer = new byte[(sizeof(char) * str.Length) + sizeof(ushort)];

            BufferHelper.Write(buffer, 0, str);

            var val = BufferHelper.ReadString(buffer, 0);

            Assert.AreEqual(val, str);
        }
    }
}
