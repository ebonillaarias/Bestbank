using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostUpdatePasswordRequestDto
   {
      /// <summary>
      /// El ID del comercial.
      /// </summary>
      [JsonPropertyName("commercial_id")]
      public int CommercialId { get; set; }
      /// <summary>
      /// Contraseña actual del comercial.
      /// </summary>
      [JsonPropertyName("current_password")]
      public string Password { get; set; }
      /// <summary>
      /// Contraseña actual del comercial.
      /// </summary>
      [JsonPropertyName("new_password")]
      public string NewPassword { get; set; }
   }
}
