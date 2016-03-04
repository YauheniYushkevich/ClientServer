using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Server.Model
{
    [DataContract]
    public class DocHistory
    {
        [DataMember]
        public DocInfo Doc { get; set; }
        [DataMember]
        public List<RowDocHistory> rdHistory { get; set; }

        public DocHistory()
        { rdHistory = new List<RowDocHistory>(); }
    }
    [DataContract]
    public class RowDocHistory
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
    }
}