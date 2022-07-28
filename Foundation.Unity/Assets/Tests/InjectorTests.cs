﻿using System.Linq;
using Foundation.Architecture;
using UnityEngine;


[IntegrationTest.Timeout(10)]
[IntegrationTest.DynamicTest("TestScene")]
public class InjectorTests : MonoBehaviour
{
    #region models
     public interface IWordService
    {
        string GetWord();
    }

    public interface ISentanceService
    {
        string GetSentance();
    }
    
    public class WordService : IWordService
    {
        public string GetWord()
        {
            return "World";
        }
    }

    public class SentanceService : ISentanceService
    {
        [Inject]
        public IWordService Words;

        public string GetSentance()
        {
            return "Hello "+Words.GetWord();
        }
    }

    public class Document
    {
        [Inject]
        public ISentanceService Sentances;
        [Inject]
        public IWordService Words;

        public string Print()
        {
            return Sentances.GetSentance();
        }
    }
#endregion

    void Cleanup()
    {
        InjectService.UnregisterAll();
    }

    void Start()
    {
        TestMethod.RunAll(this, Cleanup);

        IntegrationTest.Pass();
    }


    [TestMethod]
    public void TestTransient()
    {
        //Register a concrete type keyed by an interface
        InjectService.RegisterTransient<IWordService, WordService>();
        InjectService.RegisterTransient<ISentanceService, SentanceService>();
        InjectService.RegisterTransient<Document>();

        var document = InjectService.Get<Document>();

        Assert.IsNotNull(document);
        Assert.IsNotNull(document.Sentances);
        Assert.IsNotNull(document.Words);
        Assert.AreEqual(document.Print(), document.Sentances.GetSentance());
    }

    [TestMethod]
    public void TestTransient2()
    {
        //Not we are not using the interface here
        InjectService.RegisterTransient<WordService>();
        Assert.IsNotNull(InjectService.Get<WordService>());

        InjectService.RegisterTransient(() => new SentanceService());
        Assert.IsNotNull(InjectService.Get<SentanceService>());
    }


    [TestMethod]
    public void TestSingleton()
    {
        InjectService.RegisterSingleton<IWordService, WordService>();
        InjectService.RegisterSingleton<ISentanceService, SentanceService>();
        InjectService.RegisterSingleton<Document>();

        var document = InjectService.Get<Document>();

        Assert.IsNotNull(document);
        Assert.IsNotNull(document.Sentances);
        Assert.IsNotNull(document.Words);
        Assert.AreEqual(document.Print(), document.Sentances.GetSentance());


        var document2 = InjectService.Get<Document>();

        Assert.AreEqual(document, document2);
        Assert.AreEqual(document.Words, document2.Words);
        Assert.AreEqual(document.Sentances, document2.Sentances);
    }

    [TestMethod]
    public void TestSingleton2()
    {
        // Note we are NOT using the interface here
        // Since we use a dictionary internally, these are keyed by the concrete type only.
        // We can make the InjectService more flexible, but, that would be a performance hit.

        InjectService.RegisterSingleton<WordService>();
        Assert.IsNotNull(InjectService.Get<WordService>());

        InjectService.RegisterSingleton(() => new SentanceService());
        Assert.IsNotNull(InjectService.Get<SentanceService>());

        InjectService.RegisterSingleton(new Document());
        Assert.IsNotNull(InjectService.Get<Document>());

        InjectService.UnregisterAll();

        InjectService.RegisterSingleton<IWordService, WordService>(new WordService());
        Assert.IsNotNull(InjectService.Get<IWordService>());

        InjectService.RegisterSingleton<ISentanceService, SentanceService>(() => new SentanceService());
        Assert.IsNotNull(InjectService.Get<ISentanceService>());
    }


    [TestMethod]
    public void TestRemove()
    {
        InjectService.RegisterTransient<IWordService, WordService>();
        InjectService.RegisterTransient<ISentanceService, SentanceService>();
        InjectService.RegisterTransient<Document>();


        InjectService.Unregister<Document>();
        var document = InjectService.Get<Document>();
        Assert.IsNull(document);

        InjectService.UnregisterAll();
        Assert.IsNull(InjectService.Get<IWordService>());
        Assert.IsNull(InjectService.Get<ISentanceService>());
    }

    [TestMethod]
    public void TestNull()
    {
        var document = InjectService.Get<Document>();
        Assert.IsNull(document);
    }

    [TestMethod]
    public void TestPrint()
    {
        InjectService.RegisterSingleton<IWordService, WordService>();
        InjectService.RegisterSingleton<ISentanceService, SentanceService>();
        InjectService.RegisterSingleton<Document>();

        var log = InjectService.Print();
        Assert.IsTrue(log.Count() == 3);
    }
}