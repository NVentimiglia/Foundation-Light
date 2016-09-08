using System.Collections;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class ThreadingTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            updateCounter = 0;
        }

        private double updateCounter;

        [TestMethod]
        public async Task TestUpdate()
        {
            var task = ThreadingService.RunUpdate(Update);

            await Task.Delay(1000);

            //Asset works
            Assert.IsTrue(updateCounter >= 900);

            task.Dispose();

            var old = updateCounter;

            await Task.Delay(1000);

            //Asset Dispose works
            Assert.IsTrue(updateCounter == old);
        }
        void Update(double delta)
        {
            updateCounter += delta;
        }

        [TestMethod]
        public async Task TestDelay()
        {
            var task = ThreadingService.RunDelay(UpdateDelay, 1000);
            await Task.Delay(500);
            //Asset Not early executed
            Assert.IsTrue(updateCounter == 0);
            await Task.Delay(1500);
            //Asset Executed in time
            Assert.IsTrue(updateCounter == 1);
            task.Dispose();

            updateCounter = 0;
            task = ThreadingService.RunDelay(UpdateDelay, 1000);
            await Task.Delay(500);
            task.Dispose();
            await Task.Delay(1500);
            //asset early dispose
            Assert.IsTrue(updateCounter == 0);
        }
        void UpdateDelay()
        {
            updateCounter++;
        }


        [TestMethod]
        public async Task TestCoroutine()
        {
            updateCounter = 0;
            ThreadingService.RunRoutine(CoroutineAsync());
            await Task.Delay(100);
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
}

