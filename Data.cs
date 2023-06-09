﻿using System.Runtime.Serialization.Formatters.Binary;

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
            ConsoleKey key = Console.ReadKey(true).Key;
            int change_pos = 0;
            int current_pos = Console.GetCursorPosition().Top - offset_y;
            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    change_pos = -1;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    change_pos = 1;
                    break;
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
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
    class Quiz
    {
        readonly static private string admin = "admin";
        readonly static private int tab_size = 20;
        readonly static private int error_level = 4;
        readonly private Data data;
        private User? current_user;
        public User? UserInfo
        {
            get { return current_user; }
        }
        static private void Show(string title, string[] msg, int cursor_y = 0)
        {
            Console.SetCursorPosition(Cursor.Cursor.offset_x - 2, Cursor.Cursor.offset_y - 2);
            Console.WriteLine(title);
            Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, Cursor.Cursor.offset_y);
            int current = Console.GetCursorPosition().Top;

            foreach (string m in msg)
            {
                Console.Write(m);
                current++;
                Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, current);
            }
            Console.SetCursorPosition(Cursor.Cursor.offset_x, Cursor.Cursor.offset_y + cursor_y);
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
                            if (login == "") MSG("! Login cannot be empty !");
                            else if (password == "") MSG("! Password cannot be empty !");
                            else
                            {
                                int index = data.Users.FindIndex(user => user.Login == login && user.Password == password);
                                if (index == -1) MSG("! Login or password is incorrect !");
                                else
                                {
                                    current_user = data.Users[index];
                                    if (current_user != null) UserMenu();
                                    return;
                                }
                            }
                            break;
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
            Show(title, msg);

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
                            EnterPassword(out password, hide: false);
                            break;
                        case 2:
                            EnterDOB(out dd, out mm, out yy);
                            break;
                        case 3:
                            if (login == "") MSG("! Login cannot be empty !");
                            else if (password == "") MSG("! Password cannot be empty !");
                            else
                            {
                                int index = data.Users.FindIndex(user => user.Login == login);
                                if (index != -1) MSG("! Login already taken !");
                                else
                                {
                                    DateOnly dob;
                                    try
                                    {
                                        dob = new(yy, mm, dd);
                                    }
                                    catch (Exception)
                                    {
                                        MSG("! Date is incorrect !");
                                        break;
                                    }
                                    User user = new(login, password, dob);
                                    data.AddUser(user);
                                    MSG("  User registered.");
                                    Console.ReadKey(true);
                                    break;
                                }
                            }
                            break;
                        case 4:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(login, 0);
                    ShowValue(password, 1);
                    ShowValue($"{dd}.{mm}.{yy}", 2);
                }
            } while (move != msg.Length - 1);
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
        static public void EnterValue(out int value, int offset = 0)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            Console.Write("                     ");
            Console.SetCursorPosition(x + tab_size + offset, y);
            int.TryParse(Console.ReadLine(), out value);
            Console.SetCursorPosition(x, y);
        }
        static public string EnterPassword(out string pass, int offset = 0, bool hide = true)
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
        static public void EnterDOB(out int yy, out int mm, out int dd)
        {
            string input;
            int[] res = Array.Empty<int>();
            int temp;
            do
            {
                EnterValue(out input, res.Length * 4);
                input = input.Replace(' ', '.').Replace('/', '.');
                string[] arr = input.Split('.');
                foreach (string inp in arr)
                {
                    if (int.TryParse(inp, out temp) == false) continue;
                    res = res.Append(temp).ToArray();
                }
            } while (res.Length < 3);
            yy = res[0];
            mm = res[1];
            dd = res[2];
        }
        public void UserMenu()
        {
            if (current_user == null) return;
            if (current_user.Login == admin) Console.BackgroundColor = ConsoleColor.DarkMagenta;
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
            Console.Clear();
            if (current_user == null) return;
            string[] msg = (from lesson in data.LessonInfo select lesson.Name)
                            .Append("Back")
                            .ToArray();
            if (current_user.Login == admin)
            {
                msg = msg.Append("Add").Append("Remove").ToArray();
            }
            string title = "Choose quiz";
            Show(title, msg);

            int limit = msg.Length - 1;
            int move;

            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (move == limit - 2 && current_user.Login == admin) return;
                    if (move == limit - 1 && current_user.Login == admin)
                    {
                        AddQuiz();
                        break;
                    }
                    if (move == limit)
                    {
                        if (current_user.Login == admin)
                        {
                            RemoveQuiz();
                            break;
                        }
                        else
                        {
                            return;
                        }
                    }
                    LessonMenu(move);
                    Console.Clear();
                    Show(title, msg, move);
                }
            } while (move != msg.Length - 1);
        }
        public void AddQuiz()
        {
            Console.Clear();
            string[] msg = {
                "Name: ",
                "Submit",
                "Back"
            };
            string title = "Add quiz";
            Show(title, msg);

            string text = "";

            int limit = msg.Length - 1;
            int move;

            ShowValue(text, 0);
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
                            if (text == "") MSG("! Name cannot be empty !");
                            else if (data.LessonInfo.Any(lesson => lesson.Name == text)) MSG("! Name is already taken !");
                            else
                            {
                                data.LessonInfo.Add(new LessonInfo(text));
                                data.WriteData();
                                return;
                            }
                            break;
                        case 2:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(text, 0);
                }
            } while (move != msg.Length - 1);
        }
        public void RemoveQuiz()
        {
            Console.Clear();
            string[] msg = {
                "Name: ",
                "Submit",
                "Back"
            };
            string title = "Remove quiz";
            Show(title, msg);

            string name = "";
            int index;

            int limit = msg.Length - 1;
            int move;

            ShowValue(name, 0);
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterValue(out name);
                            break;
                        case 1:
                            if (name == "")
                            {
                                MSG("! Name cannot be empty !");
                                break;
                            }
                            index = data.LessonInfo.FindIndex(l => l.Name == name);
                            if (index == -1) MSG("! Quiz not found !");
                            else
                            {
                                data.LessonInfo.RemoveAt(index);
                                data.WriteData();
                                return;
                            }
                            break;
                        case 2:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(name, 0);
                }
            } while (move != msg.Length - 1);
        }
        public void LessonMenu(int lesson)
        {
            if (current_user == null) return;
            if (lesson < 0 || lesson > data.LessonInfo.Count) return;
            Console.Clear();
            string[] msg = { "Play", "Leaderboard", "Back" };

            //ADMIN
            if (current_user.Login == admin)
            {
                msg = msg.Append("Edit").ToArray();
            }

            string title = data.LessonInfo[lesson].Name;
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
                            ShowLeaderboard(lesson);
                            break;
                        case 2:
                            return;
                        case 3:
                            if (current_user.Login == admin)
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

            if (data.LessonInfo[lesson].Questions.Count == 0)
            {
                ShowValue("This quiz in not done yet", 0);
                ShowValue("Try again later", 1);
                ShowValue("Press any key to continue ...", 3);
                Console.ReadKey(true);
                return;
            }

            string title;
            int limit;
            int move;
            bool[] isTrue;


            string[] msg;


            int correct = 0;
            bool flag = true; //answer check
            foreach (Question current in data.LessonInfo[lesson].Questions)
            {
                Console.Clear();
                title = current.Text;
                msg = new string[current.Answers.Count + 1]; //+Submit
                for (int ans = 0; ans < current.Answers.Count; ans++)
                {
                    msg[ans] = current.Answers[ans].ToString();
                }
                msg[msg.Length - 1] = "Submit";

                limit = msg.Length - 1;
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
                            for (int ans = 0; ans < current.Answers.Count && flag; ans++)
                            {
                                if ((isTrue[ans] && current.Answers[ans].isTrue == false)
                                    ||
                                    (isTrue[ans] == false && current.Answers[ans].isTrue == true))
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
            data.LessonInfo[lesson].Played(current_user.Login, correct);
            //(re)write to leaderboard
            data.LessonInfo[lesson].Leaderboard.WriteResult(current_user.Login, correct);
            data.WriteData();
            data.WriteUsers();
            ShowValue("Your result:", 0);
            ShowValue($"{correct}/{data.LessonInfo[lesson].Questions.Count}", 1);
            ShowValue("Press any key to continue ...", 3);
            Console.ReadKey(true);
        }
        public void ShowLeaderboard(int lesson)
        {
            Console.Clear();
            data.LessonInfo[lesson].Leaderboard.Show();
            Console.WriteLine("\nPress any key to continue ...");
            Console.ReadKey(true);
        }
        public void Edit(int lesson)
        {
            if (current_user == null || current_user.Login != admin) return;
            Console.Clear();
            string[] msg = {
                "Show questions",
                "Add question",
                "Edit question",
                "Clear leaderboard",
                "Back"
            };
            string title = $"Edit quiz {data.LessonInfo[lesson].Name}";
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
                            EditQuestion(lesson);
                            break;
                        case 3:
                            ClearLeaderboard(lesson);
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
            if (current_user == null || current_user.Login != admin) return;
            if (lesson >= data.LessonInfo.Count) return;
            Console.Clear();
            data.LessonInfo[lesson].ShowQuestions();
            if (data.LessonInfo[lesson].Questions.Count == 0)
                Console.WriteLine("No data");
            Console.WriteLine("\nPress any key to continue");
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
                            if (amount <= 0) MSG("! Amount of answers value is incorrect !");
                            else if (text == "") MSG("! Text cannot be empty !");
                            else
                            {
                                List<Answer>? answers = EditQuestionAnswers(amount, text);
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
        public void EditQuestion(int lesson)
        {
            if (lesson >= data.LessonInfo.Count) return;
            Console.Clear();
            string[] msg = {
                "Index: ",
                "Submit",
                "Back"
            };
            string title = "Edit question";
            Show(title, msg);

            int index = 0;

            int limit = msg.Length - 1;
            int move;

            ShowValue(index, 0);
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    switch (move)
                    {
                        case 0:
                            EnterValue(out index);
                            break;
                        case 1:
                            if (data.LessonInfo[lesson].Questions.Count <= index) MSG("! Question not found !");
                            else
                            {
                                List<Answer>? answers =
                                EditQuestionAnswers(data.LessonInfo[lesson].Questions[index].Answers.Count,
                                                    data.LessonInfo[lesson].Questions[index].Text,
                                                    data.LessonInfo[lesson].Questions[index].Answers);
                                if (answers != null)
                                {
                                    data.LessonInfo[lesson].Questions[index].Answers = answers;
                                    data.WriteData();
                                }
                                return;
                            }
                            break;
                        case 2:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg, move);
                    ShowValue(index, 0);
                }
            } while (move != msg.Length - 1);
        }
        public List<Answer>? EditQuestionAnswers(int answers_amount, string question, List<Answer>? answers = null)
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

            string title = "Enter answers: " + question;
            Show(title, msg);

            string[] answers_text;
            bool[] answers_true;
            if (answers == null)
            {
                answers_text = new string[answers_amount];
                answers_true = new bool[answers_amount];
            }
            else
            {
                answers_text = (from answer in answers select answer.Text).ToArray();
                answers_true = (from answer in answers select answer.isTrue).ToArray();
            }

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
                            EnterValue(out answers_text[move / 2]);
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
        public void ClearLeaderboard(int lesson)
        {
            data.LessonInfo[lesson].Leaderboard.Clear();
            data.WriteData();
        }
        public void Stats()
        {
            if (current_user == null) return;
            Console.Clear();

            bool flag = true;
            foreach(LessonInfo l in data.LessonInfo)
            {
                if (l.Stats.ContainsKey(current_user.Login) == false) continue;
                else if (flag)
                {
                    Console.WriteLine(" -------- \tAVG\tBEST\tGAMES");
                    flag = false;
                }
                Console.WriteLine($"" +
                $"{l.Name}\t\t" +
                $"{Math.Round(l.Stats[current_user.Login].AVG, 3)}\t" +
                $"{l.Stats[current_user.Login].Best}\t" +
                $"{l.Stats[current_user.Login].Games}");
            }
            if (flag)
            {
                Console.WriteLine("No data.");
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
                            EnterDOB(out dd, out mm, out yy);
                            break;
                        case 2:
                            if (password == "")
                            {
                                MSG("! Password cannot be empty !");
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
                                break;
                            }
                            User? newUser = current_user.Clone() as User;
                            if (newUser != null)
                            {
                                newUser.SetPassword(password);
                                newUser.SetDOB(new DateOnly(yy, mm, dd));
                                data.UpdateUser(ref current_user, newUser);
                                MSG("  Info saved.");
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
            Console.ReadKey(true);
            Console.SetCursorPosition(x, y);
        }

    }
    [Serializable]
    class User : ICloneable
    {
        readonly private string login;
        private string password;
        private DateOnly birthDate;
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
        public User(string login, string password, DateOnly birthDate)
        {
            this.login = login;
            this.password = password;
            this.birthDate = birthDate;
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
            return new User(login, password, birthDate);
        }
    }
    [Serializable]
    class Statistic
    {
        private double avg_score;
        private int games;
        private int best_score;
        public int Games
        {
            get { return games; }
            set { games = (value >= 0 ? value : 0); }
        }
        public double AVG
        {
            get { return avg_score; }
            set { avg_score = (value >= 0 ? value : 0); }
        }
        public int Best
        {
            get { return best_score; }
            set { best_score = (value >= 0 ? value : 0); }
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
            set { answers = value; }
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
        private Dictionary<string, int> data;
        public Leaderboard()
        {
            data = new();
        }
        public void WriteResult(string login, int value)
        {
            if (data.ContainsKey(login))
            {
                if (data[login] < value)
                {
                    if (data[login] < value)
                    {
                        data[login] = value;
                    }
                }
            }
            else
            {
                data.Add(login, value);
            }
            Sort();
        }
        public void Show()
        {
            for (int result = 0; result < data.Count; result++)
            {
                Console.WriteLine($"{result + 1})" +
                    $"{data.ElementAt(result).Key} - {data.ElementAt(result).Value}");
            }
            if (data.Count == 0)
            {
                Console.WriteLine("No data");
            }
        }
        private void Sort()
        {
            data = data.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
        public void Clear()
        {
            data.Clear();
        }
    }
    [Serializable]
    class LessonInfo
    {
        private string name;
        private List<Question> questions;
        private Leaderboard leaderboard;
        private SortedDictionary<string, Statistic> stats;
        public LessonInfo(string name)
        {
            this.name = name;
            questions = new List<Question>();
            leaderboard = new();
            stats = new();
        }
        public SortedDictionary<string, Statistic> Stats
        {
            get { return stats; }
        }
        public string Name
        {
            get { return name; }
        }
        public Leaderboard Leaderboard
        {
            get { return leaderboard; }
        }
        public List<Question> Questions
        {
            get { return questions; }
        }
        public void ShowQuestions()
        {
            int index = 1;
            foreach (Question q in questions)
            {
                Console.WriteLine($"{index})");
                q.ShowInfo();
                index++;
            }
        }
        public void AddQuestion(Question q)
        {
            questions.Add(q);
        }
        public void Played(string name, int result)
        {
            if (stats.ContainsKey(name))
            {
                stats[name].Best = Math.Max(stats[name].Best, result);
                stats[name].AVG = (stats[name].AVG * stats[name].Games + result) / (stats[name].Games+1);
                stats[name].Games++;
            }
            else
            {
                stats.Add(name, new Statistic() { Games = 1, Best = result, AVG = result });
            }
            leaderboard.WriteResult(name, result);
        }
    }
    class Data
    {
        static private string user_data_path = "users.bin";
        static private string data_path = "data.bin";
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
            FileStream fs_users = new(user_data_path, FileMode.OpenOrCreate, FileAccess.Read);

            string login;
            string password;
            int yy, mm, dd;

            try
            {
                while (fs_users.Length != fs_users.Position)
                {
                    login = (string)bf.Deserialize(fs_users);
                    password = (string)bf.Deserialize(fs_users);
                    yy = (int)bf.Deserialize(fs_users);
                    mm = (int)bf.Deserialize(fs_users);
                    dd = (int)bf.Deserialize(fs_users);
                    users.Add(new User(login, password, new DateOnly(yy, mm, dd)));
                }
            }
            catch (Exception)
            {}
            fs_users.Close();

            //Lessons
            FileStream fs_data = new(data_path, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                lessonInfo = ((List<LessonInfo>)bf.Deserialize(fs_data));
            }
            catch (Exception)
            {
                lessonInfo = new List<LessonInfo>();
            }
            fs_data.Close();
        }
        public void WriteUser(User user)
        {
            if (File.Exists(user_data_path) == false)
            {
                File.Create(user_data_path);
            }
            FileStream fs = new(user_data_path, FileMode.Append, FileAccess.Write);
            BinaryFormatter bf = new();
            bf.Serialize(fs, user.Login);
            bf.Serialize(fs, user.Password);
            bf.Serialize(fs, user.BirthDate.Year);
            bf.Serialize(fs, user.BirthDate.Month);
            bf.Serialize(fs, user.BirthDate.Day);
            fs.Close();
        }
        public void WriteUsers()
        {
            FileStream fs = new(user_data_path, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new();
            foreach(User user in users)
            {
                bf.Serialize(fs, user.Login);
                bf.Serialize(fs, user.Password);
                bf.Serialize(fs, user.BirthDate.Year);
                bf.Serialize(fs, user.BirthDate.Month);
                bf.Serialize(fs, user.BirthDate.Day);
            }
            fs.Close();
        }
        public void WriteData()
        {
            FileStream fs = new(data_path, FileMode.OpenOrCreate);
            BinaryFormatter bf = new();
            bf.Serialize(fs, lessonInfo);
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