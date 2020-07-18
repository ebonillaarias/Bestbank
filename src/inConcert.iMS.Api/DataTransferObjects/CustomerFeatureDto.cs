using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class CustomerFeatureDto
   {
      /// <summary>
      /// ID
      /// </summary>
      [JsonPropertyName("id")]
      public string Id { get; set; }

      /// <summary>
      /// Nombre
      /// </summary>
      [JsonPropertyName("name")]
      public string Name { get; set; }
   }
}
