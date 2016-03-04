using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Server.WorkService
{
    public static class MailAgent
    {
        private static string SMTPSERVER = "smtp.gmail.com";
        private static int PORTNO = 587;
        private static string gmailServicePassword = "QW_4Nfkd_12ML.";
        private static string gmailServiceUserName = "mailservicetest0@gmail.com";

        public static bool SendEmail(string[] emailToAddress, string[] ccemailTo, string subject, string body, bool isBodyHtml)
        {
            if (emailToAddress == null || emailToAddress.Length == 0)
            {
                return false;// "Email To Address Empty";
            }

            List<string> tempFiles = new List<string>();

            SmtpClient smtpClient = new SmtpClient(SMTPSERVER, PORTNO);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(gmailServiceUserName, gmailServicePassword);
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(gmailServiceUserName);
                message.Subject = subject == null ? "" : subject;
                message.Body = body == null ? "" : body;
                message.IsBodyHtml = isBodyHtml;


                foreach (string email in emailToAddress)
                {
                    message.To.Add(email);
                }
                if (ccemailTo != null && ccemailTo.Length > 0)
                {
                    foreach (string emailCc in ccemailTo)
                    {
                        message.CC.Add(emailCc);
                    }
                }

                try
                {
                    smtpClient.Send(message);
                    return true;// "Email Send SuccessFully";
                }
                catch
                {
                    return false;// "Email Send failed";
                }
            }
        }
    }
}