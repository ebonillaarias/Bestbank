using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice Call Details DTO.
   /// </summary>
   public class GetBOCallDetailsResponseDto
   {
      /// <summary>
      /// Id de la llamada.
      /// </summary>
      [JsonPropertyName("id")]
      public int Id { get; set; }

      /// <summary>
      /// Caller
      /// </summary>
      [JsonPropertyName("client")]
      public string CustomerName { get; set; }

      /// <summary>
      /// Fecha de Inicio de llamada en UTC
      /// </summary>
      [JsonPropertyName("start")]
      public string StartDate { get; set; }

      /// <summary>
      /// Fecha de fin de llamada en UTC
      /// </summary>
      [JsonPropertyName("end")]
      public string EndDate { get; set; }

      /// <summary>
      /// Duracion de la llamada
      /// </summary>
      [JsonPropertyName("duration")]
      public string Duration { get; set; }


      /// <summary>
      /// Direccion de la llamada (Inbound, Outbound)
      /// </summary>
      [JsonPropertyName("direction")]
      public string Direction { get; set; }

      /// <summary>
      /// Número telefonico que recibió la llamada
      /// </summary>
      [JsonPropertyName("interlocutor")]
      public string Interlocutor { get; set; }

      /// <summary>
      /// Resultado de la llamada (Rechazada, NoContestada, Completada, Transferida).
      /// </summary>
      [JsonPropertyName("result")]
      public string Result { get; set; }

      /// <summary>
      /// Nombre del comercial al cual fue tranferida la llamada.
      /// </summary>
      [JsonPropertyName("transferred")]
      public string Transferred { get; set; }

      public List<BOCallPartsDto> Parts { get; set; }
   }
}
