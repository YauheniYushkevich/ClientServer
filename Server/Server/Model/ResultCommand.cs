using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Server.Model
{
    //public enum CmdState : short
    //{
    //    Successfully = 0,
    //    Error
    //}

    //[DataContract]
    //public class CmdInfo
    //{
    //    [DataMember]
    //    public CmdState State { get; set; }
    //    [DataMember]
    //    public string Description { get; set; }
    //    [DataMember]
    //    public string Name { get; set; }
    //    //[DataMember]
    //    //public bool ImplementationCMD { get; set; }
    //    [DataMember]
    //    public string Message { get; set; }
    //    public CmdInfo(CmdState state, string name, string description, string message)
    //    {
    //        State = state;
    //        Name = name;
    //        Description = description;
    //        Message = message;
    //    }
    //}

    [DataContract]
    public class ResultCommand
    {
        
        [DataMember]
        public bool IsOk { get; set; }
        [DataMember]
        public string Message { get; set; }
        
        public ResultCommand(bool is_Ok, string message)
        {
            IsOk = is_Ok;
            Message = message;
        }
    }

    
}