using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Principal;

namespace Server.WorkService
{
    public class CustomRoleAuthorizationPolicy : IAuthorizationPolicy
    {
        private string id;
        private ClaimSet issuer;
        public CustomRoleAuthorizationPolicy()
        {
            id = Guid.NewGuid().ToString();
            issuer = ClaimSet.System;
        }

        public string Id { get { return id; } }
        public ClaimSet Issuer { get { return issuer; } }

        public bool Evaluate(EvaluationContext ec, ref object state)
        {

            DB.DB_ServiceDataContext db_context=new DB.DB_ServiceDataContext();
            if (!(ec.Properties.TryGetValue("Identities", out state)))
                return false;
            List<IIdentity> identities = state as List<IIdentity>;

            if (identities == null || identities.Count == 0)
                return false;

            foreach (IIdentity i in identities)
            {
                if (i.IsAuthenticated)
                {
                    if (i.Name=="guest")
                    { ec.Properties["Principal"] = new GenericPrincipal(i, new string[] { "amonimus" }); }
                    else
                    { ec.Properties["Principal"] = new GenericPrincipal(i, new string[] { db_context.TUsers.Where(n => n.Email == i.Name).Select(n => n.TRole.Role).SingleOrDefault() }); }
                }
            }
            
            return true;
        }
    }
}