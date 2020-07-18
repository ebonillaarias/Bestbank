using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice Calls Response Model.
   /// </summary>
   public class PostCallsUpdateCallPartRedirectChannelResponseModel
   {
      /// <summary>
      /// Resultado de la solicitud.
      /// </summary>
      public ResultStatus Status { get; set; }

      /// <summary>
      /// Mensaje.
      /// </summary>
      public string Message { get; set; }
   }
}
