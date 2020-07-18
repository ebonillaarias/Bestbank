using inConcert.iMS.Enums;

namespace inConcert.iMS.Api.DataTransferObjects
{
   /// <summary>
   /// BackOffice SignIn Response DTO.
   /// </summary>
   public class PostBOSigninResponseDto
   {
      /// <summary>
      /// El token contiene email y user-id del usuario backoffice.
      /// </summary>
      public string access_token { get; set; }

      /// <summary>
      /// Estado cuando se hizo la conexión
      /// </summary>
      public ResultStatus status { get; set; }
   }
}
