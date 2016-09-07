using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class ObjectEventsTest
    {
        public class Msg
        {
            public string Content { get; set; }
        }

        public class GameObject
        {

        }

        const string MagicString = "Hello World";
        private int counter = 0;

        [TestCleanup]
        public void Cleanup()
        {
            counter = 0;
        }

        [TestMethod]
        public void TestStringRoute()
        {
            var msg = new Msg {Content = MagicString};
            var route = "Users/NVenti";

            ObjectEvents<string, Msg>.Subscribe(route, Handler);
            ObjectEvents<string, Msg>.Publish(route, msg);
            ObjectEvents.Publish(route, msg);
            ObjectEvents.Publish(route, msg, typeof(string), typeof(Msg));
            Assert.AreEqual(counter, 3);

            //bad route
            ObjectEvents.Publish(route +route, msg, typeof(string), typeof(Msg));
            ObjectEvents<string, Msg>.Unsubscribe(route, Handler);
            ObjectEvents<string, Msg>.Publish(route, msg);

            Assert.AreEqual(counter, 3);

            ObjectEvents<string, Msg>.Subscribe(route, Handler);
            ObjectEvents<string, Msg>.Unsubscribe(route);
            ObjectEvents<string, Msg>.Publish(route, msg);

            Assert.AreEqual(counter, 3);
        }


        [TestMethod]
        public void TestObjectRoute()
        {
            var msg = new Msg { Content = MagicString };
            var route = new GameObject();
            var route2 = new GameObject();

            ObjectEvents<GameObject, Msg>.Subscribe(route, Handler);
            ObjectEvents<GameObject, Msg>.Publish(route, msg);
            ObjectEvents.Publish(route, msg);
            ObjectEvents.Publish(route, msg, typeof(GameObject), typeof(Msg));
            Assert.AreEqual(counter, 3);

            //bad route
            ObjectEvents.Publish(route2, msg, typeof(GameObject), typeof(Msg));
            ObjectEvents<GameObject, Msg>.Unsubscribe(route, Handler);
            ObjectEvents<GameObject, Msg>.Publish(route, msg);

            Assert.AreEqual(counter, 3);

            ObjectEvents<GameObject, Msg>.Subscribe(route, Handler);
            ObjectEvents<GameObject, Msg>.Unsubscribe(route);
            ObjectEvents<GameObject, Msg>.Publish(route, msg);

            Assert.AreEqual(counter, 3);
        }


        public void Handler(Msg myMessage)
        {
            Assert.AreEqual(myMessage.Content, MagicString);
            counter++;
        }
    }
}
