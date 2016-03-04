using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Command;
using System.Windows.Input;
using System.Windows;
using Client.Server;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace Client.ViewModels
{
    public delegate void ExceptionEventHandler(Exception ex);
    class MainViewModel : CommonBase
    {

        public event ExceptionEventHandler ExceptionEvent;
        #region Account
        private static ServiceClient _client = new ServiceClient();
        ServiceClient Client
        {
            get { return _client; }
            set
            {
                _client = value;
                Username = string.Empty;
                Password = string.Empty;
            }
        }

        private bool _is_login = false;
        public bool IsLogin
        {
            get { return _is_login; }
        }

        public string Username
        {
            get
            {
                if (_client == null)
                    _client = new ServiceClient();

                return _client.ClientCredentials.UserName.UserName;
            }
            set
            {
                if (!(_client != null && _client.State == System.ServiceModel.CommunicationState.Created))
                    _client = new ServiceClient();
                _client.ClientCredentials.UserName.UserName = value.ToLower();
                onPropertyChanged("Username");
            }
        }
        public string Password
        {
            get
            {
                if (_client == null)
                    _client = new ServiceClient();
                return _client.ClientCredentials.UserName.Password;
            }
            set
            {
                if (!(_client != null && _client.State == System.ServiceModel.CommunicationState.Created))
                    _client = new ServiceClient();
                _client.ClientCredentials.UserName.Password = value;
                onPropertyChanged("Password");
            }
        }

        
        #region LogInCommand
        private DelegateCommand loginCommand;
        public ICommand LogInCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new DelegateCommand(LogIn, CanLogIn);
                }
                return loginCommand;
            }
        }
        /// <summary>
        /// Функция определяет можно ли выполнять команду подключения(входа) к серверу
        /// </summary>
        /// <returns></returns>
        private bool CanLogIn()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || _is_login)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Команда подключения(входа) к серверу. Приуспешной операции входа, происходит начальная инициализация.
        /// Выполняется запрос на получение свойств пользователя(аккаунта), его истории. выполняется добавление обработчиков событий пользователя(аккаунта)
        /// коллекций документов и пользователей, а также заполнение коллекций языков, издателей и ролей. Свойтво IsLogin устанавливается в значение true
        /// Если подключение завершено ошибкой, выполняется функция LogOut комманды LogOutCommand
        /// </summary>
        private void LogIn()
        {
            try
            {
                Client.Open();
                ResultCommand result;
                result = _client.LogIn();//Username, Password);
                //Logs.Log = result;
                Logs.SetLog(result);
                if (result.IsOk)
                {
                    MyUserInit();

                    DocumentCollectionInit();

                    UserCollectionInit();

                    _languages = new ObservableCollection<string>(_client.GetAllLanguages().ouput);
                    onPropertyChanged("Languages");
                    _publishers = new ObservableCollection<string>(_client.GetAllPublishers().ouput);
                    onPropertyChanged("Publishers");
                    _roles = new ObservableCollection<string>(_client.GetAllRoles().ouput);
                    onPropertyChanged("Roles");

                    _is_login = true;
                    onPropertyChanged("IsLogin");
                }
                else
                {
                    LogOut();
                }
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                //LogOut();
                //MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }
        
        #endregion

        #region LogOutCommand
        private DelegateCommand logoutCommand;
        public ICommand LogOutCommand
        {
            get
            {
                if (logoutCommand == null)
                {
                    logoutCommand = new DelegateCommand(LogOut, CanLogOut);
                }
                return logoutCommand;
            }
        }
        
        private bool CanLogOut()
        {
            return IsLogin;
        }
        /// <summary>
        /// Команда выхода из аккаунта. Происходит очитска основных коллекций класса: Languages,Publishers, Roles, DocumentCollection, 
        /// UserCollection; а закрытие клиента. Свойтво IsLogin устанавливается в значение false.
        /// </summary>
        private void LogOut()
        {
            DocumentCollection.Clear();
            _document_collection = new DocumentCollectionViewModel();
            onPropertyChanged("DocumentCollection");

            UserCollection.Clear();
            _user_collection = new UserCollectionViewModel(true);
            onPropertyChanged("UserCollection");

            _my_user = new MyUserViewModel();
            onPropertyChanged("MyUser");

            NewUser.Clear();
            NewUser = new NewUserViewModel();
            NewUser.Create += this.CreateNewUser;

            if (Client.State != System.ServiceModel.CommunicationState.Faulted)
            { Client.Close(); }
            string username = Username;
            Client = new ServiceClient();
            Username = username;

            _is_login = false;
            onPropertyChanged("IsLogin");
        }

        #endregion
        #endregion

        #region Logs
        private static LogsViewModel _logs;
        public LogsViewModel Logs
        {
            get
            {
                if (_logs == null)
                {
                    _logs = new LogsViewModel();
                }
                return _logs;
            }
        }
        #endregion
       
        #region MyUser
        private MyUserViewModel _my_user;// = new MyUserViewModel();
        public MyUserViewModel MyUser
        {
            get
            {
                if (_my_user == null)
                {
                    _my_user = new MyUserViewModel();
                }
                return _my_user;
            }
        }

        private void MyUserInit()
        {
            _my_user = new MyUserViewModel(_client.GetMyHistory(/*Username*/));
            onPropertyChanged("MyUser");
            MyUser.SaveMyUserEvent += this.SaveMyUser;
            MyUser.DeleteMyUserEvent += this.DeleteMyUser;
            MyUser.DeleteMyUserEvent += this.LogOut;
            MyUser.UpdateMyUserEvent += this.GetMyUser;
            MyUser.MyUserNotFoundEvent += this.LogOut;
            MyUser.MyUserRoleChangeEvent += this.MyUserRoleChange;

        }
        private bool SaveMyUser(string new_name, string new_surname, string new_password)
        {
            try
            {
                ResultCommand result = _client.EditMyUser(/*Username,*/new_name, new_surname, new_password);
                Logs.SetLog(result);
                if (result.IsOk && !String.IsNullOrEmpty(new_password))
                {
                    string _un = Username;
                    string _pw = new_password;
                    LogOut();
                    _client = new ServiceClient();
                    Username = _un;
                    Password = _pw;
                    LogIn();
                }
                return result.IsOk;
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return false;
            }
        }
        private void DeleteMyUser()
        {
            try
            {
                ResultCommand result = _client.DeleteMyUser();//Username);
                //_logs.Log = result;
                Logs.SetLog(result);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
            }
        }
        private UserHistory GetMyUser()
        {
            try{
            return _client.GetMyHistory();//Username);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return null;
            }
        }
        private void MyUserRoleChange()
        {
            MessageBox.Show("Your role was changed", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            DocumentCollectionInit();
            UserCollectionInit();

        }
        #endregion

        #region NewUser
        private NewUserViewModel _new_user;
        public NewUserViewModel NewUser
        {
            get {
                if (_new_user == null)
                {
                    _new_user = new NewUserViewModel();
                }
                return _new_user; }
            set
            {
                _new_user = value;
                onPropertyChanged("NewUser");
            }
        }
        private void CreateNewUser(User user)
        {
            try{
            ResultCommand result = Client.CreateAccount(user);
            Logs.SetLog(result);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
            }
        }
        #endregion

        #region Documents

        private DocumentCollectionViewModel _document_collection;
        public DocumentCollectionViewModel DocumentCollection
        {
            get
            {
                if (_document_collection == null)
                {
                    _document_collection = new DocumentCollectionViewModel();
                }
                return _document_collection;
            }
        }

        private void DocumentCollectionInit()
        {
            _document_collection = new DocumentCollectionViewModel(MyUser.Role);
            onPropertyChanged("DocumentCollection");
            DocumentCollection.DeleteDocEvent += this.DeleteDoc;
            DocumentCollection.DownloadDocEvent += this.DownloadDoc;
            DocumentCollection.FindDocumentsEvent += this.FindDocuments;
            DocumentCollection.SaveChangeDocEvent += this.SaveDoc;
            DocumentCollection.UploadDocEvent += this.UploadDoc;
            DocumentCollection.GetDocInfo += this.GetDocInfo;
        }
        private void UploadDoc(DocInfo doc_info, Stream fs)
        {
            try
            {
                ResultCommand result = Client.UploadFile(doc_info, fs);
                Logs.SetLog(result);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
            }
        }
        private bool DownloadDoc(int ID_Doc, string downloaded_path)
        {
            try
            {
                FileStream fs;
                Stream out_fs;
                DocInfo result = _client.DownloadFile(ID_Doc,/* Username,*/ out out_fs);
                if (result != null)
                {
                    using (fs = new FileStream(downloaded_path, FileMode.Create, FileAccess.Write))
                    {
                        //read from the input stream in 65000 byte chunks
                        const int bufferLen = 65000;
                        byte[] buffer = new byte[bufferLen];
                        int count = 0;
                        while ((count = out_fs.Read(buffer, 0, bufferLen)) > 0)
                        {
                            // save to output stream
                            fs.Write(buffer, 0, count);
                        }
                        fs.Dispose();
                        out_fs.Dispose();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return false;
            }
        }
        private void SaveDoc(DocInfo doc_info)
        {
            try
            {
                ResultCommand result = _client.EditDocument(doc_info);
                //ResultCommand result = _client.EditDocument(doc_info);
                //Logs.Log = result;
                Logs.SetLog(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool DeleteDoc(int id_doc)
        {
            try{
            ResultCommand result = _client.DeleteDocument(id_doc);
            //Logs.Log = result;
            Logs.SetLog(result);
            //DocumentCollection.Documents.Remove(DocumentCollection.Documents.Where(n => n.ID_Doc == id_doc).Single());
            return result.IsOk;
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return false;
            }

        }
        private DocsInfo FindDocuments(string find_title, string find_publisher, string find_tags)
        {
            try
            {
                return _client.FindDocuments(find_title, find_publisher, find_tags);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return null;
            }

        }
        private DocInfo GetDocInfo(int id_doc)
        {
            try
            {
                return _client.UpdateDocInfo(id_doc);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return null;
            }
        }

        #endregion

        #region Users

        private UserCollectionViewModel _user_collection;
        public UserCollectionViewModel UserCollection
        {
            get
            {
                if (_user_collection == null)
                { 
                    _user_collection = new UserCollectionViewModel(true);
                }
                return _user_collection;
            }
        }

        private void UserCollectionInit()
        {
            _user_collection = new UserCollectionViewModel(MyUser.Role);
            onPropertyChanged("UserCollection");
            UserCollection.DeleteUserEvent += this.DeleteUser;
            UserCollection.GetUserHistoryEvent += this.GetHistory;
            UserCollection.SaveUserEvent += this.SaveUser;
            UserCollection.FindUserEvent += this.FindUser;
        }
        private bool SaveUser(User user)
        {
            try
            {
                ResultCommand result = Client.ChangeUserRole(user.Email, user.Role);
                //Logs.Log = result;
                Logs.SetLog(result);
                return result.IsOk;
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return false;
            }
        }
        private bool DeleteUser(User user)
        {
            try
            {
                ResultCommand result = Client.DeleteUser(user.Email);
                Logs.SetLog(result);
                return result.IsOk;
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return false;
            }
        }
        private UserHistory GetHistory(User user)
        {
            try{
                return Client.GetUserHistory(user.Email);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return null;
            }
        }
        private ListUsers FindUser(string find_email, string find_name, string find_surname, string find_role)
        {
            try{
            return Client.FindUsers(find_email, find_name, find_surname, find_role);
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
                return null;
            }
        }


        #endregion


        private ObservableCollection<string> _languages;
        public ObservableCollection<string> Languages
        {
            get
            {
                if (_languages == null)
                {
                    _languages = new ObservableCollection<string>();
                }
                return _languages;
            }
        }

        #region Publishers
        private ObservableCollection<string> _publishers;
        public ObservableCollection<string> Publishers
        {
            get
            {
                if (_publishers == null)
                {
                    _publishers = new ObservableCollection<string>();
                }
                if (IsLogin)
                {
                    _publishers = new ObservableCollection<string>(_client.GetAllPublishers().ouput);
                }
                return _publishers;
            }            
        }
        #region UpdatePublishersCommand
        private bool _is_updated;
        public bool IsUpdated
        {
            get { return _is_updated; }
            set
            {
                _is_updated = value;
                if (value && IsLogin)
                {
                    UpdatePublishersCommand.Execute(value);
                }
                onPropertyChanged("IsUpdated");
            }
        }
        private DelegateCommand update_publishersCommand;
        public ICommand UpdatePublishersCommand
        {
            get
            {
                if (update_publishersCommand == null)
                {
                    update_publishersCommand = new DelegateCommand(UpdatePublishers, CanUpdatePublishers);
                }
                return update_publishersCommand;
            }
        }

        private bool CanUpdatePublishers()
        {
            return IsLogin;
        }
        private void UpdatePublishers()
        {
            _publishers = new ObservableCollection<string>(_client.GetAllPublishers().ouput);
            onPropertyChanged("Publishers");
        }
        #endregion
        #endregion

        private ObservableCollection<string> _roles = new ObservableCollection<string>();
        public ObservableCollection<string> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new ObservableCollection<string>();
                }
                return _roles;
            }
        }

        public MainViewModel()
        {
            _is_login = false;
            onPropertyChanged("IsLogin");
            NewUser.Create += this.CreateNewUser;
            ExceptionEvent += this.ExceptionEventHandler;
        }

        private void ExceptionEventHandler(Exception ex)
        {
            Exception first_ex = ex;
            while (first_ex.InnerException != null)
            {
                first_ex = first_ex.InnerException;
            }
            Logs.SetLog(ex);
            LogOut();
        }

    }
}
