using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PatchBOSupervisorsRequestModel
   {
      /// <summary>
      /// Id del supervisor.
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Email del supervisor.
      /// </summary>
      public string Email { get; set; }

      /// <summary>
      /// Booleano para indicar si de debe aprobar (true)
      /// el alta de supervisor o no (false).
      /// </summary>
      public bool Approve { get; set; }
   }
}
