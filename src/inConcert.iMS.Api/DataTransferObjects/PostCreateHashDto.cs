using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostCreateHash
   {
      /// <summary>
      /// ID
      /// </summary>
      [JsonPropertyName("data")]
      public string Data { get; set; }
   }
}
