using inConcert.iMS.Domain;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   public interface ICustomerService
    {
      Task<GetCustomersResult> GetCustomersBySiebelId(string endPoint, string siebelId);
      Task<GetCustomersResult> GetCustomersByCommercialId(string endPoint, string commercialId);
      GetCustomersByCallResponseModel GetCustomersByCallId(int data);
   }
}
