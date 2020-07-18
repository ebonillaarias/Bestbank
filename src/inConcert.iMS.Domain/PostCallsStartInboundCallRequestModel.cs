using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class PostCallsStartInboundCallRequestModel
   {
      /// <summary>
      /// Id. del que llama.
      /// </summary>
      public string CallerId { get; set; }
      /// <summary>
      /// Id del que es llamado.
      /// </summary>
      public string CalledId { get; set; }
   }
}
