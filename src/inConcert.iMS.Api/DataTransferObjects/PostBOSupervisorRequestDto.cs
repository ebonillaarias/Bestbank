using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostBOSupervisorRequestDto
   {
      /// <summary>
      /// Nombre del supervisor.
      /// </summary>
      [JsonPropertyName("name")]
      public string Name { get; set; }
      
      /// <summary>
      /// Email del supervisor.
      /// </summary>
      [JsonPropertyName("email")]
      public string Email { get; set; }

      /// <summary>
      /// Password del supervisor.
      /// </summary>
      [JsonPropertyName("password")]
      public string Password { get; set; }
   }
}
