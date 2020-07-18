namespace inConcert.iMS.Domain
{
   public class PostCallsStartOutboundCallResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Id de llamada dada de alta.
      /// </summary>
      public int CallId { get; set; }

      /// <summary>
      /// PBXPhoneNumber.
      /// </summary>
      public string PBXPhoneNumber { get; set; }

      /// <summary>
      /// MobilePhoneNumber.
      /// </summary>
      public string MobilePhoneNumber { get; set; }
   }
}
