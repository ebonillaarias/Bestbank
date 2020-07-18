using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace inConcert.iMS.ServiceAgent.FirebaseCM
{
   /// <summary>
   /// Proporciona una interfaz Firebase Cloud Message
   /// </summary>
   public interface IFirebaseClient
   {
      /// <summary>
      /// Envía una notificacion push al dispositivo (comercial).
      /// </summary>
      /// <param name="endPoint">Endpoint del servicio Firebase para el envio de notificaciones push.</param>
      /// <param name="token">Token del dispositivo (comercial) al cual se debe enviar la notificacion push.</param>
      /// <returns>
      /// Modelo <see cref="FCMResponse"/> con los datos de la respuesta.
      /// </returns>
      Task<FCMResponse> Send(string endPoint, string token);
   }
}
