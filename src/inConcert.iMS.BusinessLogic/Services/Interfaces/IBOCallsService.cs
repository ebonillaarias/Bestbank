using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   /// <summary>
   /// Interfaz de Administracion de llamadas en BackOffice
   /// </summary>
   public interface IBOCallsService
   {
      /// <summary>
      /// Dado un comercial retorna información sobre las llamadas en las que participó.
      /// Si <paramref name="days"/> es 0 (cero) retorna todas las llamadas, en otro caso
      /// se devuelve las llamadas de los últimos n dias indicado por dicho parámetro.
      /// </summary>
      /// <param name="commercialId">Id del comercial.</param>
      /// <param name="days">Últimos días a considerar.</param>
      /// <returns>
      /// Modelo <see cref="GetBOCallsByCommercialResponseModel"/> con los datos de la respuesta.
      /// </returns>
      GetBOCallsByCommercialResponseModel GetCallsByCommercials(int commercialId, int days);

      /// <summary>
      /// Retorna información sobre las llamadas, en el cual los filtros <paramref name="data.Result"/> y
      /// <paramref name="data.Commercials"/> aplican UNICAMENTE sobre la ultima parte de la llamada;
      /// mientras que el resto de los filtros (StarDate, EndDate y Client) aplican sobre la llamada en si (tabla Calls)
      /// </summary>
      /// <param name="data">Modelo con los datos a filtrar.</param>
      /// <returns>
      /// Modelo <see cref="PostBOCallsResponseModel"/> con los datos de la respuesta.
      /// </returns>
      PostBOCallsResponseModel GetCallsByLastPart(PostBOCallsRequestModel data);

      /// <summary>
      /// Recupera información de una llamada y las partes de la misma.
      /// </summary>
      /// <param name="id">ID de la llamada de la cual se desea obtener información.</param>
      /// <param name="url">URL donde se alojan los audios de las partes de la llamada.</param>
      /// <returns>
      /// Modelo <see cref="GetBOCallDetailsResponseModel"/> con los datos de la respuesta.
      /// </returns>
      GetBOCallDetailsResponseModel GetCallDetails(int id, string url);

      /// <summary>
      /// Retorna información sobre las llamadas.
      /// </summary>
      /// <param name="data">Modelo con los datos a filtrar.</param>
      /// <returns>
      /// Modelo <see cref="GetBOCallsCustomersByCommercialsResponseModel"/> con los datos de la respuesta.
      /// </returns>
      GetBOCallsCustomersByCommercialsResponseModel GetCustomersByCommercials(List<int> data);

      Task<StatusResponseModel> StartCallPart(string endPoint, PostCallsStartCallPartRequestModel data);

      PostCallsUpdateCallPartRedirectChannelResponseModel UpdateCallPartRedirectChannel(PostCallsUpdateCallPartRedirectChannelRequestModel data);

      PostCallsEndCallPartResponseModel UpdateCallPartEnd(PostCallsEndCallPartRequestModel data);

      Task<StatusResponseModel> EndCallUpdateDBState(int inCallId, string host);

      Task<PostCallsStartInboundCallResponseModel> StartInboundCall(string host, PostCallsStartInboundCallRequestModel data, bool getPeersOnlyConnected);

      Task<PostCallsStartOutboundCallResponseModel> StartOutboundCall(string host, PostCallsStartOutboundCallRequestModel data);

      StatusResponseModel UploadCallPartFile_UpdateTable(PostCallsUploadCallPartFileRequestModel data);

      StatusResponseModel UploadCallPartFile_UploadFile(PostCallsUploadCallPartFileRequestModel data, List<string> allowExtentions);

      Task<StatusResponseModel> SetCustomer(PostCallsSetCustomerRequestModel data, CallDirection direction, string host);

      /// <summary>
      /// Transferir una llamada activa a otro comercial.
      /// </summary>
      /// <param name="inOrigChannel">Nombre del canal SIP que originó la llamada.</param>
      /// <param name="inRedirectChannel">Nombre del canal SIP que debe redirigirse a otro peer.</param>
      /// <param name="inPeer">Peer del comercial al cual transferir.</param>
      /// <returns>
      /// Modelo <see cref="StatusResponseModel"/> con los datos de la respuesta.
      /// </returns>
      StatusResponseModel CallRedirect(string inOrigChannel, string inRedirectChannel, int inPeer);

      /// <summary>
      /// Permite obtener el id de la llamada en base a una session del comercial.
      /// </summary>
      /// <param name="id">SessionId del comercial.</param>
      /// <param name="inboundOnly">Booleano para indicar si solo debe tener en cuenta las llamadas entrantes (true) o todas (false).</param>
      /// <returns></returns>
      GetCallIdBySessionIdResponseModel GetCallIdBySessionId(string id, bool inboundOnly = true, bool isRecordCall = false);

      StatusResponseModel RecordCall(string origChannel, bool record);

      GetCommercialsResponseModel GetCommercialsFromCallParts();

      GetCustomersResponseModel GetCustomersFromCalls();
   }
}
