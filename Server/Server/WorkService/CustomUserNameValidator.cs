using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using Server.DB;


namespace Server.WorkService
{
    public class CustomUserNameValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (!(string.IsNullOrEmpty(userName)))
            {
                DB_ServiceDataContext db_context = new DB_ServiceDataContext();
                TUser tu = (from c in db_context.TUsers
                            where (c.Email == userName && c.Password == password)
                            select c).SingleOrDefault();
                if (tu == null)
                    if (userName != "guest")
                        throw new FaultException("Invalid login or password");
            }
            else
                throw new FaultException("Username can`t be empty");
        }
    }
}