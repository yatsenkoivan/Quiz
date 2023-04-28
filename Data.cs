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
        Math, Biology, Geography, IT, Physics, History
    }
    enum Menus
    {
        Exit, Log_in, Register, password, login, Date_Of_Birth
    }
    class Quiz
    {
        private int cursor_pos;
        private Data data;
        static private void Show(Menus[] msg)
        {
            Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, Cursor.Cursor.offset_y);
            int current = Console.GetCursorPosition().Top;
            foreach(Menus m in msg)
            {
                Console.WriteLine(m);
                current++;
                Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, current);
            }
            Console.SetCursorPosition(Cursor.Cursor.offset_x, Cursor.Cursor.offset_y);
            Cursor.Cursor.Show();
        }
        public Quiz()
        {
            cursor_pos = 0;
            data = new Data();
        }
        private bool Procced(Menus m)
        {
            switch (m)
            {
                case Menus.Log_in:
                    loginPage();
                    break;
                case Menus.Register:
                    registerPage();
                    break;
                case Menus.login:

                    break;
                case Menus.password:

                    break;
                case Menus.Date_Of_Birth:

                    break;
                case Menus.Exit:
                    return false;
            }
            return true;
        }
        public void Menu()
        {
            Console.Clear();
            Menus[] msg = {
                Menus.Log_in,
                Menus.Register,
                Menus.Exit
            };
            int limit = msg.Length-1;
            Show(msg);
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (Procced(msg[move]) == false) return;
                    Console.Clear();
                    Show(msg);
                }
            } while (move == -1 || msg[move] != Menus.Exit);
            
        }
        public void loginPage()
        {
            Console.Clear();
            Menus[] msg = {
                Menus.login,
                Menus.password,
                Menus.Exit
            };
            string login = "";
            string password = "";
            int limit = msg.Length - 1;
            Show(msg);
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (Procced(msg[move]) == false) return;
                    Console.Clear();
                    Show(msg);
                }
            } while (move == -1 || msg[move] != Menus.Exit);
        }
        public void registerPage()
        {
            Console.Clear();
            Menus[] msg = {
                Menus.login,
                Menus.password,
                Menus.Date_Of_Birth,
                Menus.Exit
            };
            string login = "";
            string password = "";
            int limit = msg.Length - 1;
            Show(msg);
            int move;
            do
            {
                move = Cursor.Cursor.Move(limit);
                if (move != -1)
                {
                    if (Procced(msg[move]) == false) return;
                    Console.Clear();
                    Show(msg);
                }
            } while (move == -1 || msg[move] != Menus.Exit);
        }
    }
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
            
        }
    }
}