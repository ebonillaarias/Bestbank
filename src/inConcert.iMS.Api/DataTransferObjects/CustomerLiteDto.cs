using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class CustomerLiteDto
   {
      /// <summary>
      /// El ID del cliente.
      /// </summary>
      [JsonPropertyName("id")]
      public string Id { get; set; }
      
      /// <summary>
      /// El nombre del cliente.
      /// </summary>
      [JsonPropertyName("name")]
      public string Name { get; set; }
    }
}
