using inConcert.iMS.Enums;
using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class PostCallsStartInboundCallResponseModel
   {
      /// <summary>
      /// Estado del mensaje.
      /// </summary>
      public ResultStatus Status { get; set; }
      
      /// <summary>
      /// Mensaje.
      /// </summary>
      public string Message { get; set; }


      /// <summary>
      /// Id de llamada dada de alta.
      /// </summary>
      public int CallId { get; set; }

      /// <summary>
      /// Lista de peers.
      /// </summary>
      public List<int> PeerList { get; set; }
   }
}
