using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class DomainEventsTest
    {
        public class Msg
        {
            public string Content { get; set; }
        }

        const string MagicString = "Hello World";
        private int counter = 0;

        [TestCleanup]
        public void Cleanup()
        {
            counter = 0;
        }

        [TestMethod]
        public void TestSubscribe()
        {
            var msg = new Msg {Content = MagicString};
            
            DomainEvents<Msg>.Subscribe(Handler);
            DomainEvents<Msg>.Publish(msg);
            DomainEvents.Publish(msg);
            DomainEvents.Publish(msg, typeof(Msg));
            Assert.AreEqual(counter, 3);

            DomainEvents<Msg>.Unsubscribe(Handler);
            DomainEvents.Publish(msg);
            
            Assert.AreEqual(counter, 3);
        }

        [TestMethod]
        public void TestAddEvent()
        {
            var msg = new Msg { Content = MagicString };
            
            DomainEvents<Msg>.OnMessage += Handler;

            DomainEvents<Msg>.Publish(msg);
            DomainEvents.Publish(msg);
            DomainEvents.Publish(msg, typeof(Msg));

            Assert.AreEqual(counter, 3);
            
            DomainEvents<Msg>.Unsubscribe(Handler);
            DomainEvents.Publish(msg);

            Assert.AreEqual(counter, 3);
        }
        

        public void Handler(Msg myMessage)
        {
            Assert.AreEqual(myMessage.Content, MagicString);
            counter++;
        }
    }
}
