using inConcert.iMS.Domain;
using System.Threading.Tasks;

namespace inConcert.iMS.ServiceAgent.Siebel
{
   public interface ISiebelClient
   {
      Task<SAResponseData> GetCustomers(string endPoint, string userId);
      
      Task<SAResponseData> NewCall(string endPoint, SiebelRequestNewCallModel model);

      Task<SAStatusResponse> EndCall(string endPoint, SiebelRequestEndCallModel model);
   }
}
