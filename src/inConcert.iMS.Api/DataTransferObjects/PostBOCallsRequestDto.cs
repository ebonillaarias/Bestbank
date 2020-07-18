using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice Calls Request DTO.
   /// </summary>
   public class PostBOCallsRequestDto
   {
      /// <summary>
      /// Filtro: fecha de inicio.
      /// </summary>
      public string Start { get; set; }

      /// <summary>
      /// Filtro: fecha de fin.
      /// </summary>
      public string End { get; set; }

      /// <summary>
      /// Filtro: resultado de la llamada.
      /// </summary>
      public string Result { get; set; }

      /// <summary>
      /// Filtro: id del cliente de la llamada
      /// </summary>
      [JsonPropertyName("client")]
      public string ClientID { get; set; }

      /// <summary>
      /// Filtro: Lista con ID de comerciales
      /// </summary>
      public List<int> Commercials { get; set; }
   }
}
