using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Server.Model;
using System.Text.RegularExpressions;

namespace Server.WorkService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
        #region user management
        [OperationContract]
        ResultCommand LogIn();//string email, string password);

        [OperationContract]
        ResultCommand CreateAccount(User user);

        [OperationContract]
        ResultCommand DeleteMyUser();//string email);

        [OperationContract]
        ResultCommand DeleteUser(string email);

        [OperationContract]
        ResultCommand ChangeUserRole(string email, string role);

        [OperationContract]
        ResultCommand EditMyUser(/*string email, */string name = "", string surname = "", string password = "");

        [OperationContract]
        ListUsers FindUsers(string _email = null, string _name = null, string _surname = null, string _role = null);
        #endregion

        #region History
        [OperationContract]
        UserHistory GetMyHistory();//string email);

        [OperationContract]
        UserHistory GetUserHistory(string email);

        [OperationContract]
        DocHistory GetDocHistory(int id_document);
        #endregion

        #region document management
        [OperationContract]
        ResultCommand DeleteDocument(int id_doc);

        [OperationContract]
        DocInfo UpdateDocInfo(int id_doc);

        [OperationContract]
        ResultCommand EditDocument(DocInfo new_doc);

        [OperationContract]
        DocsInfo FindDocuments(string _title, string _publisher, string _tags);

        [OperationContract]
        RemoteDocumentInfo DownloadFile(DownloadRequest request);

        [OperationContract]
        UploadResult UploadFile(RemoteDocumentInfo document);

        #endregion

        #region Helper function
        [OperationContract]
        Str_list GetAllLanguages();

        [OperationContract]
        Str_list GetAllPublishers();

        [OperationContract]
        Str_list GetAllRoles();

        [OperationContract]
        Settings GetSettings();
        #endregion
    }
    
    public class Settings
    {
        private static Regex _valid_char_password = new Regex(@"[\W]");
        //private static int _max_size_file = 104857600;
       
        public static int MaxSizeFile { get { return 104857600; } }
     
        public static int MinLengthPassword { get { return 8; } }
    
        public static Regex ValidCharPassword { get { return _valid_char_password; } }

    }
    [DataContract]
    public class Str_list
    {
        [DataMember]
        public List<string> ouput { get; set; }
    }
}
