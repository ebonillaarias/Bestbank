namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice SignIn Request Model.
   /// </summary>
   public class PostBOSigninRequestModel
   {
      /// <summary>
      /// Usuario supervisor a conectar
      /// </summary>
      public string user { get; set; }
      
      /// <summary>
      /// Contraseña del usuario supervisor
      /// </summary>
      public string password { get; set; }
   }
}
