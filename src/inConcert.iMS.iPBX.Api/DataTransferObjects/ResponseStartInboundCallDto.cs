using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
   public class ResponseStartInboundCallDto : StatusResponseDto
   {
      /// <summary>
      ///  Identificador único de llamada.
      /// </summary>
      public string CallId { get; set; }

      /// <summary>
      ///   Lista de Peers que deberían atender la llamada para llamadas entrantes.
      /// </summary>
      public string PeerList { get; set; }
   }
}
