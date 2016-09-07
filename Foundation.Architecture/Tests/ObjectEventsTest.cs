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
            //define out message, a class
            var msg = new Msg { Content = MagicString };

            //define our routes (strings or game objects or something else)
            var route = new GameObject();
            var route2 = new GameObject();

            //subscribe using the object reference
            ObjectEvents<GameObject, Msg>.Subscribe(route, Handler);

            //many ways to send
            ObjectEvents<GameObject, Msg>.Publish(route, msg);
            ObjectEvents.Publish(route, msg);
            ObjectEvents.Publish(route, msg, typeof(GameObject), typeof(Msg));
            Assert.AreEqual(counter, 3);

            //bad route
            ObjectEvents.Publish(route2, msg, typeof(GameObject), typeof(Msg));

            //be sure to clean up as we are not using weak references
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
