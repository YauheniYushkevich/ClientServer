using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Server;
using System.Windows.Input;
using Client.Command;
using System.Windows;
using System.Text.RegularExpressions;

namespace Client.ViewModels
{
    public class NewUserViewModel:CommonBase
    {
        public delegate void CreateCommandEventHandler(User user);
        public event CreateCommandEventHandler Create;
        #region User
        User _new_user = new User() {Role="subscriber"};

        public string Email
        {
            get { return _new_user.Email; }
            set
            {
                _new_user.Email = value;
                onPropertyChanged("Email");
            }
        }
        public string Name
        {
            get { return _new_user.Name; }
            set
            {
                if (value != null)
                {
                    if (Settings.RegForName.IsMatch(value))
                    {
                        throw new ArgumentException("Incorrect char");
                    }
                    else
                    {
                        _new_user.Name = value;
                        onPropertyChanged("Name");
                    }
                }
            }
        }
        public string Surname
        {
            get { return _new_user.Surname; }
            set
            {
                if (value != null)
                {
                    if (Settings.RegForName.IsMatch(value))
                    {
                        throw new ArgumentException("Incorrect char");
                    }
                    else
                    {
                        _new_user.Surname = value;
                        onPropertyChanged("Surname");
                    }
                }
            }
        }

        public string Password
        {
            get { return _new_user.Password; }
            set
            {
                if (value != null)
                {
                    if (Settings.RegForPassword.IsMatch(value))
                    {
                        throw new ArgumentException("Incorrect char");
                    }
                    else
                    {
                        _new_user.Password = value;
                        onPropertyChanged("Password");
                    }
                }
            }
        }

        public string _confirm_password = string.Empty;
        public string ConfirmPassword
        {
            get { return _confirm_password; }
            set
            {
                if (value != null)
                {
                    if (Settings.RegForPassword.IsMatch(value))
                    {
                        throw new ArgumentException("Incorrect char");
                    }
                    else
                    {
                        _confirm_password = value;
                        onPropertyChanged("ConfirmPassword");
                    }
                }
            }
        }
        #endregion
        
        #region CreateCommand
        
        private DelegateCommand createlCommand;
        public ICommand CreateCommand
        {
            get
            {
                if (createlCommand == null)
                {
                    createlCommand = new DelegateCommand(CreateNewUser, CanCreateNewUser);
                }
                return createlCommand;
            }
        }

        private void CreateNewUser()
        {
            Create(_new_user);
            Clear();
        }

        private bool CanCreateNewUser()
        {
            return !(string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(Password) 
                || Password != ConfirmPassword || Password.Length<Settings.MinLengthPassword);
        }

        #endregion
        
        public void Clear()
        {
            Email = Name = Surname = Password = ConfirmPassword = string.Empty;
        }
        

        
    }
}
