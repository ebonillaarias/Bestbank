using Microsoft.AspNetCore.Http;

namespace inConcert.iMS.Domain
{
   public class PostCallsUploadCallPartFileRequestModel
   {
      /// <summary>
      /// Identificador único de llamada.
      /// </summary>
      public string CallId { get; set; }

      /// <summary>
      /// Número secuencial de cada parte de una llamada.
      /// </summary>
      public string CallPartNumber { get; set; }

      /// <summary>
      /// Ruta donde se alojara el archivo.
      /// </summary>
      public string PathToUpload { get; set; }

      /// <summary>
      /// Contenido del fichero de audio.
      /// </summary>
      public IFormFile FileContent { get; set; }
   }
}
