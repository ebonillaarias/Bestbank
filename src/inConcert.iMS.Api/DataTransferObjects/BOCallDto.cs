using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice Call DTO.
   /// </summary>
   public class BOCallDto
   {
      /// <summary>
      /// Id de la llamada.
      /// </summary>
      [JsonPropertyName("id")]
      public int Id { get; set; }

      /// <summary>
      /// Nombre del cliente
      /// </summary>
      [JsonPropertyName("client")]
      public string CustomerName { get; set; }

      /// <summary>
      /// Id del Cliente
      /// </summary>
      [JsonPropertyName("client_id")]
      public string CustomerId { get; set; }

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
      /// Resultado de la llamada (Rechazada, NoContestada, Completada, Transferida).
      /// </summary>
      [JsonPropertyName("result")]
      public string Result { get; set; }

      /// <summary>
      /// Nombre del comercial al cual fue tranferida la llamada.
      /// </summary>
      [JsonPropertyName("transferred")]
      public string Transferred { get; set; }

      /// <summary>
      /// Telefono del que llama.
      /// </summary>
      [JsonPropertyName("caller_id")]
      public string CallerId { get; set; }

      /// <summary>
      /// Telefono al que llaman.
      /// </summary>
      [JsonPropertyName("called_id")]
      public string CalledId { get; set; }
   }
}
