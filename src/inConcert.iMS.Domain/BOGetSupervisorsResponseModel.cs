using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class BOGetSupervisorsResponseModel
   {
      /// <summary>
      /// Listado de Supervisores
      /// </summary>
      public List<BOSupervisorModel> Supervisors { get; set; }
      
      public ResultStatus Status { get; set; }
   }
}
