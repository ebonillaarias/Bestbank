using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice Calls Request Model.
   /// </summary>
   public class PostBOCallsRequestModel
   {
      /// <summary>
      /// Fecha de inicio de la llamada.
      /// </summary>
      public string Start { get; set; }

      /// <summary>
      /// Fecha de fin de la llamada.
      /// </summary>
      public string End { get; set; }

      /// <summary>
      /// Resultado de la llamada.
      /// </summary>
      public string Result { get; set; }

      /// <summary>
      /// Id del cliente de la llamada
      /// </summary>
      public string ClientID { get; set; }

      /// <summary>
      /// Lista con ID de comerciales
      /// </summary>
      public List<int> Commercials { get; set; }
   }
}
