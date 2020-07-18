using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class PostBOCommercialsResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Nombre del comercial.
      /// </summary>
      public string Name { get; set; }
      
      /// <summary>
      /// Email del comercial.
      /// </summary>
      public string Email { get; set; }

      /// <summary>
      /// Contrasena del comercial.
      /// </summary>
      public string Password { get; set; }

      /// <summary>
      /// Peer asignado al comercial.
      /// </summary>
      public int Peer { get; set; }

      /// <summary>
      /// Link para descargar la app Android.
      /// </summary>
      public string LinkAppAndroid { get; set; }
   }
}
