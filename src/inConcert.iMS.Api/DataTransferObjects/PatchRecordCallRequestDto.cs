using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PatchRecordCallRequestDto
   {
      /// <summary>
      /// El ID de la sesion del comercial.
      /// </summary>
      public string SessionId { get; set; }
      /// <summary>
      /// Bandera para indicar si quiere pausar TRUE=Pausa la grabación, FALSE=Reanuda la grabación
      /// </summary>
      public bool Record { get; set; }
   }
}