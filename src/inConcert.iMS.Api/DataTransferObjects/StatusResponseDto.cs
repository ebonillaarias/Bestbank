using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class StatusResponseDto
   {
      /// <summary>
      /// Estado del mensaje.
      /// </summary>
      public string Status { get; set; }

      /// <summary>
      /// Texto del mensaje.
      /// </summary>
      public string Message { get; set; }
   }
}
