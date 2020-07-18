namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice SignIn Request DTO.
   /// </summary>
   public class PostBOSigninRequestDto
   {
      /// <summary>
      /// Usuario backoffice (supervisor) a conectar.
      /// </summary>
      public string user { get; set; }
      
      /// <summary>
      /// Contraseña del usuario backoffice.
      /// </summary>
      public string password { get; set; }
   }
}
