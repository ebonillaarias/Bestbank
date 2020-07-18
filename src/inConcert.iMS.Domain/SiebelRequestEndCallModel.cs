using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class SiebelRequestEndCallModel
   {
      /// <summary>
      /// Id del cliente.
      /// </summary>
      public string CustomerId { get; set; }

      /// <summary>
      /// Identifica el ID de la llamada.
      /// </summary>
      public string InteractionId { get; set; }

      /// <summary>
      /// Identifica la fecha de inicio de la llamada.
      /// Formato: MM/DD/YYYY HH24:MI:SS
      /// </summary>
      public string StartDate { get; set; }

      /// <summary>
      /// Identifica la fecha de fin de la llamada.
      /// Formato: MM/DD/YYYY HH24:MI:SS
      /// </summary>
      public string EndDate { get; set; }

      /// <summary>
      /// Resultado de la llamada.
      /// Puede ser: Rejected, Completed o NoAnswer.
      /// </summary>
      public string CallResult { get; set; }

      /// <summary>
      /// Identifica el motivo del rechazo de llamada.
      /// </summary>
      public string RejectionReason { get; set; }

        ///<summary>
        ///Identidica quien finalizo la llamada
        ///</summary>
        public string CallPartEndedBy { get; set; }

    }
}
