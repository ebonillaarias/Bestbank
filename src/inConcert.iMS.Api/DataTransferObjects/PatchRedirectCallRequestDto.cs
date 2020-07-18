using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PatchRedirectCallRequestDto
   {
      /// <summary>
      /// El ID de la sesion del comercial.
      /// </summary>
      public string SessionId { get; set; }
      /// <summary>
      /// Peer del comercial a redireccionar la llamada
      /// </summary>
      public int Peer { get; set; }
   }
}
