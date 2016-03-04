using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Server.DB;

namespace Server.Model
{
    public enum UserPrintFormat : int
    {
        GetAll = 0,
        GetFullName,
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Role { get; set; }

        public User(string _email, string _name, string _surname, string _password, string _role)
        {
            Email = _email;
            Name = _name;
            Surname = _surname;
            Password = _password;
            Role = "subscriber";
        }
        public User(TUser user)
        {
            Email = user.Email;
            Name = user.Name;
            Surname = user.Surname;
            Password = string.Empty;
            Role = user.TRole.Role;
        }
        public override string ToString()
        {
            return Name + " " + Surname;
        }
        public string ToString(UserPrintFormat format)
        {
            switch (format)
            {
                case UserPrintFormat.GetAll:
                    return "Full name: " + Name + " " + Surname + ",\n" + "Role: " + Role + ",\n" + "E-mail: " + Email + ".";
                case UserPrintFormat.GetFullName:
                    return Name + " " + Surname;
                default:
                    return ToString();
            }
        }

    }

    [DataContract]
    public class ListUsers
    {
        [DataMember]
        public List<User> Output { get; set; }

        public ListUsers()
        {
            Output = new List<User>();
        }
    }
}