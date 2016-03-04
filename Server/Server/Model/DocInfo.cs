using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Server.DB;

namespace Server.Model
{
    [DataContract]
    public class DocInfo
    {
        [DataMember]
        public int ID_Doc { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public byte[] Screen { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string Publisher { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public long FileLength { get; set; }

        public DocInfo(TDocument td)
        {
            ID_Doc = td.ID_Document;
            Title = td.Title;
            Description = td.Description;
            Language = td.TLanguage.Language;
            Publisher = td.TPublisher.Publisher;

            if (td.Screen == null)
            { Screen = null; }
            else
            { Screen = td.Screen.ToArray(); }

            if (td.Tags == null)
            { Tags = null; }
            else
            { Tags = td.Tags.TrimEnd('#'); }
            FileName = td.FileName.Substring(td.ID_Document.ToString().Length);
            // FileLength = 0;// td.FileLength;
        }

    }

    [DataContract]
    public class DocsInfo
    {
        [DataMember]
        public List<DocInfo> Output;

        public DocsInfo()
        {
            Output = new List<DocInfo>();
        }
    }
}