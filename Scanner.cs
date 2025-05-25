using System.Text;

namespace RenderStuff;

//https://stackoverflow.com/questions/722270/is-there-an-equivalent-to-the-scanner-class-in-c-sharp-for-strings
class Scanner : System.IO.StringReader
{
    string _currentWord;

    public Scanner(string source) : base(source)
    {
        ReadNextWord();
    }

    private void ReadNextWord()
    {
        StringBuilder sb = new StringBuilder();
        char nextChar;
        int next;
        do
        {
            next = this.Read();
            if (next < 0)
                break;
            nextChar = (char)next;
            if (char.IsWhiteSpace(nextChar))
                break;
            sb.Append(nextChar);
        } while (true);
        while((this.Peek() >= 0) && (char.IsWhiteSpace((char)this.Peek())))
            this.Read();
        _currentWord = sb.Length > 0 ? sb.ToString() : null;
    }

    public bool HasNextInt()
    {
        if (_currentWord == null)
            return false;
        return int.TryParse(_currentWord, out _);
    }

    public int NextInt()
    {
        try
        {
            return int.Parse(_currentWord);
        }
        finally
        {
            ReadNextWord();
        }
    }

    public bool HasNextDouble()
    {
        if (_currentWord == null)
            return false;
        return double.TryParse(_currentWord, out _);
    }

    public double NextDouble()
    {
        try
        {
            return double.Parse(_currentWord);
        }
        finally
        {
            ReadNextWord();
        }
    }

    public bool HasNext()
    {
        return _currentWord != null;
    }

    public string Next()
    {
        try
        {
            return _currentWord;
        }
        finally
        {
            ReadNextWord();
        }
    }
}
