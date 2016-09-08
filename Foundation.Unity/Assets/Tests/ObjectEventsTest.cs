using System;
using Foundation.Architecture;
using UnityEngine;

[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.Timeout(10)]
public class ObjectEventsTest : MonoBehaviour
{
    public class Msg
    {
        public string Content { get; set; }
    }

    public class SpaceShip
    {

    }

    public SpaceShip route = new SpaceShip();
    public SpaceShip route2 = new SpaceShip();

    const string MagicString = "Hello World";
    private int counter = 0;

    void Start()
    {
        TestMethod.RunAll(this, () =>
        {
            counter = 0;
            ObjectEvents<string, Msg>.Clear();
            ObjectEvents<GameObject, Msg>.Clear();
        });
    }

    [TestMethod]
    public void TestStringRoute()
    {
        counter = 0;
        var msg = new Msg {Content = MagicString};
        var route = "Users/NVenti";

        ObjectEvents<string, Msg>.Subscribe(route, Handler);
        ObjectEvents<string, Msg>.Publish(route, msg);
        ObjectEvents.Publish(route, msg);
        ObjectEvents.Publish(route, msg, typeof(string), typeof(Msg));
        Assert.AreEqual(counter, 3);

        //bad route
        ObjectEvents.Publish(route + route, msg, typeof(string), typeof(Msg));
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
        counter = 0;
        //define out message, a class
        var msg = new Msg {Content = MagicString};

        //subscribe using the object reference
        ObjectEvents<SpaceShip, Msg>.Subscribe(route, Handler);

        //many ways to send
        ObjectEvents<SpaceShip, Msg>.Publish(route, msg);
        ObjectEvents.Publish(route, msg);
        ObjectEvents.Publish(route, msg, typeof(SpaceShip), typeof(Msg));
        Assert.AreEqual(counter, 3);

        //bad route
        ObjectEvents.Publish(route2, msg, typeof(SpaceShip), typeof(Msg));

        //be sure to clean up as we are not using weak references
        ObjectEvents<SpaceShip, Msg>.Unsubscribe(route, Handler);

        ObjectEvents<SpaceShip, Msg>.Publish(route, msg);
        Assert.AreEqual(counter, 3);

        ObjectEvents<SpaceShip, Msg>.Subscribe(route, Handler);
        ObjectEvents<SpaceShip, Msg>.Unsubscribe(route);
        ObjectEvents<SpaceShip, Msg>.Publish(route, msg);

        Assert.AreEqual(counter, 3);
    }


    public void Handler(Msg myMessage)
    {
        Assert.AreEqual(myMessage.Content, MagicString);
        counter++;
    }
}