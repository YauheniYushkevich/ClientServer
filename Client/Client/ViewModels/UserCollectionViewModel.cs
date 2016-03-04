using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Client.Command;
using System.Windows.Input;
using Client.Server;

namespace Client.ViewModels
{
    class UserCollectionViewModel:CommonBase
    {
        public delegate ListUsers FindUserEventHandler(string find_email, string find_name, string find_surname, string find_role);

        private ObservableCollection<UserViewModel> _users;
        public ObservableCollection<UserViewModel> Users
        {
            get {
                if (_users == null)
                {
                    _users = new ObservableCollection<UserViewModel>();
                }
                return _users; }
        }
        private bool _is_read_only = true;
        public bool IsReadOnly
        {
            get { return _is_read_only; }
        }

        public UserCollectionViewModel(bool is_read_only)
        {
            _is_read_only = is_read_only;
            onPropertyChanged("IsReadOnly");
        }

        public UserCollectionViewModel(string role)
        {
            _is_read_only = !(role == "administrator");
            onPropertyChanged("IsReadOnly");
        }

        #region ClearCommand
        private DelegateCommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new DelegateCommand(Clear, CanClear);
                }
                return clearCommand;
            }
        }
        public void Clear()
        {
            FindEmail = FindName = FindRole = FindSurname = string.Empty;
            Users.Clear();
        }
        private bool CanClear()
        {
            if ((Users.Count != 0) || !string.IsNullOrEmpty(FindEmail) || !string.IsNullOrEmpty(FindName) || !string.IsNullOrEmpty(FindSurname) || !string.IsNullOrEmpty(FindRole))
            { return true; }
            return false;
        }
        #endregion
        #region FindUser
        private string _find_email = string.Empty;
        public string FindEmail
        {
            get { return _find_email; }
            set
            {
                _find_email = value;
                onPropertyChanged("FindEmail");
            }
        }
        private string _find_name = string.Empty;
        public string FindName
        {
            get { return _find_name; }
            set
            {
                _find_name = value;
                onPropertyChanged("FindName");
            }
        }
        private string _find_surname = string.Empty;
        public string FindSurname
        {
            get { return _find_surname; }
            set
            {
                _find_surname = value;
                onPropertyChanged("FindSurname");
            }
        }
        private string _find_role = string.Empty;
        public string FindRole
        {
            get { return _find_role; }
            set
            {
                _find_role = value;
                onPropertyChanged("FindRole");
            }
        }

        #region ClearUserCommand
        private DelegateCommand clear_userCommand;
        public ICommand ClearUserCommand
        {
            get
            {
                if (clear_userCommand == null)
                {
                    clear_userCommand = new DelegateCommand(ClearUser, CanFindUser);
                }
                return clear_userCommand;
            }
        }
        private void ClearUser()
        {
            Users.Clear();
            FindEmail = FindName = FindSurname = FindRole = string.Empty;
        }
        #endregion
        #region FindUserCommand
        public event FindUserEventHandler FindUserEvent;
        private DelegateCommand find_userCommand;
        public ICommand FindUserCommand
        {
            get
            {
                if (find_userCommand == null)
                {
                    find_userCommand = new DelegateCommand(FindUser, CanFindUser);
                }
                return find_userCommand;
            }
        }
        private bool CanFindUser()
        {
            return !string.IsNullOrEmpty(_find_email) || !string.IsNullOrEmpty(_find_name) || !string.IsNullOrEmpty(_find_surname) || !string.IsNullOrEmpty(_find_role);
        }
        private void FindUser()
        {
            Users.Clear();
            ListUsers finded_user = FindUserEvent(FindEmail, FindName, FindSurname, FindRole);
            if (finded_user != null)
            {
                foreach (User fu in finded_user.Output)
                {
                    UserViewModel add_user = new UserViewModel(fu, IsReadOnly);
                    add_user.GetUserHistoryEvent += this.GetUserHistoryEvent;
                    add_user.GetUser += this.GetUser;
                    add_user.DeleteUserEvent += this.DeleteUser;
                    add_user.SaveUserEvent += this.SaveUserEvent;
                    if (!IsReadOnly)
                    { add_user.UpdateUserInfoCommand.Execute(true); }
                    Users.Add(add_user);
                }
            }
        }
        #endregion
        #endregion

        #region UpdateUsersInfoCommand
        private bool _is_updated;
        public bool IsUpdated
        {
            get { return _is_updated; }
            set
            {
                _is_updated = value;
                if (value)
                {
                    UpdateUsersInfoCommand.Execute(value);
                }
                onPropertyChanged("IsUpdated");
            }
        }

        private DelegateCommand update_users_infoCommand;
        public ICommand UpdateUsersInfoCommand
        {
            get
            {
                if (update_users_infoCommand == null)
                {
                    update_users_infoCommand = new DelegateCommand(UpdateUsersInfo);
                }
                return update_users_infoCommand;
            }
        }

        private void UpdateUsersInfo()
        {
            if (Users.Count != 0)
            {
                List<UserViewModel> delete_list = new List<UserViewModel>();
                foreach (UserViewModel user in Users)
                {
                    if (GetUser(user.Email) != null)
                    {
                        user.UpdateUserInfoCommand.Execute(true);
                    }
                    else
                    {
                        delete_list.Add(user);
                    }
                }
                foreach (UserViewModel user in delete_list)
                {
                    Users.Remove(user);
                }
            }
        }
        #endregion

        #region UserCommands
        public event SaveUserEventHandler SaveUserEvent;
        public event DeleteUserEventHandler DeleteUserEvent;
        public event GetUserHistoryEventHandler GetUserHistoryEvent;
        private User GetUser(string email)
        {
            ListUsers lu = FindUserEvent(email, "", "", "");
            return lu.Output.SingleOrDefault();
        }
        private bool DeleteUser(User user)
        {
            if (DeleteUserEvent(user))
            {
                Users.Remove(Users.Where(n => n.Email == user.Email).Single());
                return true;
            }
            return false;
        }
        #endregion
    }
}
