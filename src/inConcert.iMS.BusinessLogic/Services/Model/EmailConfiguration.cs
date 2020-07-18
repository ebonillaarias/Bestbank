namespace inConcert.iMS.BusinessLogic.Services.Model
{
   public class EmailConfiguration
   {
      public EmailConfiguration()
      {
      }

      public EmailConfiguration(string fromEmail, string fromPassword, string fromName, string smtpAddress, int smtpPort, bool enableSsl, bool bodyAsHtml)
      {
         this.FromEmail = fromEmail;
         this.FromPassword = fromPassword;
         this.FromName = fromName;
         this.SmtpAddress = smtpAddress;
         this.SmtpPort = smtpPort;
         this.EnableSsl = enableSsl;
         this.BodyAsHtml = bodyAsHtml;
      }

      public string FromEmail { get; set; }

      public string FromPassword { get; set; }

      public string FromName { get; set; }

      public string SmtpAddress { get; set; }

      public int SmtpPort { get; set; }

      public bool EnableSsl { get; set; }

      public bool BodyAsHtml { get; set; }
   }
}
