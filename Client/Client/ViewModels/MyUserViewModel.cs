using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Server;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Client.Command;
using System.Windows.Input;
using System.Windows;

namespace Client.ViewModels
{
    public class MyUserViewModel : CommonBase
    {
        public delegate bool SaveCommandEventHandler(string new_name, string new_surname, string new_password);
        public delegate void DeleteCommandEventHandler();
        public delegate UserHistory UpdateMyUserEventHandler();
        public delegate void MyUserNotFoundEventHandler();
        public delegate void MyUserRoleChangeEventHandler();
        private User EmptyUser()
        {
            return new User
            {
                Email = string.Empty,
                Name = string.Empty,
                Surname = string.Empty,
                Role = string.Empty,
                Password = string.Empty
            };
        }

        #region User
        private User _user = new User();
        public string Email
        {
            get
            {
                if (_user == null)
                {
                    _user = EmptyUser();
                }
                return _user.Email;
            }
        }
        public string Name
        {
            get
            {
                if (_user == null)
                {
                    _user = EmptyUser();
                }
                return _user.Name;
            }
        }
        public string Surname
        {
            get
            {
                if (_user == null)
                {
                    _user = EmptyUser();
                }
                return _user.Surname;
            }
        }
        public string Role
        {
            get
            {
                if (_user == null)
                {
                    _user = EmptyUser();
                }
                return _user.Role;
            }
        }

        #region EditUser

        private User _edit_user;

        public string EditName
        {
            get
            {
                if (_edit_user == null)
                {
                    _edit_user = EmptyUser();
                }
                return _edit_user.Name;
            }
            set
            {
                if (value == null)
                { _edit_user.Name = string.Empty; }

                if (Settings.RegForName.IsMatch(value))
                {
                    MessageBox.Show("Incorrect char", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Incorrect char");
                }
                else
                {
                    _edit_user.Name = value;
                    onPropertyChanged("EditName");
                }
            }
        }
        public string EditSurname
        {
            get
            {
                if (_edit_user == null)
                {
                    _edit_user = EmptyUser();
                }
                return _edit_user.Surname;
            }
            set
            {
                if (value == null)
                { _edit_user.Surname = string.Empty; }

                if (Settings.RegForName.IsMatch(value))
                {
                    MessageBox.Show("Incorrect char", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Incorrect char");
                }
                else
                {
                    _edit_user.Surname = value;
                    onPropertyChanged("EditSurname");
                }
            }
        }
        public string EditPassword
        {
            get
            {
                if (_edit_user == null)
                {
                    _edit_user = EmptyUser();
                }
                return _edit_user.Password;
            }
            set
            {
                if (value == null)
                {
                    _edit_user.Password = string.Empty;
                    onPropertyChanged("EditPassword");
                }

                if (Settings.RegForPassword.IsMatch(value))
                {
                    MessageBox.Show("Incorrect char", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Incorrect char");
                }

                _edit_user.Password = value;
                onPropertyChanged("EditPassword");

            }
        }
        private string _confirm_edit_password = string.Empty;
        public string ConfirmEditPassword
        {
            get { return _confirm_edit_password; }
            set
            {
                if (value == null)
                {
                    _confirm_edit_password = string.Empty;
                    onPropertyChanged("ConfirmEditPassword");
                }

                if (Settings.RegForPassword.IsMatch(value))
                {
                    MessageBox.Show("Incorrect char", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Incorrect char");
                }

                _confirm_edit_password = value;
                onPropertyChanged("ConfirmEditPassword");

            }
        }

        #endregion

        #endregion

        ObservableCollection<UserHistoryViewModel> _user_history;// = new ObservableCollection<UserHistoryViewModel>();
        public ObservableCollection<UserHistoryViewModel> UserHistory
        {
            get
            {
                if (_user_history == null)
                { _user_history = new ObservableCollection<UserHistoryViewModel>(); }
                return _user_history;
            }
            //set
            //{
            //    _user_history = value;
            //    onPropertyChanged("UserHistory");
            //}
        }

        public MyUserViewModel(UserHistory user_history = null)
        {
            FillMyUser(user_history);
        }
        private void FillMyUser(UserHistory user_history)
        {
            if (user_history == null)
            {
                _user = EmptyUser();
                onPropertyChanged("Email");
                onPropertyChanged("Name");
                onPropertyChanged("Surname");
                onPropertyChanged("Role");
                _edit_user = EmptyUser();
                onPropertyChanged("EditName");
                onPropertyChanged("EditSurname");
                onPropertyChanged("EditPassword");
                onPropertyChanged("ConfirmEditPassword");

                _user_history = new ObservableCollection<UserHistoryViewModel>();
            }
            else
            {                
                _user = new User
                {
                    Email = user_history.User.Email,
                    Name = user_history.User.Name,
                    Surname = user_history.User.Surname,
                    Role = user_history.User.Role
                };
                onPropertyChanged("Email");
                onPropertyChanged("Name");
                onPropertyChanged("Surname");
                onPropertyChanged("Role");
                _edit_user = new User
                {
                    Email = user_history.User.Email,
                    Name = user_history.User.Name,
                    Surname = user_history.User.Surname,
                    Role = user_history.User.Role,
                    Password= string.Empty
                };
                onPropertyChanged("EditName");
                onPropertyChanged("EditSurname");
                onPropertyChanged("EditPassword");
                onPropertyChanged("ConfirmEditPassword");

                _user_history = new ObservableCollection<UserHistoryViewModel>();
                if (user_history.ruHistory != null)
                {
                    foreach (RowUserHistory uh in user_history.ruHistory)
                    {
                        UserHistory.Add(new UserHistoryViewModel(uh));
                    }
                }
            }
        }

        #region SaveMyUserCommand
        public event SaveCommandEventHandler SaveMyUserEvent;
        private DelegateCommand save_my_userCommand;
        public ICommand SaveMyUserCommand
        {
            get
            {
                if (save_my_userCommand == null)
                {
                    save_my_userCommand = new DelegateCommand(SaveMyUser, CanSaveMyUser);
                }
                return save_my_userCommand;
            }
        }

        private void SaveMyUser()
        {
            if (SaveMyUserEvent(EditName, EditSurname, EditPassword))
            {
                _user.Name = EditName;
                _user.Surname = EditSurname;
            }
        }
        public bool CanSaveMyUser()
        {
            bool can_save_new_name = !(string.IsNullOrEmpty(EditName) || Settings.RegForName.IsMatch(EditName));
            bool can_save_new_surname = !(string.IsNullOrEmpty(EditSurname) || Settings.RegForName.IsMatch(EditSurname));
            bool can_save_new_password = (EditPassword == ConfirmEditPassword) &&
                (!Settings.RegForPassword.IsMatch(EditPassword) || string.IsNullOrEmpty(EditPassword));
            return can_save_new_name && can_save_new_surname && can_save_new_password && (EditName!=Name|| EditSurname != Surname||!string.IsNullOrEmpty(EditPassword));
        }
        #endregion

        #region DefaultMyUserCommand
        private DelegateCommand _default_my_userCommand;
        public ICommand DefaultMyUserCommand
        {
            get
            {
                if (_default_my_userCommand == null)
                {
                    _default_my_userCommand = new DelegateCommand(DefaultMyUser);
                }
                return _default_my_userCommand;
            }
        }
        private void DefaultMyUser()
        {
            EditPassword = string.Empty;
            ConfirmEditPassword = string.Empty;
            EditName = Name;
            EditSurname = Surname;
        }

        #endregion

        #region DeleteUserCommand

        public event DeleteCommandEventHandler DeleteMyUserEvent;

        public bool CanDisplayDeleteUser { get; set; }
        private DelegateCommand delete_userCommand;
        public ICommand DeleteUserCommand
        {
            get
            {
                if (delete_userCommand == null)
                {
                    delete_userCommand = new DelegateCommand(DeleteUser, CanDeleteUser);
                }
                return delete_userCommand;
            }
        }
        private void DeleteUser()
        {
            if (MessageBox.Show("Do you really want to delete your account?", "Delete my account", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DeleteMyUserEvent();
            }
        }
        private bool CanDeleteUser()
        {
            CanDisplayDeleteUser = (!string.IsNullOrEmpty(Email));
            onPropertyChanged("CanDisplayDeleteUser");
            return CanDisplayDeleteUser;
        }

        #endregion

        #region UpdateMyUserCommand
        private bool _is_updated;
        public bool IsUpdated
        {
            get { return _is_updated; }
            set
            {
                _is_updated = value;
                if (value)
                {
                    UpdateMyUserCommand.Execute(value);
                }
                onPropertyChanged("IsUpdated");
            }
        }
        public event UpdateMyUserEventHandler UpdateMyUserEvent;
        public event MyUserNotFoundEventHandler MyUserNotFoundEvent;
        public event MyUserRoleChangeEventHandler MyUserRoleChangeEvent;
        private DelegateCommand update_my_userCommand;
        public ICommand UpdateMyUserCommand
        {
            get
            {
                if (update_my_userCommand == null)
                {
                    update_my_userCommand = new DelegateCommand(UpdateMyUser);
                }
                return update_my_userCommand;
            }
        }

        private void UpdateMyUser()
        {
            if (!string.IsNullOrEmpty(_user.Email))
            {
                UserHistory result = UpdateMyUserEvent();

                if (result == null)
                {
                    MyUserNotFoundEvent();
                    FillMyUser(result);
                }
                else
                {
                    if (result.User.Role != _user.Role)
                    {
                        FillMyUser(result);
                        MyUserRoleChangeEvent();
                    }
                }
            }
        }
        #endregion
    }
}
