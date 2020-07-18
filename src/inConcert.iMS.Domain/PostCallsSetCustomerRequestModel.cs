namespace inConcert.iMS.Domain
{
   public class PostCallsSetCustomerRequestModel
   {
      /// <summary>
      /// Identificador de session.
      /// </summary>
      public string SessionId { get; set; }

      /// <summary>
      /// Id de cliente.
      /// </summary>
      public string CustomerId { get; set; }

      /// <summary>
      /// Nombre de cliente.
      /// </summary>
      public string CustomerName { get; set; }
   }
}
