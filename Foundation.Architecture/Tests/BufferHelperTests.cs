using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class BufferHelperTests
    {

        [TestMethod]
        public void TestInt()
        {
            var buffer = new byte[sizeof(int)];

            BufferHelper.Write(buffer, 0, 69);

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


        [TestMethod]
        public void TestBadRead()
        {
            try
            {
                var buffer = new byte[sizeof(float)];

                BufferHelper.Write(buffer, 0, 69f);

                var val = BufferHelper.ReadFloat(buffer, 0);
                val = BufferHelper.ReadFloat(buffer, sizeof(float));

                Assert.Fail("Should of Failed");

            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
            }
        }


        [TestMethod]
        public void TestBadWrite()
        {
            try
            {
                var buffer = new byte[sizeof(float)];

                BufferHelper.Write(buffer, 0, 69f);
                BufferHelper.Write(buffer, sizeof(float), 69f);

                Assert.Fail("Should of Failed");

            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
            }
        }
    }
}
