using inConcert.iMS.Enums;
using System;

namespace inConcert.iMS.Domain.Entities
{
   public class CallParts
   {
      /// <summary>
      /// Id de la parte de la llamada.
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Identificador único de llamada.
      /// </summary>
      public int CallId { get; set; }
      public Calls Call { get; set; }

      /// <summary>
      /// Número secuencial de cada parte de una llamada, comenzando en 1.
      /// </summary>
      public string CallPartNumber { get; set; }
      
      /// <summary>
      /// Peer asociado a esta parte de la llamada.
      /// </summary>
      public int Peer { get; set; }

      /// <summary>
      /// Id del comercial asociado a esta parte de la llamada.
      /// </summary>
      public int CommercialId { get; set; }
      
      /// <summary>
      /// Nombre del comercial asociado a esta parte de la llamada.
      /// </summary>
      public string CommercialName { get; set; }
      
      /// <summary>
      /// Nombre del canal SIP que origina la llamada.
      /// Es el que se utilizará como parámetro Channel en las llamadas a los métodos PauseCallRecord y 
      /// ResumeCallRecord del interfaz AMI
      /// </summary>     
      public string OrigChannel { get; set; }

      /// <summary>
      /// Nombre del canal SIP a utilizar como parámetro RedirectChannel en
      /// las llamadas al método RedirectCall del interfaz iAMI.
      /// </summary>
      public string RedirectChannel { get; set; }
      
      /// <summary>
      /// Fecha y hora UTC de inicio de esta parte de la llamada.
      /// </summary>
      public DateTimeOffset StartDate { get; set; }

      /// <summary>
      /// Fecha y hora UTC de fin de esta parte de la llamada.
      /// </summary>
      public DateTimeOffset? EndDate { get; set; }

      /// <summary>
      ///  El resultado telefónico de esta parte de la llamada.
      ///  Posibles valores “NoAnswer”, “Rejected”, “Completed” y “Transferred”.
      /// </summary>
      public CallResult CallResult { get; set; }

      /// <summary>
      /// “Cliente” si fue el cliente quien finalizó el intento de llamada, o “Comercial” si el comercial no llegó a contestar la llamada.
      /// </summary>
      public CallPartEndedBy CallPartEndedBy { get; set; }

      /// <summary>
      /// Ruta relativa del fichero que contiene la grabación de esta parte de la llamada.
      /// </summary>
      public string FilePath { get; set; }

      /// <summary>
      /// Motivo asociado al rechazo de la llamada. Aplica para CallResult=Rejected.Vacío si no aplica.
      /// </summary>
      public string RejectionReason { get; set; }
   }
}
