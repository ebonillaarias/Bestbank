using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PostBOSupervisorRequestModel
   {
      /// <summary>
      /// Email del supervisor
      /// </summary>
      public string Email { get; set; }
      
      /// <summary>
      /// Contraseña asociada al supervisor.
      /// </summary>
      public string Password { get; set; }
      
      /// <summary>
      /// Nombre del supervisor.
      /// </summary>
      public string Name { get; set; }
   }
}
