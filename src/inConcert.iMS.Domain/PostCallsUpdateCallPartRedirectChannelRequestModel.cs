namespace inConcert.iMS.Domain
{
   public class PostCallsUpdateCallPartRedirectChannelRequestModel
   {
      /// <summary>
      /// Identificador único de llamada.
      /// </summary>
      public int CallId { get; set; }

      /// <summary>
      /// Número secuencial de cada parte de una llamada, comenzando en 1.
      /// </summary>
      public string CallPartNumber { get; set; }

      /// <summary>
      /// Nombre del canal SIP a utilizar como parámetro RedirectChannel en las llamadas al método RedirectCall del interfaz iAMI.
      /// </summary>
      public string RedirectChannel { get; set; }
   }
}
