using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class GetCallsRecentsByCommercialDto
   {
      /// <summary>
      /// El numero del interlocutor.
      /// </summary>
      [JsonPropertyName("calledNumber")]
      public string CalledNumber { get; set; }

      [JsonPropertyName("direction")]
      public string Direction { get; set; }

      [JsonPropertyName("callResult")]
      public string CallPartResult { get; set; }

      [JsonPropertyName("dateTime")]
      public string DateTime { get; set; }

      /// <summary>
      /// customer que fue interlocutor
      /// </summary>
      [JsonPropertyName("customer")]
      public CustomerDto Customer { get; set; }
   }
}
