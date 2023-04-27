using Quiz;

class Start
{
    static public void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.Title = "Quiz";
        Console.CursorVisible = false;
        Quiz.Quiz q = new();
        q.Menu();
    }
}