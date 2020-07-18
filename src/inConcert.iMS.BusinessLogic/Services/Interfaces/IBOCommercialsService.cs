using inConcert.iMS.Domain;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   public interface IBOCommercialsService
   {
      /// <summary>
      /// <para>
      /// Busca todos los comerciales en los cuales el campo 'Name' coincide y/o contiene la palabra
      /// clave indicada por el parametro <paramref name="strName"/>.
      /// </para>
      /// <para>
      /// Si <paramref name="strName"/> es NULL, string vacio o string blanco (todos espacios)
      /// se recuperan todos los comerciales.
      /// </para>
      /// </summary>
      /// <param name="strName">Palabra clave a buscar en los nombres de los comerciales.</param>
      /// <param name="pMaxTimeKeepAlive">Tiempo máximo (en minutos, 1 por defecto) por el cuál se asume que un comercial esta conectado tras el último KeepAlive.</param>
      /// <returns>
      /// Modelo <see cref="BOGetCommercialsResponseModel"/> con los datos de la respuesta.
      /// </returns>
      BOGetCommercialsResponseModel GetCommercials(string strName = null, uint pMaxTimeKeepAlive = 1);
      
      GetBOCommercialByIdResponseModel GetCommercialById(int id);
      GetBOCommercialByPeerResponseModel GetCommercialByPeer(int peer);
      PostBOCommercialsResponseModel CreateCommercial(PostBOCommercialsRequestModel commercialData);
      StatusResponseModel DeleteCommercial(int id);
      StatusResponseModel UpdateCommercial(int id, PutCommercialRequestModel commercial);
   }
}
