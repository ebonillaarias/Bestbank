using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
   public class RequestUploadCallPartFileDto
   {
      /// <summary>
      ///  Identificador único de llamada.
      /// </summary>
      [Required]
      public string CallId { get; set; }
      
      /// <summary>
      ///  Número secuencial de cada parte de una llamada, comenzando en 1.
      /// </summary>
      [Required]
      public string CallPartNumber { get; set; }
      
      /// <summary>
      /// Contenido del fichero de audio.
      /// </summary>
      [Required]
      public IFormFile FileContent { get; set; }
   }
}
