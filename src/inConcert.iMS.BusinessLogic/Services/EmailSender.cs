using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.BusinessLogic.Services.Model;

namespace inConcert.iMS.BusinessLogic.Services
{
   public class EmailSender : INotifier
   {
      private readonly EmailConfiguration _configuration;

      public EmailSender(EmailConfiguration configuration)
      {
         this._configuration = configuration;
      }

      public bool Notify(NotifierData notifierConfiguration)
      {
         return this.Send(notifierConfiguration);
      }

      public Task<bool> NotifyAsync(NotifierData notifierConfiguration)
      {
         return Task.FromResult(this.Send(notifierConfiguration));
      }

      private bool Send(NotifierData notifierConfiguration)
      {
         EmailData data = notifierConfiguration as EmailData;
         if (data == null)
         {
            return false;
         }

         try
         {
            using (MailMessage mail = new MailMessage())
            {
               mail.From = new MailAddress(this._configuration.FromEmail, this._configuration.FromName ?? this._configuration.FromEmail);
               mail.To.Add(new MailAddress(data.To));
               mail.Subject = data.Subject;
               mail.IsBodyHtml = this._configuration.BodyAsHtml;

               AlternateView alternateView = AlternateView.CreateAlternateViewFromString(data.Body, null, MediaTypeNames.Text.Html);
               if (data.Attachments != null && data.Attachments.Count() > 0)
               {
                  foreach (EmailAttachment attachment in data.Attachments)
                  {
                     LinkedResource linkedResource = new LinkedResource(attachment.Path);
                     linkedResource.ContentId = attachment.ContentId;
                     linkedResource.ContentType = attachment.MediaType;
                     alternateView.LinkedResources.Add(linkedResource);

                     Attachment att = new Attachment(attachment.Path);
                     att.ContentDisposition.Inline = attachment.Inline;
                     mail.Attachments.Add(att);
                  }

               }

               mail.AlternateViews.Add(alternateView);

               using (SmtpClient client = new SmtpClient())
               {
                  client.Port = this._configuration.SmtpPort;
                  client.DeliveryMethod = SmtpDeliveryMethod.Network;
                  client.UseDefaultCredentials = false;
                  client.Host = this._configuration.SmtpAddress;
                  client.EnableSsl = this._configuration.EnableSsl;
                  client.Credentials = new NetworkCredential(this._configuration.FromEmail, this._configuration.FromPassword);

                  client.Send(mail);
               }
            }

            return true;
         }
         catch (Exception ex)
         {
            SmtpException smtpEx = ex as SmtpException;
            if (smtpEx != null)
            {
               // TODO: Log here something
            }

            // TODO: Log here something

            return false;
         }
      }
   }
}
