namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
   public class ResponseStartOutboundCallDto : StatusResponseDto
   {
      /// <summary>
      /// Identificador único de llamada.
      /// </summary>
      public string CallId { get; set; }

      /// <summary>
      /// Número de teléfono fijo asociado al comercial.
      /// </summary>
      public string PBXPhoneNumber { get; set; }

      /// <summary>
      /// Número de teléfono móvil asociado al comercial.
      /// </summary>
      public string MobilePhoneNumber { get; set; }
   }
}
