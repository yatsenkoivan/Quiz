using Quiz;

class Start
{
    static public void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.Title = "Quiz";
        //Console.CursorVisible = false;
        Quiz.Quiz q = new();
        q.Menu();
        //Data d = new();
        User user = new("1", "2", new DateOnly(1,1,1));
        User newuser = new("1", "2", new DateOnly(1,1,1));
        //d.UpdateUser(user, newuser);
    }
}