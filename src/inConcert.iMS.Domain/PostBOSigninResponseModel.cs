using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice SignIn Response Model.
   /// </summary>
   public class PostBOSigninResponseModel
   {
      /// <summary>
      /// El token contiene email y user-id del usuario.
      /// </summary>
      public string accessToken { get; set; }

      /// <summary>
      /// Estado cuando se hizo la conexión
      /// </summary>
      public ResultStatus status { get; set; }
   }
}
