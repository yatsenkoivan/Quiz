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

namespace Quiz
{
    enum Lesson
    {
        Math, Biology, Geography, IT, Physics, History
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