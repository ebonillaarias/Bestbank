using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class PutCallsEndCallResponseModel
   {
        /// <summary>
        /// Estado del mensaje.
        /// </summary>
        public ResultStatus Status { get; set; }

      /// <summary>
      /// Mensaje.
      /// </summary>
      public string Message { get; set; }
   }
}
