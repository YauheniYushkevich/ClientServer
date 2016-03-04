using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Server.DB;
using System.Text.RegularExpressions;
using Server.Model;
using System.Threading;
using System.IdentityModel.Selectors;
using System.IO;

namespace Server.WorkService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service : IService
    {
        DB_ServiceDataContext db_context = new DB_ServiceDataContext();
       // string DocDirectory = @"E:\DB_Documents\";
        DirectoryInfo DocDirectory = Directory.CreateDirectory(@"E:\DB_Documents\");

        #region requests
        private ResultCommand QDeleteUser(string email, bool author = false)
        {
            
            //string cmd_name;
            //if (author)
            //    cmd_name = "Delete my account";
            //else
            //    cmd_name = "Delete user account";

            //string cmd_description = "Delete user accoun email" + email;
            try
            {
                TUser user = db_context.TUsers.Where(n => n.Email == email).SingleOrDefault();
                if (user != null)
                {
                    if (!(user.TRole.Role == "administrator" && author == false))
                    {
                        db_context.THistories.DeleteAllOnSubmit(user.THistories);
                        db_context.TUsers.DeleteOnSubmit(user);
                        db_context.SubmitChanges();
                        return new ResultCommand(true, "User " + email + " successfully deleted");
                        //return new CmdInfo(CmdState.Successfully, cmd_name, cmd_description, "User account deleted successfully");
                    }
                    throw new ArgumentException("You can not delete user with role ofadministrator");
                    //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "You can not delete user with role ofadministrator");
                }
                throw new ArgumentException("User " + email + " is not found");
                
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "User is not found");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "User " + email + " has not deleted" + ex.Message);
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, ex.Message);
            }
        }

        private UserHistory QGetHistory(string email)
        {
            UserHistory result = null;
            try
            {
                TUser tu = db_context.TUsers.Where(n => n.Email == email).SingleOrDefault();
                if (tu != null)
                {
                    result = new UserHistory();
                    result.User = new User(tu);

                    foreach (THistory h in tu.THistories)
                    {
                        result.ruHistory.Add(new RowUserHistory { Doc_title = h.TDocument.Title, Date = h.Date });
                    }
                    return result;
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
        private DocHistory QGetHistory(int id_document)
        {
            DocHistory result = null;
            try
            {
                TDocument td = db_context.TDocuments.Where(n => n.ID_Document == id_document).SingleOrDefault();
                if (td != null)
                {
                    result = new DocHistory();
                    result.Doc = new DocInfo(td);
                    foreach (THistory h in td.THistories)
                    {
                        result.rdHistory.Add(new RowDocHistory { Email = h.Email, Date = h.Date });
                    }
                    return result;
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
        #endregion

        #region user management
        #region MyUser

        /// <summary>
        /// Account creation. E-mail notification of the registration of a new user (to administrators and this user)
        /// </summary>
        /// <param name="user">New user</param>
        /// <returns>Errors:
        ///                    Some fields are empty;
        ///                    Invalid length of the password or or password contains invalid characters
        ///                    Invalid email;
        ///                    User already exists;</returns>
        public ResultCommand CreateAccount(User user)
        {
            //string cmd_name = "Create account";
            //string cmd_description = "Account creation. E-mail notification of the registration of a new user (to administrators and this user)";
            
            //Checking empty fields
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Surname) || string.IsNullOrEmpty(user.Password))
            {
                throw new ArgumentException("Some fields are empty. Fill in the Email, Name, Surname and Password");
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "Some fields are empty. Fill in the Email, Name, Surname and Password"); 
            }

            //Cheking Length fields "Password", cheking invalid characters for password
            if (user.Password.Length < Settings.MinLengthPassword || Settings.ValidCharPassword.IsMatch(user.Password))
            {
                throw new ArgumentException("The length of the password is less than " + Settings.MinLengthPassword +
                    " or password contains invalid characters");
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "The length of the password is less than " + Settings.MinLengthPassword +
                //    " or password contains invalid characters");
            }

            try
            {
                //check if user already exist
                if (db_context.TUsers.Where(n => n.Email == user.Email).SingleOrDefault() == null)
                {
                    //send email to user. if email is not sent, generate error "incorrect email"
                    string[] mailto = { user.Email };
                    string body = user.ToString() + ", you are registered on our server. \n" + user.ToString(UserPrintFormat.GetAll);
                    if (MailAgent.SendEmail(mailto, null, "Registration Info", body, false))
                    {
                        TUser nUser = new TUser()
                        {
                            Email = user.Email,
                            Name = user.Name,
                            Surname = user.Surname,
                            Password = user.Password,
                            ID_Role = db_context.TRoles.Where(n => n.Role == "subscriber").Select(n => n.ID_Role).SingleOrDefault()
                        };
                        db_context.TUsers.InsertOnSubmit(nUser);
                        db_context.SubmitChanges();

                        //send email to all administrators
                        mailto = db_context.TUsers.Where(n => n.ID_Role == 2).Select(n => n.Email.TrimEnd(' ')).ToArray();
                        body = user.ToString() + ",\n  server registered a new user \n" + user.ToString(UserPrintFormat.GetAll);
                        MailAgent.SendEmail(mailto, null, "New User Info", body, false);
                        return new ResultCommand(true, "User " + user.ToString() + " successfully registered.");
                        //return new CmdInfo(CmdState.Successfully, cmd_name, cmd_description, "user " + user.ToString() + " successfully registered");
                    }
                    throw new ArgumentException("Invalid email: " + user.Email);
                    //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "Invalid email");
                }
                throw new ArgumentException("User with this Email already exists.");
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "User already exists");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "Account could not be created. " + ex.Message);
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, ex.Message);
            }
        }
        
        /// <summary>
        /// Authentication.
        /// </summary>
        /// <returns></returns>
        public ResultCommand LogIn()//string username, string password)
        {            
            try
            {
                TUser tu = db_context.TUsers.Where(n => n.Email == Thread.CurrentPrincipal.Identity.Name).Single();
                //return new CmdInfo(CmdState.Successfully, "Login", "", "Hi, " + tu.Name + " " + tu.Surname);
                //TUser tu = db_context.TUsers.Where(n => n.Email == username).SingleOrDefault();
                //if (tu == null || tu.Password != password)
                //{
                //    throw new ArgumentException("Incorrect Username or Password.");
                //    //return new CmdInfo(CmdState.Error, "Login", "", "Incorrect Username or Password"); 
                //}
                return new ResultCommand(true, "You are logged. Hi, " + tu.Name + " " + tu.Surname + ". Your role is " + tu.TRole.Role);
                //return new CmdInfo(CmdState.Successfully, "Login", "", "Hi, " + tu.Name + " " + tu.Surname);

            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "You are not logged." + ex.Message);
                //return new CmdInfo(CmdState.Error, "Login", "", ex.Message);
            }
        }

        /// <summary>
        /// Deleting your account.
        /// </summary>
        /// <returns></returns>
        public ResultCommand DeleteMyUser()//string username)
        {
            ///
            /// DeleteMyUser()
            /// 
            return QDeleteUser(Thread.CurrentPrincipal.Identity.Name, true);
            /// 
            //return QDeleteUser(username, true);

        }

        /// <summary>
        /// Edit your account.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultCommand EditMyUser(/*string username,*/ string name = "", string surname = "", string password = "")
        {
            try
            {
                //TUser user = db_context.TUsers.Where(n => n.Email == username).SingleOrDefault();
                TUser user = db_context.TUsers.Where(n => n.Email == Thread.CurrentPrincipal.Identity.Name).SingleOrDefault();
                                                                                                /// EditMyUser(string name = "", string surname = "", string password = "")
                bool is_changed = false;
                if (!string.IsNullOrEmpty(name))
                {
                    user.Name = name;
                    is_changed = true;
                }
                if (!string.IsNullOrEmpty(surname))
                {
                    user.Surname = surname;
                    is_changed = true;
                }
                if (!string.IsNullOrEmpty(password))
                {    
                    //Cheking Length fields "Password", cheking invalid characters for password
                    if (password.Length < Settings.MinLengthPassword || Settings.ValidCharPassword.IsMatch(password))
                    {
                        throw new ArgumentException("Password length is less than " + Settings.MinLengthPassword +
                            " or invalid characters are used.");
                        //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "Password length is less than " + Settings.MinLengthPassword +
                        //    " or invalid characters are used");
                    }
                    else
                    {
                        user.Password = password;
                        is_changed = true;
                    }
                }

                if (is_changed)
                {
                    db_context.SubmitChanges();
                    return new ResultCommand(true, "Your settings updated successfully.");
                    //return new CmdInfo(CmdState.Successfully, cmd_name, cmd_description, "Settings updated successfully");
                }
                throw new ArgumentException("All fields are empty.");
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "All fields are empty");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "Your settings wasn`t updated. "+ ex.Message);
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, ex.Message);
            }
        }
        #endregion

        #region Users
        /// <summary>
        /// Removing another account. This function is available only to users with the role "administrator".
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ResultCommand DeleteUser(string email)
        {
            return QDeleteUser(email);
        }

        /// <summary>
        /// Change the user's role. This function is available only to users with the role "administrator".
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public ResultCommand ChangeUserRole(string email, string role)
        {
            try
            {
                TUser user = db_context.TUsers.Where(n => n.Email == email).SingleOrDefault();
                TRole role_upd = db_context.TRoles.Where(n=>n.Role==role).SingleOrDefault();

                if (user != null)
                {
                    if (user.TRole.Role != "administrator")
                    {
                        if (role_upd != null)
                        {
                            user.TRole = role_upd;
                            //    role_upd.ID_Role;
                            db_context.SubmitChanges();
                            return new ResultCommand(true, "The role of the user " + email + " successfully changed.");
                            //return new CmdInfo(CmdState.Successfully, cmd_name, cmd_description, "The role of the user successfully changed");
                        }
                        throw new ArgumentException(role + " role does not exist.");
                        //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "This role does not exist");
                    }
                    throw new ArgumentException("You can not change the role of administrator.");
                    //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "You can not change the role of administrator");
                }
                throw new ArgumentException(email + " user does not exist.");
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, "This user does not exist");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "The user role can not be changed. " + ex.Message);
                //return new CmdInfo(CmdState.Error, cmd_name, cmd_description, ex.Message);
            }
        }

        /// <summary>
        /// Search users.
        /// </summary>
        /// <param name="_email"></param>
        /// <param name="_name"></param>
        /// <param name="_surname"></param>
        /// <param name="_role"></param>
        /// <returns></returns>
        public ListUsers FindUsers(string _email = null, string _name = null, string _surname = null, string _role = null)
        {
            if (string.IsNullOrEmpty(_email) && string.IsNullOrEmpty(_name) && string.IsNullOrEmpty(_surname) && string.IsNullOrEmpty(_role))
                return null;

            IQueryable<TUser> res_users = db_context.TUsers;

            if (!string.IsNullOrEmpty(_email))
            { res_users = res_users.Where(n => n.Email.Contains(_email)); }
            if (!string.IsNullOrEmpty(_name))
            { res_users = res_users.Where(n => n.Name.Contains(_name)); }
            if (!string.IsNullOrEmpty(_surname))
            { res_users = res_users.Where(n => n.Surname.Contains(_surname)); }
            if (!string.IsNullOrEmpty(_role))
            { res_users = res_users.Where(n => n.TRole.Role == _role); }

            ListUsers lu = new ListUsers();
            foreach (TUser tu in res_users)
            { lu.Output.Add(new User(tu)); }

            return lu;
        }
        #endregion
        #endregion

        #region History

        /// <summary>
        /// Getting your browsing history.
        /// </summary>
        /// <returns></returns>
        public UserHistory GetMyHistory()//string email)
        {
            ///
            /// GetMyHistory()
            /// 
            return QGetHistory(Thread.CurrentPrincipal.Identity.Name);
            //return QGetHistory(email);
        }

        /// <summary>
        /// Getting user browsing history. This function is available only to users with the role "administrator".
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserHistory GetUserHistory(string email)
        {
            return QGetHistory(email);
        }

        /// <summary>
        /// Getting users have viewed the document. This function is available only to users with the role "administrator".
        /// </summary>
        /// <param name="id_document"></param>
        /// <returns></returns>
        public DocHistory GetDocHistory(int id_document)
        {
            return QGetHistory(id_document);
        }
        #endregion

        #region document management
        public DocInfo UpdateDocInfo(int id_doc)
        {
            DocInfo result = null;
            TDocument document = db_context.TDocuments.Where(n => n.ID_Document == id_doc).SingleOrDefault();
            
            if (document != null)
            {
                result = new DocInfo(document);
            }
            return result;
        }
        /// <summary>
        /// Finded Document by Title , Publisher , Tags
        /// </summary>
        /// <param name="_title"></param>
        /// <param name="_publisher"></param>
        /// <param name="_tags"></param>
        /// <returns></returns>
        public DocsInfo FindDocuments(string _title, string _publisher, string _tags)
        {
            DocsInfo result = null;
            if (string.IsNullOrEmpty(_title) && string.IsNullOrEmpty(_publisher) && string.IsNullOrEmpty(_tags))
                return null;
            try
            {
                IQueryable<TDocument> documents = db_context.TDocuments;

                if (!string.IsNullOrEmpty(_title))
                    documents = documents.Where(n => n.Title.Contains(_title));

                if (!string.IsNullOrEmpty(_publisher))
                    documents = documents.Where(n => n.TPublisher.Publisher == _publisher);

                if (!string.IsNullOrEmpty(_tags))
                {
                    string[] tags = _tags.Split('#').Where(n => n != string.Empty).ToArray();
                    IQueryable<TDocument> res = documents.Where(n => n.Tags.Contains("#" + tags[0] + "#"));
                    for (int i = 1; i < tags.Length; i++)
                    {
                        res = res.Union(documents.Where(n => n.Tags.Contains("#" + tags[1] + "#")));
                    }
                    documents = res;
                }

                foreach (TDocument doc in documents)
                {
                    if (result == null)
                    {
                        result = new DocsInfo();
                    }
                    result.Output.Add(new DocInfo(doc));
                }

                return result;
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// Edit information about document
        /// </summary>
        /// <param name="new_doc"></param>
        /// <returns></returns>
        public ResultCommand EditDocument(DocInfo new_doc)
        {
            try
            {
                TDocument t_old_doc = db_context.TDocuments.Where(n => n.ID_Document == new_doc.ID_Doc).SingleOrDefault();

                if (t_old_doc == null)
                {
                    throw new ArgumentException("Document " + new_doc.Title + " not found.");
                }

                if (string.IsNullOrEmpty(new_doc.Title) || string.IsNullOrEmpty(new_doc.Language) || string.IsNullOrEmpty(new_doc.Publisher))
                    throw new ArgumentException("The document " + new_doc.Title + " contains a empty field. Complete field: Title, Language, Publisher.");

                //Проверка существует ли в БД данный язык(должен)
                TLanguage tl = db_context.TLanguages.Where(n => n.Language == new_doc.Language).SingleOrDefault();
                if (tl == null)
                    throw new ArgumentException(new_doc.Language + " language is not supported.");
                //Проверка существует ли в БД данный издатель, если издатель отсутствует, то добавляем его
                TPublisher tp = db_context.TPublishers.Where(n => n.Publisher == new_doc.Publisher).SingleOrDefault();
                if (tp == null)
                {
                    List<int> id_p = db_context.TPublishers.Select(n => n.ID_Publisher).ToList();
                    id_p.Sort();
                    int new_id_p = id_p.Count;
                    for (int i = 0; i < id_p.Count; i++)
                    {
                        if (id_p[i] != i)
                        {
                            new_id_p = i;
                            break;
                        }
                    }
                    tp = new TPublisher { ID_Publisher = new_id_p, Publisher = new_doc.Publisher };
                    db_context.TPublishers.InsertOnSubmit(tp);
                    db_context.SubmitChanges();
                }
                t_old_doc = db_context.TDocuments.Where(n => n.ID_Document == new_doc.ID_Doc).Single();
                //Edit(Save)
                t_old_doc.Title = new_doc.Title;
                t_old_doc.Description = new_doc.Description;
                t_old_doc.TLanguage = tl;
                t_old_doc.TPublisher = tp;
                t_old_doc.Tags = new_doc.Tags;


                db_context.SubmitChanges();
                return new ResultCommand(true, "Document" + new_doc.Title + " is successful edit.");
                //return new CmdInfo(CmdState.Successfully, "EditDocument", "", "Operation is successful");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "Document" + new_doc.Title + " is successful edit. " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_doc"></param>
        /// <returns></returns>
        public ResultCommand DeleteDocument(int id_doc)
        {
            try
            {
                TDocument t_del_doc = db_context.TDocuments.Where(n => n.ID_Document == id_doc).SingleOrDefault();
                if (t_del_doc == null)
                    throw new ArgumentException("Document not found.");

                //Delete file.
                FileInfo fi = new FileInfo(DocDirectory.FullName + t_del_doc.FileName);
                fi.Delete();

                //Delete from tables
                db_context.THistories.DeleteAllOnSubmit(t_del_doc.THistories);
                db_context.TDocuments.DeleteOnSubmit(t_del_doc);
                db_context.SubmitChanges();

                return new ResultCommand(true, "Document deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ResultCommand(false, "The document was not deleted. " + ex.Message);
            }
        }

        /// <summary>
        /// Download the document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RemoteDocumentInfo DownloadFile(DownloadRequest request)
        {
            RemoteDocumentInfo result = new RemoteDocumentInfo();
            try
            {
                DocInfo doc_info = new DocInfo(db_context.TDocuments.Where(n => n.ID_Document == request.ID_Doc).SingleOrDefault());
                if (doc_info == null)
                    throw new ArgumentException();
                string filePath = System.IO.Path.Combine(DocDirectory.FullName, doc_info.ID_Doc.ToString() + doc_info.FileName);

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);


                // check if exists
                if (!fileInfo.Exists)
                    throw new System.IO.FileNotFoundException();
                doc_info.FileLength = fileInfo.Length;
                // open stream
                System.IO.FileStream stream = new System.IO.FileStream(filePath,
                          System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // return result 
                result.DocInfo = doc_info;
                result.FileByteStream = stream;

                //Add to tHistories
                List<int> id_d = db_context.THistories.Select(n => n.ID_row).ToList();
                id_d.Sort();
                int new_id_h = id_d.Count + 1;

                for (int i = 0; i < id_d.Count; i++)
                {
                    if (id_d[i] != i)
                    {
                        new_id_h = i;
                        break;
                    }
                }
                THistory newHistory = new THistory
                {
                    ID_row = new_id_h,
                    Email = Thread.CurrentPrincipal.Identity.Name,
                    ID_Document = doc_info.ID_Doc,
                    Date = DateTime.Now
                };
                db_context.THistories.InsertOnSubmit(newHistory);
                db_context.SubmitChanges();
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Upload the document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UploadResult UploadFile(RemoteDocumentInfo request)
        {
            try
            {
                //Проверка на пустые строки
                bool empty_field = string.IsNullOrEmpty(request.DocInfo.Title) || string.IsNullOrEmpty(request.DocInfo.Language) || string.IsNullOrEmpty(request.DocInfo.Publisher) || string.IsNullOrEmpty(request.DocInfo.FileName);
                if (empty_field)
                {
                    throw new ArgumentException("The document contains a empty field. Complete field: Title, Language, Publisher, FileName.");
                }
                //Проверка существует ли в БД документ с таким же заголовком(не должно)
                TDocument td = (from c in db_context.TDocuments
                                where (c.Title == request.DocInfo.Title &&
                                    c.TLanguage.Language == request.DocInfo.Language &&
                                    c.TPublisher.Publisher == request.DocInfo.Publisher &&
                                    c.FileLength == request.DocInfo.FileLength)
                                select c).SingleOrDefault();
                //TDocument td = db_context.TDocuments.Where(n => n.Title == request.di.Title).Single();
                if (td != null)
                    throw new ArgumentException("Document with the given title already exists.");
                //Проверка существует ли в БД данный язык(должен)
                TLanguage tl = db_context.TLanguages.Where(n => n.Language == request.DocInfo.Language).SingleOrDefault();
                if (tl == null)
                    throw new ArgumentException(request.DocInfo.Language + " language is not supported.");
                //Проверка существует ли в БД данный издатель, если издатель отсутствует, то добавляем его
                TPublisher tp = db_context.TPublishers.Where(n => n.Publisher == request.DocInfo.Publisher).SingleOrDefault();
                if (tp == null)
                {
                    List<int> id_p = db_context.TPublishers.Select(n => n.ID_Publisher).ToList();
                    id_p.Sort();
                    int new_id_p = id_p.Count;
                    for (int i = 0; i < id_p.Count; i++)
                    {
                        if (id_p[i] != i)
                        {
                            new_id_p = i;
                            break;
                        }
                    }
                    tp = new TPublisher { ID_Publisher = new_id_p, Publisher = request.DocInfo.Publisher };
                    db_context.TPublishers.InsertOnSubmit(tp);
                    db_context.SubmitChanges();
                }
                if (request.DocInfo.FileLength > Settings.MaxSizeFile)
                {
                    throw new ArgumentException("Document have size more then " + Settings.MaxSizeFile + "bytes.");
                }
                //можно приступить к загрузке
                //Загрузка(прием) файла.

                //Определяем новый ID для документа
                List<int> id_d = db_context.TDocuments.Select(n => n.ID_Document).ToList();
                id_d.Sort();
                int new_id_d = id_d.Count + 1;

                for (int i = 0; i < id_d.Count; i++)
                {
                    if (id_d[i] != i)
                    {
                        new_id_d = i;
                        break;
                    }
                }
                //Добавляем новую запись о загруженом документе
                td = new TDocument
                {
                    ID_Document = new_id_d,
                    Title = request.DocInfo.Title,
                    Description = request.DocInfo.Description,
                    TLanguage = tl,
                    TPublisher = tp,
                    Screen = request.DocInfo.Screen,
                    Tags = request.DocInfo.Tags + "#",
                    FileName = new_id_d.ToString() + request.DocInfo.FileName,
                    FileLength = request.DocInfo.FileLength
                };

                //Загрузка
                FileStream targetStream = null;
                Stream sourceStream = request.FileByteStream;

                string filePath = DocDirectory.FullName + td.FileName;

                using (targetStream = new FileStream(filePath, FileMode.Create,
                                      FileAccess.Write, FileShare.None))
                {
                    //read from the input stream in 65000 byte chunks

                    const int bufferLen = 65000;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;
                    while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        // save to output stream
                        targetStream.Write(buffer, 0, count);
                    }
                    targetStream.Dispose();
                    sourceStream.Dispose();
                }

                db_context.TDocuments.InsertOnSubmit(td);
                db_context.SubmitChanges();

                return new UploadResult
                {
                    Result = new ResultCommand(true, "The document " + request.DocInfo.Title + " is successfully uploaded.")
                };
            }
            catch (Exception ex)
            {
                return new UploadResult
                {
                    Result = new ResultCommand(false, "The document can not uploaded. " + ex.Message)
                };
            }
        }
        #endregion

        #region Helper function
        /// <summary>
        /// Get a list of the all used languages 
        /// </summary>
        /// <returns></returns>
        public Str_list GetAllLanguages()
        {
            try
            {
                return new Str_list { ouput = db_context.TLanguages.Select(n => n.Language.TrimEnd(' ')).ToList() };
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Getting a list of publishers
        /// </summary>
        /// <returns></returns>
        public Str_list GetAllPublishers()
        {
            try
            {
                return new Str_list { ouput = db_context.TPublishers.Select(n => n.Publisher.TrimEnd(' ')).ToList() };
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Getting a list of user roles
        /// </summary>
        /// <returns></returns>
        public Str_list GetAllRoles()
        {
            try
            {
                return new Str_list { ouput = db_context.TRoles.Select(n => n.Role.TrimEnd(' ')).ToList() };
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Settings GetSettings()
        {
            return new Settings();
        }
        #endregion
        
    }
}
