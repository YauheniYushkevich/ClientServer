using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Server;
using System.Windows.Media;
using System.IO;

namespace Client.ViewModels
{
    public class LogsViewModel : CommonBase
    {
        private const string _file_name = "Logs.txt";

        private string _text;
        public string Text
        {
            get
            {
                if (_text == null)
                {
                    _text = string.Empty;
                }
                return _text;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    _description = string.Empty;
                }
                return _description;
            }
        }

        private bool? _result = null;
        public bool? Result
        {
            get { return _result; }
        }

        public void SetLog(ResultCommand result)
        {
            FileStream fs_log = new FileStream(_file_name, FileMode.Append, FileAccess.Write);
            _text = "[" + DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss") + "]" + result.Message;
            onPropertyChanged("Text");
            //_description = "Function " + result.Name + ". " + result.State + ". " + result.Description + ".";
            //onPropertyChanged("Description");
            _result = result.IsOk;
            onPropertyChanged("Result");
            string message=_text+"\r\n"+_description+"\r\n";
            fs_log.Write(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetByteCount(message));
            //fs_log.Write(Encoding.UTF8.GetBytes(_description +'\n'), 0, Encoding.UTF8.GetByteCount(_description)+1);
            fs_log.Dispose();
        }

        public void SetLog(string operation, string description, bool? result = null)
        {
            _text = "[" + DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss") + "]" + operation;
            onPropertyChanged("Text");
            _description = description;
            onPropertyChanged("Description");
            _result = result;
            onPropertyChanged("Result");
        }
        public void SetLog(Exception ex)
        {
            Exception first_ex = ex;
            while (first_ex.InnerException != null)
            {
                first_ex = first_ex.InnerException;
            }
            ResultCommand result = new ResultCommand
            {
                Message=first_ex.Message,
                IsOk=false
            };
            //CmdInfo cmd = new CmdInfo();
            //{
            //    cmd.Name = first_ex.TargetSite != null ? first_ex.TargetSite.Name : string.Empty;
            //    cmd.State = CmdState.Error;
            //    cmd.Message = first_ex.Message;
            //    cmd.Description = first_ex.TargetSite != null ? first_ex.TargetSite.DeclaringType.FullName : string.Empty;
            //};
            SetLog(result);
        }

    }
}
