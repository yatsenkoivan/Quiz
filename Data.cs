using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace Cursor
{
    class Cursor
    {
        static public char symbol = '■';
        static public int offset_x = 10;
        static public int offset_y = 5;
        static public int dif = 2;
        static public int Move(int limit)
        {
            char key = Console.ReadKey(true).KeyChar;
            int change_pos = 0;
            int current_pos = Console.GetCursorPosition().Top - offset_y;
            switch (key)
            {
                case 'w':
                case 'W':
                    change_pos = -1;
                    break;
                case 's':
                case 'S':
                    change_pos = 1;
                    break;
                case ' ':
                case (char)ConsoleKey.Enter:
                    return current_pos;
            }
            if (current_pos + change_pos > limit || current_pos  + change_pos < 0) return -1;
            current_pos += change_pos;
            Hide();
            Console.SetCursorPosition(offset_x, offset_y + current_pos);
            Show();
            return -1;
        }
        static public void Hide()
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.Write(' ');
            Console.SetCursorPosition(x, y);
        }
        static public void Show()
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.Write(symbol);
            Console.SetCursorPosition(x, y);
        }
    }
}

namespace Quiz
{
    enum Lesson
    {
        Math, Biology, English, IT, Physics, History
    }
    class Quiz
    {
        readonly static private int tab_size = 20;
        readonly static private int error_level = 4;
        readonly private Data data;
        private User? current_user;
        public User? UserInfo
        {
            get { return current_user; }
        }
        static private void Show(string title, string[] msg, int cursor_y=0)
        {
            Console.SetCursorPosition(Cursor.Cursor.offset_x - 2, Cursor.Cursor.offset_y - 2);
            Console.WriteLine(title);
            Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, Cursor.Cursor.offset_y);
            int current = Console.GetCursorPosition().Top;

            foreach(string m in msg)
            {
                Console.Write(m);
                current++;
                Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, current);
            }
            Console.SetCursorPosition(Cursor.Cursor.offset_x, Cursor.Cursor.offset_y+cursor_y);
            Cursor.Cursor.Show();

        }
        static private void ShowValue<T>(T value, int level)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif + tab_size, Cursor.Cursor.offset_y + level);
            Console.WriteLine(value);
            Console.SetCursorPosition(x, y);
        }
        public Quiz()
        {
            data = new Data();
        }
        public void Menu()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();
            string[] msg = {
                "Log in",
                "Register",
                "Exit"
            };
            string title = "Menu";
            Show(title, msg);

            int limit = msg.Length-1;
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            LoginPage();
                            break;
                        case 1:
                            RegisterPage();
                            break;
                        case 2:
                            return;
                    }
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != msg.Length - 1);
        }
        public void LoginPage()
        {
            Console.Clear();
            string[] msg = {
                "Login: ",
                "Password: ",
                "Submit",
                "Back"
            };
            string title = "Login";

            Show(title, msg);

            string login = "";
            string password = "";
            string hidden_pass = "";

            int limit = msg.Length - 1;
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterValue(out login);
                            break;
                        case 1:
                            hidden_pass = EnterPassword(out password);
                            break;
                        case 2:
                            if (login == "")
                            {
                                MSG("! Login cannot be empty !");
                                Console.ReadKey(true);
                                break;
                            }
                            if (password == "")
                            {
                                MSG("! Password cannot be empty !");
                                Console.ReadKey(true);
                                break;
                            }
                            int index = data.Users.FindIndex(user => user.Login == login && user.Password == password);
                            if (index == -1)
                            {
                                MSG("! Login or password is incorrect !");
                                Console.ReadKey(true);
                                break;
                            }
                            current_user = data.Users[index];
                            if (current_user != null) UserMenu();
                            return;
                        case 3:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(login, 0);
                    ShowValue(hidden_pass, 1);
                }
            } while (move != msg.Length - 1);
        }
        public void RegisterPage()
        {
            Console.Clear();
            string[] msg = {
                "Login: ",
                "Password: ",
                "Date of birth: ",
                "Submit",
                "Back"
            };
            string title = "Register";
            Show(title,msg);

            string login = "";
            string password = "";
            int yy = 1, mm = 1, dd = 1;

            int limit = msg.Length - 1;
            int move;

            ShowValue($"{dd}.{mm}.{yy}", 2);
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterValue(out login);
                            break;
                        case 1:
                            EnterPassword(out password, hide : false);
                            break;
                        case 2:
                            EnterValue(out dd);
                            EnterValue(out mm,4);
                            EnterValue(out yy,8);
                            break;
                        case 3:
                            if (login == "")
                            {
                                MSG("! Login cannot be empty !");
                                Console.ReadKey(true);
                                break;
                            }
                            if (password == "")
                            {
                                MSG("! Password cannot be empty !");
                                Console.ReadKey(true);
                                break;
                            }
                            int index = data.Users.FindIndex(user => user.Login == login);
                            if (index != -1)
                            {
                                MSG("! Login already taken !");
                                Console.ReadKey(true);
                                break;
                            
                            }
                            DateOnly dob;
                            try
                            {
                                dob = new(yy, mm, dd);
                            }
                            catch (Exception)
                            {
                                MSG("! Date is incorrect !");
                                Console.ReadKey(true);
                                break;
                            }
                            User user = new(login, password, dob, new Statistic());
                            data.AddUser(user);
                            MSG("  User registered.");
                            Console.ReadKey(true);
                            break;
                        case 4:
                            return;
                    }
                    Console.Clear();
                    Show(title,msg,move);
                    ShowValue(login, 0);
                    ShowValue(password, 1);
                    ShowValue($"{dd}.{mm}.{yy}", 2);
                }
            } while (move != msg.Length-1);
        }
        static public void EnterValue(out string value, int offset = 0)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            Console.Write("                     ");
            Console.SetCursorPosition(x + tab_size + offset, y);
            value = Console.ReadLine() ?? "";
            Console.SetCursorPosition(x, y);
        }
        static public void EnterValue(out int value, int offset=0)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            Console.Write("                     ");
            Console.SetCursorPosition(x + tab_size + offset, y);
            int.TryParse(Console.ReadLine(), out value);
            Console.SetCursorPosition(x, y);
        }
        static public string EnterPassword(out string pass, int offset=0, bool hide=true)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            Console.Write("                     ");
            Console.SetCursorPosition(x + tab_size + offset, y);

            pass = "";
            string result = "";
            char key;
            key = Console.ReadKey(true).KeyChar;
            do
            {
                if (key == (char)ConsoleKey.Backspace && result.Length > 0)
                {
                    if (pass.Length > 0) pass = pass.Remove(pass.Length - 1);
                    if (result.Length > 0) result = result.Remove(result.Length - 1);
                    Console.SetCursorPosition(x + tab_size + offset - 1, y);
                    Console.Write(' ');
                    x--;
                    Console.SetCursorPosition(x + tab_size + offset, y);
                }
                else if (Char.IsLetterOrDigit(key) || Char.IsSymbol(key))
                {
                    pass += key;
                    result += '*';
                    if (hide) Console.Write('*');
                    else Console.Write(key);
                    x++;
                }
                key = Console.ReadKey(true).KeyChar;
            } while (key != (char)ConsoleKey.Enter);
            return result;
        }
        public void UserMenu()
        {
            if (current_user == null) return;
            if (current_user.Login == "admin") Console.BackgroundColor = ConsoleColor.DarkGreen;
            else Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            string[] msg = {
                "Quizes",
                "Statistic",
                "Settings",
                "Logout"
            };
            string title = "Logged in: ";
            if (current_user != null) title += current_user.Login;
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            QuizesChooseMenu();
                            break;
                        case 1:
                            Stats();
                            break;
                        case 2:
                            Settings();
                            break;
                        case 3:
                            current_user = null;
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != msg.Length - 1);
        }
        public void QuizesChooseMenu()
        {
            if (current_user == null) return;
            Console.Clear();
            string[] msg = typeof(Lesson).GetEnumNames().Append("Back").ToArray();
            string title = "Choose quiz";
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;

            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (move == limit)
                    {
                        return;
                    }
                    LessonMenu(move);
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != msg.Length - 1);
        }
        public void LessonMenu(int lesson)
        {
            if (current_user == null) return;
            Console.Clear();
            string[] msg = { "Play", "Leaderboard", "Back" };

            //ADMIN
            if (current_user.Login == "admin")
            {
                msg = msg.Append("Edit").ToArray();
            }

            string title = typeof(Lesson).GetEnumNames()[lesson];
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;

            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            Play(lesson);
                            break;
                        case 1:
                            //Leaderboard(lesson)
                            break;
                        case 2:
                            return;
                        case 3:
                            if (current_user.Login == "admin")
                            {
                                Edit(lesson);
                            }
                            break;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != 2);
        }
        public void Play(int lesson)
        {
            if (current_user == null || lesson >= data.LessonInfo.Count) return;
            Console.Clear();

            string title;
            int limit;
            int move;
            bool[] isTrue;


            string[] msg;


            uint correct = 0;
            bool flag = true; //answer check
            foreach(Question current in data.LessonInfo[lesson].Questions)
            {
                Console.Clear();
                title = current.Text;
                msg = new string[current.Answers.Count+1]; //+Submit
                for (int ans=0; ans < current.Answers.Count; ans++)
                {
                    msg[ans] = current.Answers[ans].ToString();
                }
                msg[msg.Length - 1] = "Submit";

                limit = msg.Length-1;
                isTrue = new bool[current.Answers.Count];

                Show(title, msg);

                do
                {
                    move = Cursor.Cursor.Move(limit);
                    if (move != -1)
                    {
                        if (move == limit)
                        {
                            flag = true;
                            for (int ans=0; ans<current.Answers.Count && flag; ans++)
                            {
                                if ((isTrue[ans] && current.Answers[ans].isTrue == false)
                                    ||
                                    (isTrue[ans]==false && current.Answers[ans].isTrue == true))
                                {
                                    flag = false;
                                }
                            }
                            if (flag) correct++;
                        }
                        else
                        {
                            if (isTrue[move]) isTrue[move] = false;
                            else isTrue[move] = true;
                        }
                        Console.Clear();
                        Show(title, msg, move);
                        for (int level = 0; level < current.Answers.Count; level++)
                        {
                            ShowValue((isTrue[level] ? " *" : ""), level);
                        }
                    }
                } while (move != limit);
            }
            Console.Clear();
            ShowValue("Your result:", 0);
            ShowValue($"{correct}/{ data.LessonInfo[lesson].Questions.Count}",1);
            ShowValue("Press any key to continue ...", 3);
            Console.ReadKey(true);
        }
        public void Edit(int lesson)
        {
            if (current_user == null || current_user.Login != "admin") return;
            Console.Clear();
            string[] msg = {
                "Show questions",
                "Add question",
                "Edit question",
                "Clear leaderboard",
                "Back"
            };
            string title = $"Edit quiz {typeof(Lesson).GetEnumNames()[lesson]}";
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            ShowQuestions(lesson);
                            break;
                        case 1:
                            AddQuestion(lesson);
                            break;
                        case 2:
                            //EditQuestion(lesson);
                            break;
                        case 3:
                            //ClearLeaderboard(lesson);
                            break;
                        case 4:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != msg.Length - 1);
        }
        public void ShowQuestions(int lesson)
        {
            if (current_user == null || current_user.Login != "admin") return;
            if (lesson >= data.LessonInfo.Count) return;
            Console.Clear();
            data.LessonInfo[lesson].ShowQuestions();
            Console.ReadKey(true);
        }
        public void AddQuestion(int lesson)
        {
            if (lesson >= data.LessonInfo.Count) return;
            Console.Clear();
            string[] msg = {
                "Text: ",
                "Amount of answers: ",
                "Submit",
                "Back"
            };
            string title = "Add question";
            Show(title, msg);

            string text = "";
            int amount = 0;

            int limit = msg.Length - 1;
            int move;

            ShowValue(text, 0);
            ShowValue(amount, 1);
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterValue(out text);
                            break;
                        case 1:
                            EnterValue(out amount);
                            break;
                        case 2:
                            if (amount <= 0)
                            {
                                MSG("! Amount of answers value is incorrect !");
                            }
                            else if (text == "")
                            {
                                MSG("! Text cannot be empty !");
                            }
                            else
                            {
                                List<Answer>? answers = AddQuestionAnswers(amount);
                                if (answers == null) break;
                                Question q = new(text, answers);
                                data.LessonInfo[lesson].AddQuestion(q);
                                data.WriteData();
                                return;
                            }
                            break;
                        case 3:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(text, 0);
                    ShowValue(amount, 1);
                }
            } while (move != msg.Length - 1);
        }
        public List<Answer>? AddQuestionAnswers(int answers_amount)
        {
            Console.Clear();
            string[] msg = new string[answers_amount * 2 + 2];
            for (int index=0; index<answers_amount*2; index+=2)
            {
                msg[index] = $"{index/2 + 1}. Text: ";
                msg[index + 1] = $"{index/2 + 1}. IsTrue: ";
            }
            int submit_level = answers_amount * 2;
            int back_level = answers_amount * 2 + 1;

            msg[submit_level] = "Submit";
            msg[back_level] = "Back";

            string title = "Enter answers";
            Show(title, msg);

            string[] answers_text = new string[answers_amount];
            bool[] answers_true = new bool[answers_amount];

            int limit = msg.Length - 1;
            int move;


            for (int level=0; level<answers_amount; level++)
            {
                ShowValue(answers_text[level], level*2);
                ShowValue((answers_true[level] ? " (*)" : ""), level*2+1);
            }
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (move == back_level) return null;
                    else if (move == submit_level)
                    {
                        List<Answer> res = new List<Answer>();
                        for (int index=0; index< answers_amount; index++)
                        {
                            res.Add(new Answer(answers_text[index], answers_true[index]));
                        }
                        return res;
                    }
                    else
                    {
                        if (move%2 == 0) //Text
                        {
                            EnterValue(out answers_text[move / 2], move / 2);
                        }
                        else //IsTrue
                        {
                            if (answers_true[move / 2]) answers_true[move / 2] = false;
                            else answers_true[move / 2] = true;
                        }
                    }
                    Console.Clear();
                    Show(title, msg,move);
                    for (int level = 0; level < answers_amount; level++)
                    {
                        ShowValue(answers_text[level], level * 2);
                        ShowValue((answers_true[level] ? " (*)" : ""), level * 2 + 1);
                    }
                }
            } while (move != msg.Length - 1);
            return null;
        }
        public void Stats()
        {
            if (current_user == null) return;
            Console.Clear();
            Console.WriteLine(" -------- \tAVG\tBEST\tGAMES");
            foreach(Lesson lesson in typeof(Lesson).GetEnumValues())
            {
                Console.WriteLine($"" +
                    $"{lesson}\t\t" +
                    $"{current_user.Stats.GetAvgInfo(lesson)}\t" +
                    $"{current_user.Stats.GetBestInfo(lesson)}\t" +
                    $"{current_user.Stats.GetGames(lesson)}");
            }
            Console.Write("\nPress any key to return ...");
            Console.ReadKey(true);
        }
        public void Settings()
        {
            if (current_user == null) return;
            Console.Clear();
            string[] msg = {
                "Password: ",
                "Date of Birth: ",
                "Save",
                "Back"
            };
            string title = "Settings: " + current_user.Login;
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;

            string password = current_user.Password;
            int dd = current_user.BirthDate.Day;
            int mm = current_user.BirthDate.Month;
            int yy = current_user.BirthDate.Year;

            ShowValue(password, 0);
            ShowValue($"{dd}.{mm}.{yy}", 1);

            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterPassword(out password, hide: false);
                            break;
                        case 1:
                            EnterValue(out dd);
                            EnterValue(out mm, 4);
                            EnterValue(out yy, 8);
                            break;
                        case 2:
                            if (password == "")
                            {
                                MSG("! Password cannot be empty !");
                                Console.ReadKey(true);
                                break;
                            }
                            DateOnly dob;
                            try
                            {
                                dob = new(yy, mm, dd);
                            }
                            catch (Exception)
                            {
                                MSG("! Date is incorrect !");
                                Console.ReadKey(true);
                                break;
                            }
                            User? newUser = current_user.Clone() as User;
                            if (newUser != null)
                            {
                                newUser.SetPassword(password);
                                newUser.SetDOB(new DateOnly(yy, mm, dd));
                                data.UpdateUser(ref current_user, newUser);
                                MSG("  Info saved.");
                                Console.ReadKey(true);
                                return;
                            }
                            break;
                        case 3:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(password, 0);
                    ShowValue($"{dd}.{mm}.{yy}", 1);
                }
            } while (move != msg.Length - 1);
        }
        static public void MSG(string msg)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x, y + error_level);
            Console.WriteLine(msg);
            Console.SetCursorPosition(x, y);
        }

    }
    [Serializable]
    class User : ICloneable
    {
        readonly private string login;
        private string password;
        private DateOnly birthDate;
        readonly private Statistic stats;
        public string Login
        {
            get { return login; }
        }
        public string Password
        {
            get { return password; }
        }
        public DateOnly BirthDate
        {
            get { return birthDate; }
        }
        public Statistic Stats
        {
            get { return stats; }
        }
        public User(string login, string password, DateOnly birthDate, Statistic stats)
        {
            this.login = login;
            this.password = password;
            this.birthDate = birthDate;
            this.stats = stats;
        }
        public void SetPassword(string newpass)
        {
            this.password = newpass;
        }
        public void SetDOB(DateOnly dob)
        {
            birthDate = dob;
        }
        public object Clone()
        {
            return new User(login, password, birthDate, stats);
        }
    }
    [Serializable]
    class Statistic
    {
        readonly private double[] avg_score;
        readonly private uint[] games;
        readonly private double[] best_score;
        public Statistic()
        {
            avg_score = new double[typeof(Lesson).GetEnumValues().Length];
            best_score = new double[typeof(Lesson).GetEnumValues().Length];
            games = new uint[typeof(Lesson).GetEnumValues().Length];
        }
        public Statistic(double[] avg_score, uint[] games, double[] best_score)
        {
            this.avg_score = avg_score;
            this.games = games;
            this.best_score = best_score;
        }

        public double GetAvgInfo(Lesson lesson)
        {
            return avg_score[(int)lesson];
        }
        public double GetBestInfo(Lesson lesson)
        {
            return best_score[(int)lesson];
        }
        public uint GetGames(Lesson lesson)
        {
            return games[(int)lesson];
        }
        public void Played(Lesson lesson, int value)
        {
            uint games_amount = games[(int)lesson];
            avg_score[(int)lesson] = (avg_score[(int)lesson] * games_amount + value) / games_amount + 1;
            games[(int)lesson]++;
        }
    }
    [Serializable]
    class Answer
    {
        private string text;
        private bool is_true;
        public Answer(string text, bool is_true)
        {
            this.text = text;
            this.is_true = is_true;
        }
        public string Text
        {
            get { return text; }
        }
        public bool isTrue
        {
            get { return is_true; }
        }
        public override string ToString()
        {
            return text;
        }
    }
    [Serializable]
    class Question
    {
        private string text;
        private List<Answer> answers;
        public string Text
        {
            get { return text; }
        }
        public List<Answer> Answers
        {
            get { return answers; }
        }
        public Question(string text, List<Answer> answers)
        {
            this.text = text;
            this.answers = answers;
        }

        public void ShowInfo()
        {
            Console.WriteLine(text);
            foreach (Answer answer in answers)
            {
                Console.WriteLine(answer.ToString() + (answer.isTrue ? " (*)" : " "));
            }
            Console.WriteLine("-------------------------");
        }
    }
    [Serializable]
    class Leaderboard
    {
        private Dictionary<string, uint> data;
        public Leaderboard()
        {
            data = new();
        }
    }
    [Serializable]
    class LessonInfo
    {
        private List<Question> questions;
        private Leaderboard leaderboard;
        public LessonInfo()
        {
            questions = new List<Question>();
            leaderboard = new();
        }
        public List<Question> Questions
        {
            get { return questions; }
        }
        public void ShowQuestions()
        {
            foreach (Question q in questions)
            {
                q.ShowInfo();
            }
        }
        public void AddQuestion(Question q)
        {
            questions.Add(q);
        }
    }
    class Data
    {
        private List<User> users;
        private List<LessonInfo> lessonInfo;
        public List<User> Users
        {
            get { return users; }
        }
        public List<LessonInfo> LessonInfo
        {
            get { return lessonInfo; }
        }
        public Data()
        {
            BinaryFormatter bf = new();

            //USERS
            users = new List<User>();
            FileStream fs_users = new("users.bin", FileMode.OpenOrCreate, FileAccess.Read);

            string login;
            string password;
            int yy, mm, dd;
            Statistic stats;

            try
            {
                while (fs_users.Length != fs_users.Position)
                {
                    login = (string)bf.Deserialize(fs_users);
                    password = (string)bf.Deserialize(fs_users);
                    stats = (Statistic)bf.Deserialize(fs_users);
                    yy = (int)bf.Deserialize(fs_users);
                    mm = (int)bf.Deserialize(fs_users);
                    dd = (int)bf.Deserialize(fs_users);
                    users.Add(new User(login, password, new DateOnly(yy, mm, dd), stats));
                }
            }
            catch (Exception)
            {}
            fs_users.Close();

            //Lessons
            FileStream fs_data = new("data.bin", FileMode.OpenOrCreate, FileAccess.Read);
            lessonInfo = new List<LessonInfo>();
            for(int lesson= 0; lesson < typeof(Lesson).GetEnumValues().Length; lesson++)
            {
                try
                {
                    lessonInfo.Add((LessonInfo)bf.Deserialize(fs_data));
                }
                catch (Exception)
                {
                    lessonInfo.Add(new());
                }
            }
            fs_data.Close();
        }
        private void WriteUser(User user)
        {
            if (File.Exists("users.bin") == false)
            {
                File.Create("users.bin");
            }
            FileStream fs = new("users.bin", FileMode.Append, FileAccess.Write);
            BinaryFormatter bf = new();
            bf.Serialize(fs, user.Login);
            bf.Serialize(fs, user.Password);
            bf.Serialize(fs, user.Stats);
            bf.Serialize(fs, user.BirthDate.Year);
            bf.Serialize(fs, user.BirthDate.Month);
            bf.Serialize(fs, user.BirthDate.Day);
            fs.Close();
        }
        private void WriteUsers()
        {
            FileStream fs = new("users.bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new();
            foreach(User user in users)
            {
                bf.Serialize(fs, user.Login);
                bf.Serialize(fs, user.Password);
                bf.Serialize(fs, user.Stats);
                bf.Serialize(fs, user.BirthDate.Year);
                bf.Serialize(fs, user.BirthDate.Month);
                bf.Serialize(fs, user.BirthDate.Day);
            }
            fs.Close();
        }
        public void WriteData()
        {
            FileStream fs = new("data.bin", FileMode.OpenOrCreate);
            BinaryFormatter bf = new();
            foreach (LessonInfo lesson in lessonInfo)
            {
                bf.Serialize(fs, lesson);
            }
            fs.Close();
        }
        public void AddUser(User user)
        {
            users.Add(user);
            WriteUser(user);
        }
        public void UpdateUser(ref User user, User newUser)
        {
            
            int index = users.IndexOf(user);
            if (index != -1)
            {
                users[index] = newUser;
            }
            user = newUser;
            WriteUsers();
        }
    }
}