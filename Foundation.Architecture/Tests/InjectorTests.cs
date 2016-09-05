using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class InjectorTests
    {
        protected IInjectService Container;

        [TestInitialize]
        public void TestInitialize()
        {
            Container = new InjectService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Container != null)
            {
                Container.UnregisterAll();
                Container = null;
            }
        }

        [TestMethod]
        public void TestTransient()
        {
            //Register a concrete type keyed by an interface
            Container.RegisterTransient<IWordService, WordService>();
            Container.RegisterTransient<ISentanceService, SentanceService>();
            Container.RegisterTransient<Document>();

            var document = Container.Get<Document>();

            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Sentances);
            Assert.IsNotNull(document.Words);
            Assert.AreEqual(document.Print(), document.Sentances.GetSentance());
        }

        [TestMethod]
        public void TestTransient2()
        {
            //Not we are not using the interface here
            Container.RegisterTransient<WordService>();
            Assert.IsNotNull(Container.Get<WordService>());

            Container.RegisterTransient(() => new SentanceService());
            Assert.IsNotNull(Container.Get<SentanceService>());
        }


        [TestMethod]
        public void TestSingleton()
        {
            Container.RegisterSingleton<IWordService, WordService>();
            Container.RegisterSingleton<ISentanceService, SentanceService>();
            Container.RegisterSingleton<Document>();

            var document = Container.Get<Document>();

            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Sentances);
            Assert.IsNotNull(document.Words);
            Assert.AreEqual(document.Print(), document.Sentances.GetSentance());


            var document2 = Container.Get<Document>();

            Assert.AreEqual(document, document2);
            Assert.AreEqual(document.Words, document2.Words);
            Assert.AreEqual(document.Sentances, document2.Sentances);
        }

        [TestMethod]
        public void TestSingleton2()
        {
            // Note we are NOT using the interface here
            // Since we use a dictionary internally, these are keyed by the concrete type only.
            // We can make the container more flexible, but, that would be a performance hit.

            Container.RegisterSingleton<WordService>();
            Assert.IsNotNull(Container.Get<WordService>());

            Container.RegisterSingleton(() => new SentanceService());
            Assert.IsNotNull(Container.Get<SentanceService>());

            Container.RegisterSingleton(new Document());
            Assert.IsNotNull(Container.Get<Document>());

            Container.UnregisterAll();

            Container.RegisterSingleton<IWordService, WordService>(new WordService());
            Assert.IsNotNull(Container.Get<IWordService>());

            Container.RegisterSingleton<ISentanceService, SentanceService>(() => new SentanceService());
            Assert.IsNotNull(Container.Get<ISentanceService>());
        }


        [TestMethod]
        public void TestRemove()
        {
            Container.RegisterTransient<IWordService, WordService>();
            Container.RegisterTransient<ISentanceService, SentanceService>();
            Container.RegisterTransient<Document>();


            Container.Unregister<Document>();
            var document = Container.Get<Document>();
            Assert.IsNull(document);

            Container.UnregisterAll();
            Assert.IsNull(Container.Get<IWordService>());
            Assert.IsNull(Container.Get<ISentanceService>());
        }

        [TestMethod]
        public void TestNull()
        {
            var document = Container.Get<Document>();
            Assert.IsNull(document);
        }

        [TestMethod]
        public void TestPrint()
        {
            Container.RegisterSingleton<IWordService, WordService>();
            Container.RegisterSingleton<ISentanceService, SentanceService>();
            Container.RegisterSingleton<Document>();

            var log = Container.Print();
            Assert.IsTrue(log.Count() == 3);
        }

    }
}
