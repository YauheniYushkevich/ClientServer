using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Server;

namespace Client.ViewModels
{
    public class UserHistoryViewModel : CommonBase
    {
        RowUserHistory _history = new RowUserHistory();

        public string Title
        {
            get { return _history.Doc_title; }
            //set
            //{
            //    _history.Doc_title = value;
            //    onPropertyChanged("Title");
            //}
        }
        public DateTime Date
        {
            get { return _history.Date; }
            //set
            //{
            //    _history.Date = value;
            //    onPropertyChanged("Date");
            //}
        }

        public UserHistoryViewModel(RowUserHistory _history)
        {
            this._history = _history;
        }
    }
}
