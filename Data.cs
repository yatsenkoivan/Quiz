﻿using System;
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
    enum Menus
    {
        Exit=0, Log_in, Register, password, login, Date_Of_Birth, submitLogin, submitRegister
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
        static private void Show(string title, string[] msg)
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
            Console.SetCursorPosition(Cursor.Cursor.offset_x, Cursor.Cursor.offset_y);
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
                    Show(title, msg);
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
                            UserMenu();
                            return;
                        case 3:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg);
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
                    Show(title,msg);
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
                if (key == (char)ConsoleKey.Backspace)
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
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            string[] msg = {
                "Play Quiz",
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
                    Show(title, msg);
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
                    Console.Clear();
                    Show(title, msg);
                }
            } while (move != msg.Length - 1);
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
                                data.UpdateUser(current_user, newUser);
                                MSG("  Info saved.");
                                Console.ReadKey(true);
                                return;
                            }
                            break;
                        case 3:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg);
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
    }
    class Question
    {
        private string text;
        private List<Answer> answers;
    }
    class Data
    {
        private List<User> users;
        private List<Question>[] questions;
        public List<User> Users
        {
            get { return users; }
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

            //QUESTIONS
            questions = new List<Question>[typeof(Lesson).GetEnumValues().Length];
            FileStream fs_data = new("data.bin", FileMode.OpenOrCreate, FileAccess.Read);

            /*
             * 
             * 
             * 
             * 
            */

            fs_data.Close();
        }
        private void WriteUser(User user)
        {
            FileStream fs = new("users.bin", FileMode.OpenOrCreate, FileAccess.Write);
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
        public void AddUser(User user)
        {
            users.Add(user);
            WriteUser(user);
        }
        public void UpdateUser(User user, User newUser)
        {
            
            int index = users.IndexOf(user);
            if (index != -1)
            {
                users[index] = newUser;
            }
            WriteUsers();
        }
    }
}