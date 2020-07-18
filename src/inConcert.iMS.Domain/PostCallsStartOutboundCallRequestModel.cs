using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class PostCallsStartOutboundCallRequestModel
   {
      /// <summary>
      /// Peer del que llama.
      /// </summary>
      public string Peer { get; set; }
      /// <summary>
      /// Id del que es llamado.
      /// </summary>
      public string CalledId { get; set; }
   }
}
