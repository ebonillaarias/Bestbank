using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetCallIdBySessionIdResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Id de la llamada
      /// </summary>
      public int CallId { get; set; }

      /// <summary>
      /// Nombre del canal SIP que originó la llamada.
      /// </summary>
      public string OrigChannel { get; set; }

      /// <summary>
      /// Nombre del canal SIP que debe redirigirse a otro peer.
      /// </summary>
      public string RedirectChannel { get; set; }
   }
}



