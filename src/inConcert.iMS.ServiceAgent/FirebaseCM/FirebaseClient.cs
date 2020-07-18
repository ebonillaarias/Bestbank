using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace inConcert.iMS.ServiceAgent.FirebaseCM
{

   /// <summary>
   /// Clase que implementa la interfaz <see cref="IFirebaseClient"/>
   /// </summary>
   public class FirebaseClient : IFirebaseClient
   {
      private readonly HttpClient _client;

      public FirebaseClient(HttpClient client)
      {
         _client = client;
      }

      /// <summary>
      /// Envía una notificacion push al dispositivo (comercial).
      /// </summary>
      /// <param name="endPoint">Endpoint del servicio Firebase para el envio de notificaciones push.</param>
      /// <param name="token">Token del dispositivo (comercial) al cual se debe enviar la notificacion push.</param>
      /// <returns>
      /// Modelo <see cref="FCMResponse "/> con los datos de la respuesta.
      /// </returns>
      public async Task<FCMResponse> Send(string endPoint, string token)
      {
         var dataPayload = new
         {
            to = token,
            priority = "high",
            time_to_live = 60,
            data = new
            {
               content_title = "FCM Tittle",
               content_text = "FCM Text",
            }
         };
         string stringPayload = System.Text.Json.JsonSerializer.Serialize(dataPayload);
         StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
         HttpResponseMessage response = await _client.PostAsync(endPoint, httpContent);
         string responseContent = await response.Content.ReadAsStringAsync();
         FCMResponse responseModel = System.Text.Json.JsonSerializer.Deserialize<FCMResponse>(responseContent);
         return responseModel;
      }
   }
}
