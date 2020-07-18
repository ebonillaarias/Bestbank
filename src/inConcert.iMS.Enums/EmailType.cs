using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Enums
{
   public enum EmailType
   {
      /// <summary>
      /// Email para alta de nuevo Comercial.
      /// </summary>
      CommercialNew,

      /// <summary>
      /// Email para olvido de contraseña de comercial.
      /// </summary>
      CommercialPasswordNew,

      /// <summary>
      /// Email para cuando se aprueba/rechaza solicitud nuevo supervisor.
      /// </summary>
      SupervisorState
   }
}
