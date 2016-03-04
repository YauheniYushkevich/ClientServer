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
    public delegate bool SaveUserEventHandler(User user);
    public delegate bool DeleteUserEventHandler(User user);
    public delegate UserHistory GetUserHistoryEventHandler(User user);
    public delegate User GetUserEventHandler(string email);
    
    public class UserViewModel : CommonBase
    {
        #region User
        private User _user=new User();

        public string Email
        {
            get
            {
                if (_user == null)
                {
                    _user = new User();
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
                    _user = new User();
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
                    _user = new User();
                }
                return _user.Surname;
            }
        }
        public string Role
        {
            get {
                if (_user == null)
                {
                    _user = new User();
                }
                return _user.Role; }
        }

        private bool _is_read_only = true;
        public bool IsReadOnly
        {
            get { return _is_read_only; }
        }
        
        #region EditUser
        private User _edit_user = new User();
        public string EditRole
        {
            get {
                if (_edit_user == null)
                {
                    _edit_user = new User();
                }
                return _edit_user.Role; }
            set
            {
                if (value != Role && IsReadOnly)
                {
                    MessageBox.Show("You cann`t change user " + Name + " " + Surname + " role");
                    throw new ArgumentException("You cann`t change user " + Name + " " + Surname + " role");
                }
                else
                {
                    _edit_user.Role = value;
                }
                onPropertyChanged("EditRole");
            }
        }
        #endregion

        #endregion
        
        ObservableCollection<UserHistoryViewModel> _user_history;
        public ObservableCollection<UserHistoryViewModel> UserHistory
        {
            get
            {
                if (_user_history == null)
                {
                    _user_history = new ObservableCollection<UserHistoryViewModel>();
                }
                return _user_history;
            }
        }

        public UserViewModel(User user, bool is_read_only)
        {
            if (user != null)
            {
                Fill(user);

                _is_read_only = is_read_only;
                onPropertyChanged("IsReadOnly");
                
            }
        }
        private void Fill(User user)
        {
            _user.Email = user.Email;
            onPropertyChanged("Email");
            _user.Name = user.Name;
            onPropertyChanged("Name");
            _user.Surname = user.Surname;
            onPropertyChanged("Surname");
            _user.Role = user.Role;
            onPropertyChanged("Role");

            _edit_user.Email = user.Email;
            _edit_user.Name = user.Name;
            _edit_user.Surname = user.Surname;
            _edit_user.Role = user.Role;
            onPropertyChanged("EditRole");
        }
        
        #region SaveUserCommand
        public event SaveUserEventHandler SaveUserEvent;
        private DelegateCommand save_userCommand;
        public ICommand SaveUserCommand
        {
            get
            {
                if (save_userCommand == null)
                {
                    save_userCommand = new DelegateCommand(SaveUser, CanSaveUser);
                }
                return save_userCommand;
            }
        }

        private void SaveUser()
        {
            if (SaveUserEvent(_edit_user))
            {
                _user.Role = _edit_user.Role;
                onPropertyChanged("Role");
            }
            else
            {
                _edit_user.Role = _user.Role;
                onPropertyChanged("EditRole");
            }
        }

        private bool CanSaveUser()
        {
            return !((Role==EditRole) || IsReadOnly);
        }

        #endregion

        #region DeleteUserCommand
        public event DeleteUserEventHandler DeleteUserEvent;
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
            if (MessageBox.Show("Are you sure want to delete this user?", "Delete user", MessageBoxButton.YesNoCancel,
                  MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DeleteUserEvent(_user);
            }
        }
        private bool CanDeleteUser()
        {
            return !IsReadOnly;
        }
        #endregion

        #region UpdateUserInfoCommand
        public event GetUserEventHandler GetUser;
        public event GetUserHistoryEventHandler GetUserHistoryEvent;
        private DelegateCommand update_user_infoCommand;
        public ICommand UpdateUserInfoCommand
        {
            get
            {
                if (update_user_infoCommand == null)
                {
                    update_user_infoCommand = new DelegateCommand(UpdateUserInfo);
                }
                return update_user_infoCommand;
            }
        }

        private void UpdateUserInfo()
        {
            if (IsReadOnly)
            {
                Fill(GetUser(_user.Email));
            }
            else
            {
                UserHistory uh = GetUserHistoryEvent(_user);
                Fill(uh.User);
                UserHistory.Clear();
                foreach (RowUserHistory ruh in uh.ruHistory)
                {
                    UserHistory.Add(new UserHistoryViewModel(ruh));
                }
            }
        }
        #endregion
    }
}
