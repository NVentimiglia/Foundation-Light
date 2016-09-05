namespace Foundation.Architecture.Tests
{
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
}
