using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class SiebelRequestNewCallModel
   {
      /// <summary>
      /// Identifica el ID de la llamada.
      /// </summary>
      public string InteractionId { get; set; }
      
      /// <summary>
      /// Dirección de la llamada.
      /// </summary>
      public CallDirection Direction { get; set; }

      /// <summary>
      /// Identifica el origen de la llamada.
      /// </summary>
      public string CallerId { get; set; }

      /// <summary>
      /// Identifica teléfono de destino de llamada.
      /// </summary>
      public string CalledId { get; set; }
      
      /// <summary>
      /// Id del usuario/comercial.
      /// Será utilizado para devolver la lista de clientes de este usuario.
      /// </summary>
      public string UserId { get; set; }
      
      /// <summary>
      /// Id del cliente.
      /// </summary>
      public string CustomerId { get; set; }
   }
}
