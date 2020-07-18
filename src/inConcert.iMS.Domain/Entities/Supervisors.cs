using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
   public class Supervisors
   {
      /// <summary>
      /// Id del supervisor.
      /// </summary>
      public int Id { get; set; }
      /// <summary>
      /// Dirección de correo electrónico del supervisor. Único.
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
      /// <summary>
      /// Estado del supervisor. Valores posibles: 0-Pendiente de aprobación, 1-Activo, 2-Superusuario
      /// </summary>
      public int State { get; set; }
      ///<summary>
      ///Referencia Tabla de logs
      ///</summary>
        //public LogsGenerals LogsGeneral { get; set; }
    }
}
