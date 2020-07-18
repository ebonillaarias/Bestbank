using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice Supervisor DTO
   /// </summary>
   public class BOSupervisorDto
   {
      /// <summary>
      /// Id del supervisor.
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Nombre del supervisor.
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Email del supervisor.
      /// </summary>
      public string Email { get; set; }

      /// <summary>
      /// Estado del supervisor. Posibles valores: 0-Pendiente de aprobación, 1-Activo, 2-Superusuario
      /// </summary>
      public int State { get; set; }
   }
}
