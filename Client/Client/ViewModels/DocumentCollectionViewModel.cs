using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Command;
using Client.Server;
using Microsoft.Win32;
using System.Windows;

namespace Client.ViewModels
{
    public class DocumentCollectionViewModel: CommonBase
    {
        public delegate bool DeleteDocEventHandler(int id_doc);
        public delegate DocsInfo FindDocumentsEventHandler(string find_title, string find_publisher, string find_tags);
 
        #region DocumetCollection
       
        private ObservableCollection<DocumentViewModel> _documents = new ObservableCollection<DocumentViewModel>();
        public ObservableCollection<DocumentViewModel> Documents
        {
            get
            {
                if (_documents == null)
                {
                    _documents = new ObservableCollection<DocumentViewModel>();
                }
                return _documents;
            }
        }

        private bool _is_read_only = true;
        public bool IsReadOnly
        {
            get { return _is_read_only; }
        }
        #endregion


        public DocumentCollectionViewModel(string role=null)
        {
            _is_read_only = !(role == "administrator" || role == "editor");
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
            FindTitle = FindTags = FindPublisher = string.Empty;
            Documents.Clear();
        }
        private bool CanClear()
        {
            if ((Documents.Count != 0) || !string.IsNullOrEmpty(_find_title) || !string.IsNullOrEmpty(_find_publisher) || !string.IsNullOrEmpty(_find_tags))
            { return true; }
            return false;
        }
        #endregion
        #region FindDocument
        private string _find_title = string.Empty;
        public string FindTitle
        {
            get { return _find_title; }
            set
            {
                _find_title = value;
                onPropertyChanged("FindTitle");
            }
        }
        private string _find_publisher = string.Empty;
        public string FindPublisher
        {
            get { return _find_publisher; }
            set
            {
                _find_publisher = value;
                onPropertyChanged("FindPublisher");
            }
        }
        private string _find_tags = string.Empty;
        public string FindTags
        {
            get { return _find_tags; }
            set
            {
                _find_tags = value;
                onPropertyChanged("FindTags");
            }
        }

        #region FindDocCommand
        public event FindDocumentsEventHandler FindDocumentsEvent;
        private DelegateCommand find_docCommand;
        public ICommand FindDocCommand
        {
            get
            {
                if (find_docCommand == null)
                {
                    find_docCommand = new DelegateCommand(FindDoc, CanFindDoc);
                }
                return find_docCommand;
            }
        }
        private bool CanFindDoc()
        {
            return !string.IsNullOrEmpty(_find_title) || !string.IsNullOrEmpty(_find_tags) || !string.IsNullOrEmpty(_find_publisher);
        }
        private void FindDoc()
        {
            Documents.Clear();
            DocsInfo finded_document = FindDocumentsEvent(FindTitle, FindPublisher, FindTags);
            if (finded_document != null)
            {
                foreach (DocInfo di in finded_document.Output)
                {
                    DocumentViewModel doc = new DocumentViewModel(di, IsReadOnly);
                    doc.DeleteDocEvent += this.DeleteDoc;
                    doc.DownloadDocEvent += this.DownloadDocEvent;
                    doc.SaveChangeDocEvent += this.SaveChangeDocEvent;
                    doc.GetDocInfo += this.GetDocInfo;
                    doc.DeleteDocFromListEvent += this.DeleteDocFromList;
                    //doc.UploadDocEvent += this.UploadDocEvent;
                    Documents.Add(doc);
                }
                //DocumentCollectionAddDocEvent();
            }
        }
        #endregion
        #endregion
        #region AddDocCommand
        private DelegateCommand add_docCommand;
        public ICommand AddDocCommand
        {
            get
            {
                if (add_docCommand == null)
                {
                    add_docCommand = new DelegateCommand(AddDocuments, CanAddDocuments);
                }
                return add_docCommand;
            }
        }

        public bool CanAddDocuments()
        {
            return !IsReadOnly;
        }
        private void AddDocuments()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "All files (*.*)|*.*";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                foreach (string fn in myDialog.FileNames.ToList())
                {
                    if (Documents.Where(n => n.PathToDoc == fn).SingleOrDefault() == null)
                    {
                        DocumentViewModel doc = new DocumentViewModel(fn, IsReadOnly);
                        doc.UploadDocEvent += this.UploadDocEvent;
                        doc.DeleteDocFromListEvent += this.DeleteDocFromList;
                        Documents.Add(doc); 
                    }
                }
                //DocumentCollectionAddDocEvent();
            }
        }
        #endregion

        #region UploadAllDocCommand
        private DelegateCommand upload_all_docCommand;
        public ICommand UploadAllDocCommand
        {
            get
            {
                if (upload_all_docCommand == null)
                {
                    upload_all_docCommand = new DelegateCommand(UploadAllDoc, CanUploadAllDoc);
                }
                return upload_all_docCommand;
            }
        }
        private bool CanUploadAllDoc()
        {
            int c = Documents.Where(n => n.UploadDocCommand.CanExecute(null)).Count();
            if (c == 0)
                return false;
            else return true;
        }
        private void UploadAllDoc()
        {
            IEnumerable<DocumentViewModel> docs = Documents.Where(n => n.UploadDocCommand.CanExecute(null));
            foreach (DocumentViewModel doc in docs)
            {
                doc.UploadDocCommand.Execute(null);
            }
        }
        #endregion
        #region DownloadAllDocCommand
        private DelegateCommand download_all_docCommand;
        public ICommand DownloadAllDocCommand
        {
            get
            {
                if (download_all_docCommand == null)
                {
                    download_all_docCommand = new DelegateCommand(DownloadAllDoc, CanDownloadAllDoc);
                }
                return download_all_docCommand;
            }
        }
        private bool CanDownloadAllDoc()
        {
            int c = Documents.Where(n => n.DownloadDocCommand.CanExecute(null)).Count();
            if (c == 0)
                return false;
            else return true;
        }
        private void DownloadAllDoc()
        {
            System.Windows.Forms.FolderBrowserDialog myDialog = new System.Windows.Forms.FolderBrowserDialog();
            myDialog.Description = "Select directory for downloaded documents";
            if (myDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IEnumerable<DocumentViewModel> docs = Documents.Where(n => n.DownloadDocCommand.CanExecute(null));
                foreach (DocumentViewModel doc in docs)
                {
                    doc.DownloadDocCommand.Execute(myDialog.SelectedPath + "/");
                }
            }
        }
        #endregion
        #region SaveAllDocCommand

        private DelegateCommand save_all_docCommand;
        public ICommand SaveAllDocCommand
        {
            get
            {
                if (save_all_docCommand == null)
                {
                    save_all_docCommand = new DelegateCommand(SaveAllDoc, CanDownloadAllDoc);
                }
                return save_all_docCommand;
            }
        }

        private void SaveAllDoc()
        {
            IEnumerable<DocumentViewModel> docs = Documents.Where(n => n.DownloadDocCommand.CanExecute(null));
            foreach (DocumentViewModel doc in docs)
            {
                doc.SaveDocCommand.Execute(null);
            }
        }
        #endregion
        #region DeleteAllDocCommand
        private DelegateCommand delete_all_docCommand;
        public ICommand DeleteAllDocCommand
        {
            get
            {
                if (delete_all_docCommand == null)
                {
                    delete_all_docCommand = new DelegateCommand(DeleteAllDoc, CanDownloadAllDoc);
                }
                return delete_all_docCommand;
            }
        }
        private void DeleteAllDoc()
        {
            if (MessageBox.Show("Are you sure you want to delete all finded documents?", "Delete documents", MessageBoxButton.YesNoCancel,
                  MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IEnumerable<DocumentViewModel> docs = Documents.Where(n => n.DownloadDocCommand.CanExecute(null));
                
                foreach (DocumentViewModel doc in docs)
                {
                    if (DeleteDocEvent(doc.ID_Doc))
                        delete_list.Add(doc);
                }
                foreach (DocumentViewModel doc in delete_list)
                {
                    Documents.Remove(doc);
                }
                delete_list.Clear();
            }
        }
        #endregion

        #region DocumentsCommand
        public event UploadDocEventHandler UploadDocEvent;
        public event DownloadDocEventHandler DownloadDocEvent;
        public event SaveChangeDocEventHandler SaveChangeDocEvent;
        public event DeleteDocEventHandler DeleteDocEvent;
        public event GetDocInfoEventHandler GetDocInfo;
        private List<DocumentViewModel> delete_list = new List<DocumentViewModel>();
        private void DeleteDoc(int id_doc)
        {
            if (DeleteDocEvent(id_doc))
            {
                Documents.Remove(Documents.Where(n => n.ID_Doc == id_doc).Single());
            }
        }
        private void DeleteDocFromList(DocumentViewModel doc)
        {
            delete_list.Add(doc);
        }
       // private bool DeleteDocFromli
        #endregion

        #region UpdateDocumentsInfoCommand

        private bool _is_updated;
        public bool IsUpdated
        {
            get { return _is_updated; }
            set
            {
                _is_updated = value;
                if (value)
                {
                    UpdateDocumentsInfoCommand.Execute(value);
                }
                onPropertyChanged("IsUpdated");
            }
        }
        private DelegateCommand update_documents_infoCommand;
        public ICommand UpdateDocumentsInfoCommand
        {
            get
            {
                if (update_documents_infoCommand == null)
                {
                    update_documents_infoCommand = new DelegateCommand(UpdateDocumentsInfo);
                }
                return update_documents_infoCommand;
            }
        }

        private void UpdateDocumentsInfo()
        {
            foreach (DocumentViewModel d in Documents)
            {
                d.UpdateDocInfoCommand.Execute(null);
            }
            foreach (DocumentViewModel d in delete_list)
            {
                Documents.Remove(d);
            }
            delete_list.Clear();
        }
        #endregion
    }
}
