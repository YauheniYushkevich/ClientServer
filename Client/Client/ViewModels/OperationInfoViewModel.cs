using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ViewModels
{
    public class OperationInfoViewModel : CommonBase
    {
        private OperationStatus _status = OperationStatus.Nothing;
        public OperationStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                if (value == OperationStatus.Nothing)
                { OperationStatusValue = string.Empty; }
                else
                { OperationStatusValue = value.ToString(); }
                onPropertyChanged("OperationStatusValue");
            }
        }
        private string _operation_status_tooltip = string.Empty;
        public string OperationStatusToolTip
        {
            get { return _operation_status_tooltip; }
            set
            {
                _operation_status_tooltip = value;
                onPropertyChanged("OperationStatusToolTip");
            }
        }

        public string OperationStatusValue { get; set; }

        public OperationInfoViewModel(OperationStatus status, string tooltip = "")
        {
            Status = status;
            OperationStatusToolTip = tooltip;
        }
    }
    public enum OperationStatus : int
    {
        Nothing,
        Downloaded,
        ErrorDownloaded,
        Uploaded,
        ErrorUploaded,
        Saved,
        ErrorSaved,
        Deleted,
        ErrorDeleted
    }
}
