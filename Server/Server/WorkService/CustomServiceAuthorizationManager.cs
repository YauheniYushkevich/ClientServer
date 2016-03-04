using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Security.Principal;

namespace Server.WorkService
{
    public class CustomServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (operationContext.ServiceSecurityContext.IsAnonymous)
                return true;

            if (operationContext.ServiceSecurityContext.PrimaryIdentity.IsAuthenticated)
            {
                GenericPrincipal gp;
                gp = (GenericPrincipal)operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];
                string[] roles = operationContext.RequestContext.RequestMessage.Headers.Action.Split('/');
               // if(gp.IsInRole("administrator"))

                switch (roles[roles.Count() - 1])
                {
                    case "LogIn" :
                    case "EditMyUser":
                    case "DeleteMyUser":
                    case "FindUsers":
                    case "GetMyHistory":
                    case "GetAllLanguages":
                    case "GetAllPublishers":
                    case "GetAllRoles":
                    case "FindDocuments":
                    case "DownloadFile":
                    case "UpdateDocInfo":
                        if (!gp.IsInRole("anonimus"))
                            return true;
                        break;
                    case "CreateAccount":
                        if(gp.IsInRole("amonimus"))
                            return true;
                        break;
                    case "DeleteUser":
                    case "ChangeUserRole":
                    case "GetUserHistory":
                        if (gp.IsInRole("administrator"))
                            return true;
                        break;
                    case "GetDocHistory":
                    case "DeleteDocument":
                    case "UploadFile":
                    case "EditDocument":

                        if (gp.IsInRole("administrator") || gp.IsInRole("editor"))
                            return true;
                        break;
                    default:
                        return true;
                    //break;
                }
            }
            return false;
            /*
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;
            action.EndsWith("GetSecretCode");
            GenericPrincipal a = (GenericPrincipal)operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];
            //Thread.CurrentPrincipal.IsInRole();

            bool f = a.IsInRole("subscriber");
            // roles= operationContext.ServiceSecurityContext.PrimaryIdentity.
            Console.WriteLine("action: {0}", action);
            foreach (ClaimSet cs in operationContext.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                if (cs.Issuer == ClaimSet.System)
                {
                    foreach (Claim c in cs.FindClaims("http://example.com/claims/allowedoperation", Rights.PossessProperty))
                    {
                        Console.WriteLine("resource: {0}", c.Resource.ToString());
                        if (action == c.Resource.ToString())
                            return true;
                    }
                }
            }
            return false;*/
        }
    }
}