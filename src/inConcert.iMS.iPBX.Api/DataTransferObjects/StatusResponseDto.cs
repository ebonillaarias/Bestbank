namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
   public class StatusResponseDto
   {
      /// <summary>
      /// Indicador de éxito o fracaso de la operación.
      /// </summary>
      public string Status { get; set; }
      
      /// <summary>
      /// Texto asociado al status.
      /// </summary>
      public string Message { get; set; }
   }
}