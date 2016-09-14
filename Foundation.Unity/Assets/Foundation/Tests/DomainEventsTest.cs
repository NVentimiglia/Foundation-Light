using Foundation.Architecture;
using UnityEngine;

[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.Timeout(10)]
public class DomainEventsTest : MonoBehaviour
{
    public class Msg
    {
        public string Content { get; set; }
    }

    const string MagicString = "Hello World";
    private int counter = 0;

    void Start()
    {
        TestMethod.RunAll(this, () => { counter = 0; });
    }

    [TestMethod]
    public void TestSubscribe()
    {
        //Define a message class to send
        var msg = new Msg { Content = MagicString };

        //subscribe you handlers
        DomainEvents<Msg>.Subscribe(Handler);

        //maby ways to send
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