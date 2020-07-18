using System.Net.Mime;

namespace inConcert.iMS.BusinessLogic.Services.Model
{
   public class EmailAttachment
   {
      public string Path { get; set; }

      public ContentType MediaType { get; set; }

      public string ContentId { get; set; }

      public bool Inline { get; set; }
   }
}
