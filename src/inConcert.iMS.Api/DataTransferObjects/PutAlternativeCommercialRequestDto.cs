using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PutAlternativeCommercialRequestDto
    {
        /// <summary>
        /// Id del comercial alternativo.
        /// </summary>
        [JsonPropertyName("alternative_commercial_id")]
        public int AlternativeCommercialId { get; set; }
        /// <summary>
        /// Número de orden del comercial alternativo para el comercial principal.
        /// </summary>
        [JsonPropertyName("order")]
        public int Order { get; set; }
    }
}
