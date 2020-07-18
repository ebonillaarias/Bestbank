using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice Call DTO.
   /// </summary>
   public class PostCallsSetCustomerRequestDto
   {
      /// <summary>
      /// Id de sesión.
      /// </summary>
      [JsonPropertyName("session_id")]
      public string SessionId { get; set; }

      /// <summary>
      /// Id de cliente
      /// </summary>
      [JsonPropertyName("customer_id")]
      public string CustomerId { get; set; }

      /// <summary>
      /// Nombre de cliente
      /// </summary>
      [JsonPropertyName("customer_name")]
      public string CustomerName { get; set; }
   }
}
