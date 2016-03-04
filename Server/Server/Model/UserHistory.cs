using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Server.Model
{
    [DataContract]
    public class UserHistory
    {
        [DataMember]
        public User User { get; set; }
        [DataMember]
        public List<RowUserHistory> ruHistory { get; set; }

        public UserHistory()
        { ruHistory = new List<RowUserHistory>(); }
    }

    [DataContract] 
    public class RowUserHistory
    {
        [DataMember]
        public string Doc_title { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
    }
}