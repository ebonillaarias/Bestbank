using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class BOAlternativeCommercialDto
    {
        /// <summary>
        /// El ID del comercial.
        /// </summary>
        [JsonPropertyName("order")]
        public int Order { get; set; }
        
        /// <summary>
        /// Comerciales alternativo.
        /// </summary>
        [JsonPropertyName("commercial")]
        public BOCommercialDto Commercial { get; set; }
   }
}
