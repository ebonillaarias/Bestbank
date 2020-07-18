using System.Globalization;
using System.Net;
using System.Text;

namespace inConcert.iMS.Enums
{
   public static class Utilities
   {
      #region String-Enum
      public static PhoneType GetPhoneType(string phoneType)
      {
         PhoneType phoneTypeRes;
         switch (phoneType.ToLower())
         {
            case "home":
               phoneTypeRes = PhoneType.Home;
               break;
            case "office":
               phoneTypeRes = PhoneType.Office;
               break;
            case "fax":
               phoneTypeRes = PhoneType.Fax;
               break;
            case "mobile":
            default:
               phoneTypeRes = PhoneType.Mobile;
               break;
         }
         return phoneTypeRes;
      }

      public static CustomerType GetCustomerType(string customerType)
      {
         CustomerType customerTypeRes;
         switch (customerType.ToLower())
         {
            case "r":
               customerTypeRes = CustomerType.Residential;
               break;
            case "b":
            default:
               customerTypeRes = CustomerType.Business;
               break;
         }
         return customerTypeRes;
      }

      public static CallDirection GetCallDirectionEnum(string callDirection)
      {
         CallDirection callDirectionRes;
         switch (callDirection.ToLower())
         {
            case "outbound":
               callDirectionRes = CallDirection.Outbound;
               break;
            case "inbound":
            default:
               callDirectionRes = CallDirection.Inbound;
               break;
         }
         return callDirectionRes;
      }

      public static CallResult GetCallResultEnum(string callResult)
      {
         CallResult callResultReturn = CallResult.Completed;

         switch (callResult.ToLower())
         {
            case "noanswer":
               callResultReturn = CallResult.NoAnswer;
               break;
            case "rejected":
               callResultReturn = CallResult.Rejected;
               break;
            case "transferred":
               callResultReturn = CallResult.Transferred;
               break;
         }

         return callResultReturn;
      }

      public static string GetCallDirection(CallDirection direction)
      {
         string callDirection = "Inbound";

         switch (direction)
         {
            case CallDirection.Outbound:
               callDirection = "Outbound";
               break;
         }

         return callDirection;
      }

      public static string GetCallResult(CallResult callResult)
      {
         string callResultReturn = "Completed";

         switch (callResult)
         {
            case CallResult.NoAnswer:
               callResultReturn = "NoAnswer";
               break;
            case CallResult.Rejected:
               callResultReturn = "Rejected";
               break;
            case CallResult.Transferred:
               callResultReturn = "Transferred";
               break;
            case CallResult.InProgress:
               callResultReturn = "";
               break;
         }

         return callResultReturn;
      }
      #endregion

      public static string RemoveDiacritics(string text)
      {
         var normalizedString = text.Normalize(NormalizationForm.FormD);
         var stringBuilder = new StringBuilder();

         foreach (var c in normalizedString)
         {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
               stringBuilder.Append(c);
            }
         }

         return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
      }
   }
}
