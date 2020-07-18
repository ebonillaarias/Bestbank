using System.Collections.Generic;
using System.IO;
using inConcert.iMS.Enums;
using inConcert.iMS.BusinessLogic.Properties;
using System;
using inConcert.iMS.Domain;

namespace inConcert.iMS.BusinessLogic.Services.Model
{
   public class EmailData : NotifierData
   {
      public EmailData(string to)
      {
         this.To = to;
      }

      public EmailData(string to, string subject, string body, IEnumerable<EmailAttachment> attachments)
          : this(to)
      {
         this.Subject = subject;
         this.Body = body;
         this.Attachments = attachments;
      }

      /// <summary>
      /// Consrtructor de EmailData.
      /// </summary>
      /// <param name="inEmailType">Enumerado indicando tipo de mail a crear.</param>
      /// <param name="inData">Objeto (segun el tipo de email) con los datos a usar en el asunto/cuerpo del mail.</param>
      public EmailData(EmailType inEmailType, object inData)
      {
         #region ImageHeaderAttachment
         string pathImageHeader = Path.Combine(Path.GetTempPath(), "BestBankEmailHeader.png");
         if (!File.Exists(pathImageHeader))
         {
            File.WriteAllBytes(pathImageHeader, Resources.BestBank_EmailHeader);
         }

         Guid imageHeaderId = Guid.NewGuid();         
         EmailAttachment attachmentHeader = new EmailAttachment
         {
            ContentId = imageHeaderId.ToString(),
            Inline = true,
            MediaType = new System.Net.Mime.ContentType("image/png"),
            Path = pathImageHeader,
         };
         #endregion

         switch (inEmailType)
         {
            case EmailType.CommercialNew:
               // Image AppLogin Attachment
               string pathImageAppLogin = Path.Combine(Path.GetTempPath(), "BestBankAppLogin.jpg");
               if (!File.Exists(pathImageAppLogin))
               {
                  File.WriteAllBytes(pathImageAppLogin, Resources.BestBank_AppLogin);
               }

               Guid imageAppLoginId = Guid.NewGuid();
               EmailAttachment attachmentAppLogin = new EmailAttachment
               {
                  ContentId = imageAppLoginId.ToString(),
                  Inline = true,
                  MediaType = new System.Net.Mime.ContentType("image/jpg"),
                  Path = pathImageAppLogin,
               };

               PostBOCommercialsResponseModel objCommercial = ((PostBOCommercialsResponseModel)inData);
               this.To = objCommercial.Email;
               this.Subject = "Registo na Contact APP do Best Bank";
               this.Attachments = new List<EmailAttachment> { attachmentHeader, attachmentAppLogin };

               this.Body = Resources.BestBank_EmailTemplate_CommercialNew
                  .Replace("{USER_EMAIL}", objCommercial.Email)
                  .Replace("{USER_NAME}", objCommercial.Name)
                  .Replace("{USER_PASSWORD}", objCommercial.Password)
                  .Replace("{USER_PEER}", objCommercial.Peer.ToString())
                  .Replace("{LINK_DOWNLOAD_APP}", objCommercial.LinkAppAndroid)
                  .Replace("{IMAGE_APPLOGIN_SRC}", $"cid:{imageAppLoginId.ToString()}")
                  .Replace("{IMAGE_SRC}", $"cid:{imageHeaderId.ToString()}");
               break;

            case EmailType.CommercialPasswordNew:
               ForgotPasswordResponseModel objForgotPass = ((ForgotPasswordResponseModel)inData);

               this.To = objForgotPass.CommercialEmail;
               this.Subject = "Alteração de Password da Contact App do Banco Best.";
               this.Attachments = new List<EmailAttachment> { attachmentHeader };

               this.Body = Resources.BestBank_EmailTemplate_CommercialNewPassword
                  .Replace("{USER_NAME}", objForgotPass.CommercialName)
                  .Replace("{USER_PASSWORD}", objForgotPass.CommercialPassword)
                  .Replace("{IMAGE_SRC}", $"cid:{imageHeaderId.ToString()}");
               break;

            case EmailType.SupervisorState:
               PatchBOSupervisorsRequestModel objSupervisor = ((PatchBOSupervisorsRequestModel)inData);

               this.To = objSupervisor.Email;
               this.Subject = "Supervisor de Aplicação";
               this.Attachments = new List<EmailAttachment> { attachmentHeader };

               this.Body = Resources.BestBank_EmailTemplate_SupervisorState
                  .Replace("{SUPERVISOR_STATE}", objSupervisor.Approve ? "aprovado" : "rejeitado")
                  .Replace("{IMAGE_SRC}", $"cid:{imageHeaderId.ToString()}");
               break;
         }
      }

      public string To { get; private set; }

      public string Subject { get; set; }

      public string Body { get; set; }

      public IEnumerable<EmailAttachment> Attachments { get; set; }
   }
}
