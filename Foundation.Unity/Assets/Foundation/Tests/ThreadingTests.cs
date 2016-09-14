using System.Collections;
using Foundation.Architecture;
using UnityEngine;

[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.Timeout(10)]
public class ThreadingTests : MonoBehaviour
{
    private double updateCounter;

    public void Start()
    {
        TestMethod.RunAll(this, () => updateCounter = 0);
    }


    [TestMethod]
    public IEnumerator TestUpdate()
    {
        var task = ThreadingService.RunUpdate(MyUpdate);
        yield return 1;

        yield return new WaitForSeconds(1.2f);

        //Asset works
        Assert.IsTrue(updateCounter >= 1);

        task.Dispose();

        var old = updateCounter;

        yield return new WaitForSeconds(1);

        //Asset Dispose works
        Assert.IsTrue(updateCounter == old);
    }
    void MyUpdate(double delta)
    {
        updateCounter += delta;
    }

    [TestMethod]
    public IEnumerator TestDelay()
    {
        var task = ThreadingService.RunDelay(UpdateDelay, 1);
        yield return new WaitForSeconds(.5f);
        //Asset Not early executed
        Assert.IsTrue(updateCounter == 0);
        yield return new WaitForSeconds(2f);
        //Asset Executed in time
        Assert.IsTrue(updateCounter >= 1);
        task.Dispose();

        updateCounter = 0;
        task = ThreadingService.RunDelay(UpdateDelay, 1);
        yield return new WaitForSeconds(.5f);
        task.Dispose();
        yield return new WaitForSeconds(1.5f);
        //asset early dispose
        Assert.IsTrue(updateCounter == 0);
    }
    void UpdateDelay()
    {
        updateCounter++;
    }


    [TestMethod]
    public IEnumerator TestCoroutine()
    {
        updateCounter = 0;
        ThreadingService.RunRoutine(CoroutineAsync());
        yield return new WaitForSeconds(1);
        Assert.IsTrue(updateCounter == 5);
    }

    IEnumerator CoroutineAsync()
    {
        updateCounter++;
        yield return 1;
        updateCounter++;
        yield return 1;
        updateCounter++;
        yield return 1;
        updateCounter++;
        yield return 1;
        updateCounter++;
        yield return 1;
    }
}