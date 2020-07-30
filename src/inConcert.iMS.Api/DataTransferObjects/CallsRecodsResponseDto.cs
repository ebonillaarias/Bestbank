using System;
using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class CallsRecodsResponseDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Indica si se pone en pause/play la grabación
        /// </summary>
        [JsonPropertyName("record")]
        public bool Record { get; set; }

        /// <summary>
        /// Identificador único de llamada.
        /// </summary>
        [JsonPropertyName("callid")]
        public int CallId { get; set; }

        /// <summary>
        /// Número secuencial de cada parte de una llamada, comenzando en 1.
        /// </summary>
        [JsonPropertyName("callpartnumber")]
        public string CallPartNumber { get; set; }

        [JsonPropertyName("stardate")]
        public DateTime StartDate { get; set; }
    }
}
