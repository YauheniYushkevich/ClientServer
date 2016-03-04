using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace Server.Model
{
    [MessageContract]
    public class DownloadRequest
    {
        [MessageBodyMember]
        public int ID_Doc { get; set; }
        //[MessageBodyMember]
        //public string UserName { get; set; }
    }

    [MessageContract]
    public class RemoteDocumentInfo : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public DocInfo DocInfo { get; set; }

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream { get; set; }

        public void Dispose()
        {
            if (FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }
    }
    [MessageContract]
    public class UploadResult
    {
        [MessageBodyMember]
        public ResultCommand Result { get; set; }
        //public UploadCmdInfo(CmdState state, string message)
        //{ cmd_info = new CmdInfo(state, "UploadFile", "Upload the document to the service", message); }

    }
}