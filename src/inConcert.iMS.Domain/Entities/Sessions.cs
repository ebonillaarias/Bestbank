using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
   public class Sessions
   {
      /// <summary>
      /// Id de sesión.
      /// </summary>
      public int Id { get; set; }
      /// <summary>
      /// Id de sesión externa.
      /// </summary>
      public string IdExternal { get; set; }

      /// <summary>
      /// Id del comercial.
      /// </summary>
      public int CommercialId { get; set; }
      /// <summary>
      /// Comercial correspondiente a la sesión.
      /// </summary>
      public Commercials Comercial { get; set; }

      /// <summary>
      /// Fecha y hora UTC de inicio de sesión.
      /// </summary>
      public DateTimeOffset StartDate { get; set; }
      
      /// <summary>
      /// Fecha y hora UTC de ultima vez que se invocó KeepAlive.
      /// </summary>
      public DateTimeOffset? LastKeepAlive { get; set; }

      /// <summary>
      /// Fecha y hora UTC de fin de sesión.
      /// </summary>
      public DateTimeOffset? EndDate { get; set; }

      /// <summary>
      /// Token usado para enviar notificaciones por Firebase.
      /// </summary>
      public string FirebaseToken { get; set; }
   }
}
