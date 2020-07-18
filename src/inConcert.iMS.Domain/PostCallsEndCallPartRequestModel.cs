namespace inConcert.iMS.Domain
{
   public class PostCallsEndCallPartRequestModel
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
      ///  Resultado de la llamada, que podrá tener los siguientes valores
      ///  o Rejected.La llamada fue rechazada por el destinatario, por el Sistema o por algún elemento de la red
      ///  o NoAnswer.La llamada no fue rechazada ni contestada por el destinatario dentro del intervalo de tiempo que se defina.
      ///  o Completed: La llamada fue contestada por el destinatario
      /// </summary>
      public string CallResult { get; set; }


      /// <summary>
      ///  Resultado de la llamada, que podrá tener los siguientes valores
      ///   “Cliente” si fue el cliente quien finalizó el intento de llamada, o
      ///   “Comercial” si el comercial no llegó a contestar la llamada
      /// </summary>
      public string CallPartEndedBy { get; set; }

      /// <summary>
      ///  Motivo asociado al rechazo de la llamada. Aplica para CallResult=Rejected. Vacío si no aplica.
      /// </summary>
      public string RejectionReason { get; set; }
   }
}
