using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PostLogRequestDto
    {
        /// <summary>
        /// typelog
        /// </summary>
        [JsonPropertyName("TypeLog")]
        public string TypeLog { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// userid
        /// </summary>
        [JsonPropertyName("UserId")]
        public int UserId { get; set; }

        /// <summary>
        /// callid
        /// </summary>
        [JsonPropertyName("CallsId")]
        public int CallsId { get; set; }
    }
}
