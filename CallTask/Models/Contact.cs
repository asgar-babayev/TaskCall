using System;
using System.Collections.Generic;
using System.Text;

namespace CallTask.Models
{
    public class Contact
    {
        private static int _id;
        private string _fullName;
        private string _number;
        public int Id { get; }
        public string Fullname
        {
            get { return _fullName; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value)) _fullName = value;
                else throw new Exception();
            }
        }
        public string Number
        {
            get { return _number; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value)) _number = value;
                else throw new Exception();
            }
        }

        static Contact()
        {
            _id = 0;
        }
        private Contact()
        {
            _id++;
            Id = _id;
        }

        public Contact(string fullname, string number) : this()
        {
            Fullname = fullname;
            Number = number;
        }
    }
}
