using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice Calls Response Model.
   /// </summary>
   public class PostBOCallsResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Listado con información de las llamadas.
      /// </summary>
      public List<BOCallModel> Calls { get; set; }
   }
}
