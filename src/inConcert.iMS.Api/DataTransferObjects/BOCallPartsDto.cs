using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice CallParts DTO.
   /// </summary>
   public class BOCallPartsDto
   {
      /// <summary>
      /// Numero de secuencia de la parte de una llamada
      /// </summary>
      [JsonPropertyName("id")]
      public int CallPart { get; set; }

      /// <summary>
      /// Duracion de la parte de una llamada
      /// </summary>
      [JsonPropertyName("duration")]
      public string Duration { get; set; }

      /// <summary>
      /// Ruta del archivo de la parte de una llamada.
      /// </summary>
      [JsonPropertyName("file")]
      public string Path { get; set; }
   }
}
