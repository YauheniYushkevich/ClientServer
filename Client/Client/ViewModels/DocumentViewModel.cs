using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using Client.Server;
using System.Windows.Media.Imaging;
using Client.Command;
using System.Windows.Input;

namespace Client.ViewModels
{
    public delegate void UploadDocEventHandler(DocInfo doc_info, Stream fs);
    public delegate bool DownloadDocEventHandler(int id_doc, string downloaded_path);
    public delegate void SaveChangeDocEventHandler(DocInfo doc_info);
    public delegate void DeleteDocEventHandler(int id_doc);
    public delegate void DeleteDocFromListEventHandler(DocumentViewModel doc);

    public delegate DocInfo GetDocInfoEventHandler(int id_doc);

    public class DocumentViewModel : CommonBase
    {
        #region DocInfo
        DocInfo _document;
        public DocInfo EmptyDoc()
        {
            return new DocInfo
            {
                ID_Doc = 0,
                Title = string.Empty,
                Description = string.Empty,
                Language = string.Empty,
                Publisher = string.Empty,
                Tags = string.Empty,
                Screen = null,
                FileLength = 0,
                FileName = string.Empty
            };
        }
       
        public int ID_Doc
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.ID_Doc; }
        }

        public string Title
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.Title;
            }
            set
            {
                if (!IsReadOnly)
                {
                    _document.Title = value;
                    onPropertyChanged("Title");
                }
            }
        }
        public string Description
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.Description;
            }
            set
            {
                if (!IsReadOnly)
                {
                    _document.Description = value;
                    onPropertyChanged("Description");
                }
            }
        }
        public string Language
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.Language;
            }
            set
            {
                if (!IsReadOnly)
                {
                    _document.Language = value;
                    onPropertyChanged("Language");
                }
            }
        }
        public string Publisher
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.Publisher;
            }
            set
            {
                if (!IsReadOnly)
                {
                    _document.Publisher = value;
                    onPropertyChanged("Publisher");
                }
            }
        }
        public string Tags
        {
            get
            {
                if (_document == null)
                {
                    _document = EmptyDoc();
                }
                return _document.Tags;
            }
            set
            {
                if (!IsReadOnly)
                {
                    IEnumerable<string> buf_value = value.Split('#', '@', ' ', ',').Where(n => n != string.Empty);
                    _document.Tags = string.Empty;

                    foreach (string v in buf_value)
                    { _document.Tags += '#' + v; }

                    onPropertyChanged("Tags");
                }
            }
        }
        //public byte[] Screen
        //{
        //    get
        //    {
        //        if (_document == null)
        //        {
        //            _document = EmptyDoc();
        //        }
        //        return _document.Screen;
        //    }
        //    set
        //    {
        //        if (!IsReadOnly)
        //        {
        //            _document.Screen = value;

        //            BitmapImage newImage = new BitmapImage();
        //            newImage.BeginInit();
        //            newImage.StreamSource = new MemoryStream(value);
        //            newImage.EndInit();
        //            _screen_image = newImage;

        //            //  onPropertyChanged("Screen");
        //            onPropertyChanged("ScreenImage");
        //        }
        //    }
        //}
        //private BitmapImage _screen_image = new BitmapImage();
        public BitmapImage ScreenImage
        {
            get
            {
                if (_document.Screen != null && _document.Screen.Count() > 0)
                {
                    BitmapImage newImage = new BitmapImage();
                    newImage.BeginInit();
                    newImage.StreamSource = new MemoryStream(_document.Screen);
                    newImage.EndInit();
                    return newImage;
                }
                return null;
            }
        }

        private bool _for_upload_doc = true;
        public bool ForUploadFile
        {
            get { return _for_upload_doc; }
        }
        #endregion
        private bool _is_read_only = true;
        public bool IsReadOnly
        {
            get { return _is_read_only; }
        }

        public DocumentViewModel(string path_to_doc, bool is_read_only)
        {
            if (is_read_only == false)
            {
                _document = EmptyDoc();
                PathToDoc = path_to_doc;

                _for_upload_doc = true;
                onPropertyChanged("ForUploadFile");

                _is_read_only = is_read_only;
                onPropertyChanged("IsReadOnly");
            }
        }
        public DocumentViewModel(DocInfo doc, bool is_read_only)
        {
            if (doc != null)
            {
                FillDoc(doc);
                //_document = doc;

                //if (Screen != null && Screen.Length > 0)
                //{
                //    Screen = doc.Screen;
                //}
                _for_upload_doc = false;
                onPropertyChanged("ForUploadFile");

                _is_read_only = is_read_only;
                onPropertyChanged("IsReadOnly");
                Downloaded = false;
            }
        }
        private void FillDoc(DocInfo doc)
        {
            _document = doc;
            onPropertyChanged("ID_Doc");
            onPropertyChanged("Title");
            onPropertyChanged("Description");
            onPropertyChanged("Language");
            onPropertyChanged("Publisher");
            onPropertyChanged("Tags");
            onPropertyChanged("ScreenImage");
        }

        #region PathToDoc

        private string _path_to_doc = string.Empty;
        public string PathToDoc
        {
            get { return _path_to_doc; }
            set
            {
                if (value == null)
                    _path_to_doc = string.Empty;
                else
                {
                    _path_to_doc = value;
                    if (ForUploadFile || Downloaded)
                    {
                        FileInfo fi = new FileInfo(_path_to_doc);
                        if (fi.Exists)
                        {
                            _document.FileLength = fi.Length;
                            _document.FileName = fi.Name;
                        }
                        else
                        {
                            throw new ArgumentException("File is not found");
                        }
                    }
                }
                onPropertyChanged("PathToDoc");
            }
        }

        private DelegateCommand path_to_docCommand;
        public ICommand PathToDocCommand
        {
            get
            {
                if (path_to_docCommand == null)
                {
                    path_to_docCommand = new DelegateCommand(AddPathToDocument, CanAddPathToDoc);
                }
                return path_to_docCommand;
            }
        }
        private bool CanAddPathToDoc()
        {
            return ForUploadFile;
        }

        private void AddPathToDocument()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog myDialog = new Microsoft.Win32.OpenFileDialog();
                myDialog.Filter = "All files (*.*)|*.*";
                myDialog.CheckFileExists = true;
                myDialog.Multiselect = false;
                if (myDialog.ShowDialog() == true)
                {
                    PathToDoc = myDialog.FileName;
                }
            }
            catch (Exception ex)
            { System.Windows.MessageBox.Show(ex.Message); }
        }

        #endregion

        #region PathToScreen

        private string _path_to_screen = string.Empty;
        public string PathToScreen
        {
            get { return _path_to_screen; }
            set
            {
                if (value == null)
                    _path_to_screen = string.Empty;
                else
                {
                    _path_to_screen = value;
                    if (ForUploadFile)
                    {
                        FileInfo fi = new FileInfo(_path_to_screen);
                        if (fi.Exists)
                        {
                            if (fi.Length < Settings.MaxSizeScreen + 1)
                            {
                                FileStream fs = new FileStream(_path_to_screen, FileMode.Open, FileAccess.Read);
                                byte[] buf = new byte[fi.Length];
                                fs.Read(buf, 0, (int)fi.Length);
                                _document.Screen = buf;
                                onPropertyChanged("ScreenImage");
                            }
                            else
                            { throw new ArgumentException("File length is more then 2Mb"); }
                        }
                        else
                        { throw new ArgumentException("File is not found"); }
                    }
                    else
                    {
                        if (!Directory.Exists(_path_to_screen))
                        { throw new ArgumentException("Directory is not found"); }
                    }
                }
                onPropertyChanged("PathToScreen");
            }
        }

        private DelegateCommand path_to_screenCommand;
        public ICommand PathToScreenCommand
        {
            get
            {
                if (path_to_screenCommand == null)
                {
                    path_to_screenCommand = new DelegateCommand(AddPathToScreen, CanAddPathToDoc);
                }
                return path_to_screenCommand;
            }
        }
        private void AddPathToScreen()
        {
            Microsoft.Win32.OpenFileDialog myDialog = new Microsoft.Win32.OpenFileDialog();
            myDialog.Filter = "Image (*.bmp)|*.bmp";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                try
                {
                    PathToScreen = myDialog.FileName;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region UploadDocCommand
       
        public event UploadDocEventHandler UploadDocEvent;
        private DelegateCommand upload_docCommand;
        public ICommand UploadDocCommand
        {
            get
            {
                if (upload_docCommand == null)
                {
                    upload_docCommand = new DelegateCommand(UploadDoc, CanUploadDoc);
                }
                return upload_docCommand;
            }
        }
        private bool CanUploadDoc()
        {
            return (!IsReadOnly && ForUploadFile && !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Language) &&
                !string.IsNullOrEmpty(Publisher) && !string.IsNullOrEmpty(PathToDoc));
        }
        private void UploadDoc()
        {
            try
            {
                FileStream fs = new FileStream(PathToDoc, FileMode.Open, FileAccess.Read);
                UploadDocEvent(_document, fs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region DownloadDocCommand

        public event DownloadDocEventHandler DownloadDocEvent;
        private bool Downloaded { get; set; }

        private DelegateCommand<string> download_docCommand;
        public ICommand DownloadDocCommand
        {
            get
            {
                if (download_docCommand == null)
                {
                    download_docCommand = new DelegateCommand<string>(DownloadDoc, CanDownloadDoc);
                }
                return download_docCommand;
            }
        }
        private bool CanDownloadDoc(string path)
        {
            return CanDownloadDoc();
        }
        private bool CanDownloadDoc()
        {
            return !ForUploadFile;
        }
        private void DownloadDoc(string path)
        {
            try
            {
                string downloaded_path;
                FileInfo doc_fi = new FileInfo(_document.FileName);
                Microsoft.Win32.SaveFileDialog myDialog = new Microsoft.Win32.SaveFileDialog();
                myDialog.AddExtension = true;
                myDialog.CheckPathExists = true;
                myDialog.DefaultExt = doc_fi.Extension;
                myDialog.OverwritePrompt = true;
                myDialog.Filter = doc_fi.Extension + " file|*" + doc_fi.Extension;
                myDialog.Title = "Save documents";

                if (string.IsNullOrEmpty(path))
                {
                    if (myDialog.ShowDialog() == true)
                    { downloaded_path = myDialog.FileName; }
                    else
                        return;
                }
                else
                {
                    downloaded_path = path + _document.FileName;
                    FileInfo fi = new FileInfo(downloaded_path);
                    int i = 1;
                    while (fi.Exists)
                    {
                        downloaded_path = path + doc_fi.Name.Remove(doc_fi.Name.LastIndexOf(doc_fi.Extension)) + "(" + (i) + ")" + doc_fi.Extension;
                        fi = new FileInfo(downloaded_path);
                        if (i == int.MaxValue)
                            return;
                        i++;
                    }
                }
                if (DownloadDocEvent(ID_Doc, downloaded_path))
                {
                    PathToDoc = downloaded_path;
                }
                else
                {
                    MessageBox.Show("Document wasn`t downloded", "Download document", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region SaveDocCommand
        public event SaveChangeDocEventHandler SaveChangeDocEvent;
        private DelegateCommand save_docCommand;
        public ICommand SaveDocCommand
        {
            get
            {
                if (save_docCommand == null)
                {
                    save_docCommand = new DelegateCommand(SaveDoc, CanDownloadDoc);
                }
                return save_docCommand;
            }
        }

        private void SaveDoc()
        {
            SaveChangeDocEvent(_document);
        }
        #endregion

        #region ViewDocCommand
        private DelegateCommand view_docCommand;
        public ICommand ViewDocCommand
        {
            get
            {
                if (view_docCommand == null)
                {
                    view_docCommand = new DelegateCommand(ViewDoc, CanDownloadDoc);
                }
                return view_docCommand;
            }
        }

        private void ViewDoc()
        {
            if (string.IsNullOrEmpty(_path_to_doc))
            {
                DownloadDoc(_path_to_doc);
            }
            if (!string.IsNullOrEmpty(_path_to_doc))
            {
                System.Diagnostics.Process.Start(_path_to_doc);
            }
        }
        #endregion

        #region DeleteDocCommand
        public event DeleteDocEventHandler DeleteDocEvent;
        private DelegateCommand delete_docCommand;
        public ICommand DeleteDocCommand
        {
            get
            {
                if (delete_docCommand == null)
                {
                    delete_docCommand = new DelegateCommand(DeleteDoc, CanDownloadDoc);
                        //new DelegateCommand(DeleteDoc, CanDeleteDoc);
                }
                return delete_docCommand;
            }
        }

        private void DeleteDoc()//bool ask_permision=true)
        {
            if (MessageBox.Show("Are you sure you want to delete the document?", "Delete document", MessageBoxButton.YesNoCancel,
              MessageBoxImage.Question) != MessageBoxResult.Yes)
            { return; }
            DeleteDocEvent(_document.ID_Doc);
        }
        
        //private void DeleteDoc()
        //{
        //    DeleteDoc(true);
        //}
        #endregion

        #region UpdateDocInfoCommand
        public event GetDocInfoEventHandler GetDocInfo;
        public event DeleteDocFromListEventHandler DeleteDocFromListEvent;
        private DelegateCommand update_doc_infoCommand;
        public ICommand UpdateDocInfoCommand
        {
            get
            {
                if (update_doc_infoCommand == null)
                {
                    update_doc_infoCommand = new DelegateCommand(UpdateDocInfo);
                }
                return update_doc_infoCommand;
            }
        }

        private void UpdateDocInfo()
        {
            if (ForUploadFile)
            {
                FileInfo fi = new FileInfo(PathToDoc);
                if (!fi.Exists)
                {
                    DeleteDocFromListEvent(this);
                }
            }
            else
            {
                DocInfo ud = GetDocInfo(_document.ID_Doc);
                if (ud != null)
                {
                    FillDoc(ud);
                }
                else
                {
                    DeleteDocFromListEvent(this);
                }
            }
        }
        #endregion

    }
}
