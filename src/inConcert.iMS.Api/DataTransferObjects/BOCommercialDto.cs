using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class BOCommercialDto
   {
      /// <summary>
      /// El ID del comercial.
      /// </summary>
      [JsonPropertyName("id")]
      public int CommercialId { get; set; }
      /// <summary>
      /// El nombre del comercial.
      /// </summary>
      [JsonPropertyName("name")]
      public string CommercialName { get; set; }
      /// <summary>
      /// El email del comercial.
      /// </summary>
      [JsonPropertyName("email")]
      public string CommercialEmail { get; set; }
      /// <summary>
      /// El peer del comercial.
      /// </summary>
      [JsonPropertyName("peer")]
      public int Peer { get; set; }
      /// <summary>
      /// El Id del comercial en SIEBEL.
      /// </summary>
      [JsonPropertyName("siebel_id")]
      public string SiebelId { get; set; }
      /// <summary>
      /// El numero de telefono en la PBX del comercial.
      /// </summary>
      [JsonPropertyName("pbx_phone_number")]
      public string PBXPhoneNumber { get; set; }
      /// <summary>
      /// El numero de celular del comercial.
      /// </summary>
      [JsonPropertyName("mobile_phone_number")]
      public string MobilePhoneNumber { get; set; }
      /// <summary>
      /// Estado del comercial.
      /// </summary>
      [JsonPropertyName("active")]
      public bool Active { get; set; }
      /// <summary>
      /// Estado de la conexión del comercial.
      /// </summary>
      [JsonPropertyName("connected")]
      public bool Connected { get; set; }
      /// <summary>
      /// Comerciales alternativo.
      /// </summary>
      [JsonPropertyName("alternatives")]
      public List<BOAlternativeCommercialDto> Alternatives { get; set; }
   }
}
