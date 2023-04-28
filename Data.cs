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
        Math, Biology, Geography, IT, Physics, History
    }
    enum Menus
    {
        Exit=0, Log_in, Register, password, login, Date_Of_Birth, submitLogin, submitRegister
    }
    class Quiz
    {
        static private int tab_size = 20;
        static private string[] MenusTitle = { "Exit", "Log in", "Register", "Password: ", "Login: ", "Date of birth: " };
        private int cursor_pos;
        private Data data;
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
            cursor_pos = 0;
            data = new Data();
        }
        public void Menu()
        {
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
                            loginPage();
                            break;
                        case 1:
                            registerPage();
                            break;
                        case 2:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg);
                }
            } while (move != msg.Length - 1);
        }
        public void loginPage()
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
                            enterValue(out login);
                            break;
                        case 1:
                            enterValue(out password);
                            break;
                        case 2:
                            return;
                    }
                    Console.Clear();
                    Show(title, msg);
                    ShowValue(login, 0);
                    ShowValue(password, 1);
                }
            } while (move != msg.Length - 1);
        }
        public void registerPage()
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
                            enterValue(out login);
                            break;
                        case 1:
                            enterValue(out password);
                            break;
                        case 2:
                            enterValue(out dd);
                            enterValue(out mm,4);
                            enterValue(out yy,8);
                            break;
                        case 3:
                            User user = new User(login, password, new DateOnly(yy,mm,dd));
                            data.AddUser(user);
                            return;
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
        public void enterValue(out string value, int offset=0)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            value = Console.ReadLine() ?? "";
            Console.SetCursorPosition(x, y);
        }
        public void enterValue(out int value, int offset=0)
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.SetCursorPosition(x + tab_size + offset, y);
            int.TryParse(Console.ReadLine(), out value);
            Console.SetCursorPosition(x, y);
        }
    }
    [Serializable]
    class User
    {
        private string login;
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
        private Answer[] Answers;
    }
    class Data
    {
        private User[] users;
        public Data()
        {
            if (File.Exists("users.bin") == false)
            {
                return;
            }
            if (File.Exists("data.bin") == false)
            {
                return;
            }
            FileStream fs = File.OpenRead("users.bin");
            BinaryReader br = new(fs, Encoding.Unicode);

            users = Array.Empty<User>();

            string login;
            string password;
            int yy=1, mm=1, dd=1;
            try
            {
                login = br.ReadString();
                password = br.ReadString();
                //yy = br.ReadUInt16();
                //mm = br.ReadUInt16();
                //dd = br.ReadUInt16();
                users = users.Append(new User(login, password, new DateOnly(yy, mm, dd))).ToArray();
            }
            catch (Exception)
            {}
            fs.Close();
        }
        public void AddUser(User user)
        {
            users = users.Append(user).ToArray();
            FileStream fs = new FileStream("users.bin", FileMode.Append, FileAccess.Write);
            BinaryWriter bw = new(fs, Encoding.Unicode);
            bw.Write(user.Login);
            bw.Write("\t");
            bw.Write(user.Password);
            bw.Write("\n");
            DateOnly dob = user.BirthDate;
            bw.Write($"{dob.Year} {dob.Month} {dob.Day}\n");
            fs.Close();
        }
    }
}