using inConcert.iMS.Enums;
using System;

namespace inConcert.iMS.Domain
{
   public class PostCallsStartCallPartRequestModel
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
      /// Peer al que se va a realizar el intento de llamada.
      /// </summary>
      public int Peer { get; set; }

      /// <summary>
      /// Id de comercial al momento de la llamada.
      /// </summary>
      public int CommercialId { get; set; }

      /// <summary>
      /// Nombre del comercial al momento de la llamada.
      /// </summary>
      public string CommercialName { get; set; }

      /// <summary>
      /// Nombre del canal SIP que origina la llamada.
      /// </summary>
      public string OrigChannel { get; set; }

      /// <summary>
      /// Nombre del canal SIP a utilizar como parámetro RedirectChannel en las llamadas al método RedirectCall del interfaz iAMI.
      /// </summary>
      public string RedirectChannel { get; set; }

      /// <summary>
      ///  Fecha y hora UTC de inicio del intento de llamada.
      /// </summary>
      public DateTimeOffset StartDate { get; set; }
   }
}
