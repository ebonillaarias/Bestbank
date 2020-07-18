using inConcert.iMS.Domain;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace inConcert.iMS.ServiceAgent.Siebel
{
   public class SiebelClient : ISiebelClient
   {
      private readonly HttpClient _client;

      public SiebelClient(HttpClient client)
      {
         _client = client;
      }

      public async Task<SAResponseData> GetCustomers(string endPoint, string userId)
      {
         string stringPayload = System.Text.Json.JsonSerializer.Serialize(new { UserId = userId });
         StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
         HttpResponseMessage response = await _client.PostAsync(endPoint, httpContent);
         string responseContent = await response.Content.ReadAsStringAsync();
         SAResponseData responseModel = System.Text.Json.JsonSerializer.Deserialize<SAResponseData>(responseContent);
         return responseModel;
      }

      public async Task<SAResponseData> NewCall(string endPoint, SiebelRequestNewCallModel model)
      {
         string stringPayload = System.Text.Json.JsonSerializer.Serialize(new { InteractionId = model.InteractionId, Direction = model.Direction.ToString(), CallerId = model.CallerId, CalledId = model.CalledId, UserId = model.UserId });
         StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
         HttpResponseMessage response = await _client.PostAsync(endPoint, httpContent);
         string responseContent = await response.Content.ReadAsStringAsync();
         SAResponseData responseModel = System.Text.Json.JsonSerializer.Deserialize<SAResponseData>(responseContent);
         return responseModel;
      }

      public async Task<SAStatusResponse> EndCall(string endPoint, SiebelRequestEndCallModel model)
      {
         string stringPayload = System.Text.Json.JsonSerializer.Serialize(
            new { CustomerId = model.CustomerId, 
                  InteractionId = model.InteractionId, 
                  StartDate = model.StartDate, 
                  EndDate = model.EndDate, 
                  CallResult = model.CallResult
                 });

         StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
         HttpResponseMessage response = await _client.PostAsync(endPoint, httpContent);
         string responseContent = await response.Content.ReadAsStringAsync();
         
         SAStatusResponse responseModel = System.Text.Json.JsonSerializer.Deserialize<SAStatusResponse>(responseContent);
         return responseModel;
      }
   }
}
