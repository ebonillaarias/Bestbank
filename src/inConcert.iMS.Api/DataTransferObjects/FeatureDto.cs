using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class FeatureDto
   {
      /// <summary>
      /// ID
      /// </summary>
      [JsonPropertyName("id")]
      public int Id { get; set; }

      /// <summary>
      /// Nombre
      /// </summary>
      [JsonPropertyName("name")]
      public string Name { get; set; }
   }
}
