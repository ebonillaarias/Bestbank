using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.Controllers
{
    /// <summary>
    /// Servicios de clientes.
    /// </summary>
    [Produces("application/json")]
    [Route("api/bo/calls")]
    [ApiController]
    public class BOCallsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IBOCallsService _boCallsService;
        private readonly ISecurityService _securityService;

        public BOCallsController(IMapper mapper, IConfiguration config, IBOCallsService boCallsService, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _boCallsService = boCallsService;
            _securityService = securityService;
        }

        /// <summary>
        /// Permite a un supervisor filtrar informacion sobre las llamadas.
        /// </summary>
        /// <param name="dataDtoIn">Datos que se deben filtrar.</param>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(List<BOCallDto>), StatusCodes.Status200OK)]
        public IActionResult GetCalls([FromBody][Required] PostBOCallsRequestDto dataDtoIn)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetCalls: " + JsonConvert.SerializeObject(dataDtoIn));

                PostBOCallsRequestModel requestModel = _mapper.Map<PostBOCallsRequestModel>(dataDtoIn);
                PostBOCallsResponseModel responseModel = _boCallsService.GetCallsByLastPart(requestModel);

                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<BOCallDto> response = _mapper.Map<List<BOCallDto>>(responseModel.Calls);
                        return Ok(response);
                    case ResultStatus.BAD_REQUEST:
                        // Datos entrada incorrectos
                        return BadRequest();
                    case ResultStatus.ACCESS_DENIED:
                        // Acceso denegado por no estar autenticado
                        return StatusCode(401);
                    case ResultStatus.NOT_AUTHORIZED:
                        // Sin autorización
                        return StatusCode(403);
                    default:
                        // Error interno
                        return StatusCode(500);
                }
            }

            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor filtrar informacion sobre las llamadas.
        /// </summary>
        /// <param name="commercial_id"> Id del  comercial.</param>
        /// <param name="days"> Dias a buscar.</param>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("by-commercial")]
        [ProducesResponseType(typeof(List<GetCallsRecentsByCommercialDto>), StatusCodes.Status200OK)]
        public IActionResult GetCallsByCommercials([FromQuery][Required] string commercial_id, [FromQuery] string days)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetCallsByCommercials: {commercial_id: " + commercial_id + ", days: " + days + "}");

                if (string.IsNullOrWhiteSpace(commercial_id))
                {
                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(days))
                {
                    days = "7";
                }

                if (!int.TryParse(days, out int pdays))
                {
                    responseDto = new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    };
                    return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                }

                GetBOCallsByCommercialResponseModel responseModel = _boCallsService.GetCallsByCommercials(int.Parse(commercial_id), pdays);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<GetCallsRecentsByCommercialDto> response200Dto = _mapper.Map<List<GetCallsRecentsByCommercialDto>>(responseModel.ListCustomer);
                        return StatusCode(StatusCodes.Status200OK, response200Dto);
                    case ResultStatus.BAD_REQUEST:
                        // Datos entrada incorrectos
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                    default:
                        // Error interno
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                }
            }

            catch (Exception e)
            {
                responseDto = new StatusResponseDto
                {
                    Status = "ERROR",
                    Message = "[BOCallsController-Bycommercial-Exception: " + e.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
            }
        }

        /// <summary>
        /// Permite a un supervisor obtener los datos de clientes en base a las llamadas recibidas por un conjunto de comerciales.
        /// </summary>
        /// <param name="dataDtoIn">Lista de comerciales de los cuales buscar llamadas.</param>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("clientsbycommercials")]
        [ProducesResponseType(typeof(List<CustomerLiteDto>), StatusCodes.Status200OK)]
        public IActionResult GetClientsByCommercials([FromBody][Required] List<int> dataDtoIn)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetClientsByCommercials: " + JsonConvert.SerializeObject(dataDtoIn));

                if (dataDtoIn == null)
                {
                    // Datos de entrada incorrectos.
                    return BadRequest();
                }

                GetBOCallsCustomersByCommercialsResponseModel businessResult = _boCallsService.GetCustomersByCommercials(dataDtoIn);

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<CustomerLiteDto> response = _mapper.Map<List<CustomerLiteDto>>(businessResult.Customers);
                        return Ok(response);
                    case ResultStatus.ACCESS_DENIED:
                        // Acceso denegado por no estar autenticado
                        return StatusCode(401);
                    case ResultStatus.NOT_AUTHORIZED:
                        // Sin autorización
                        return StatusCode(403);
                    default:
                        // Error interno
                        return StatusCode(500);
                }
            }

            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Recupera información de una llamada y las partes de la misma.
        /// </summary>
        /// <param name="id">ID de la llamada de la cual se desea obtener información.</param>
        /// <response code="200">Solicitud OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetBOCallDetailsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        public IActionResult GetCallDetails([FromRoute][Required] int id)
        {
            StatusResponseDto statusResponseDto;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetCallDetails: " + JsonConvert.SerializeObject(id));

                string fileServerURL = _config.GetValue<string>("FileServer:URL");
                string fileServerDir = _config.GetValue<string>("FileServer:Dir");
                string path = System.IO.Path.Combine(fileServerURL, fileServerDir); // directorio donde se alojan los audios
                GetBOCallDetailsResponseModel businessResponse = _boCallsService.GetCallDetails(id, path);

                switch (businessResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        GetBOCallDetailsResponseDto response;
                        response = _mapper.Map<GetBOCallDetailsResponseDto>(businessResponse.Info);
                        response.Parts = _mapper.Map<List<BOCallPartsDto>>(businessResponse.Parts);
                        response.Records = _mapper.Map<List<CallsRecodsResponseDto>>(businessResponse.Records);
                        return StatusCode(StatusCodes.Status200OK, response);

                    case ResultStatus.NOT_FOUND:
                        statusResponseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status404NotFound);

                    default:
                        statusResponseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status500InternalServerError, statusResponseDto);
                }
            }
            catch (Exception e)
            {
                statusResponseDto = new StatusResponseDto
                {
                    Status = "ERROR",
                    Message = "[BOCallsController-GetCallDetails-Exception: " + e.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, statusResponseDto);
            }
        }

        /// <summary>
        /// Permite actualizar customerId y customerName de una llamada entrante
        /// </summary>
        /// <param name="data">Datos de la llamada a actualizar</param>
        /// <response code="200">Comercial actualizado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Servicio no disponible.</response>
        [HttpPost("set-customer")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostCallSetCustomer([FromBody][Required] PostCallsSetCustomerRequestDto data)
        {
            StatusResponseDto responseDto;

            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/PostCallSetCustomer: " + JsonConvert.SerializeObject(data));

                #region Validacion datos entrada
                bool error = false;
                responseDto = new StatusResponseDto();

                if (!error && string.IsNullOrWhiteSpace(data.CustomerId))
                {
                    responseDto.Status = "CUSTOMER_ID_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "customer_id");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.CustomerName))
                {
                    responseDto.Status = "CUSTOMER_NAME_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "customer_name");
                    error = true;
                }

                if (error)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                }
                #endregion

                PostCallsSetCustomerRequestModel callSetCustomerModel = _mapper.Map<PostCallsSetCustomerRequestModel>(data);
                string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointEndCall");

                #region Issue ICBB-708
                // If CustomerId == REJECTED_CALL then notify Siebel without customer.
                if (string.Equals(data.CustomerId, Constants.REJECTED_CALL))
                {
                    callSetCustomerModel.CustomerName = string.Empty;
                }
                #endregion

                StatusResponseModel businessResult = await _boCallsService.SetCustomer(callSetCustomerModel, CallDirection.Inbound, endPoint);

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResult);
                        return StatusCode(StatusCodes.Status200OK, responseDto);
                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);
                    case ResultStatus.NOT_FOUND:
                        return NotFound();
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, businessResult.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite actualizar customerId y customerName de una llamada saliente
        /// </summary>
        /// <param name="data">Datos de la llamada a actualizar</param>
        /// <response code="200">Comercial actualizado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Servicio no disponible.</response>
        [HttpPost("set-customer-outbound")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostCallSetCustomerOutbound([FromBody][Required] PostCallsSetCustomerRequestDto data)
        {
            StatusResponseDto responseDto;

            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/PostCallSetCustomerOutbound: " + JsonConvert.SerializeObject(data));

                #region Validacion datos entrada
                bool error = false;
                responseDto = new StatusResponseDto();

                if (!error && string.IsNullOrWhiteSpace(data.CustomerId))
                {
                    responseDto.Status = "CUSTOMER_ID_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "customer_id");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.CustomerName))
                {
                    responseDto.Status = "CUSTOMER_NAME_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "customer_name");
                    error = true;
                }

                if (error)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                }
                #endregion

                var callSetCustomerModel = _mapper.Map<PostCallsSetCustomerRequestModel>(data);
                string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointEndCall");
                StatusResponseModel businessResult = await _boCallsService.SetCustomer(callSetCustomerModel, CallDirection.Outbound, endPoint);

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResult);
                        return StatusCode(StatusCodes.Status200OK, responseDto);
                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);
                    case ResultStatus.NOT_FOUND:
                        return NotFound();
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, businessResult.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Indicar a la PBX pausar o reanudar grabacion de la llamada
        /// </summary>
        /// <param name="dataDtoIn">SessionId y bandera para definir si se pausa o reanuda la grabación.</param>
        /// <response code="204">Solicitud OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Error con el servicio externo.</response>
        [HttpPatch("/api/calls/record")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult RecordCall([FromBody][Required] PatchRecordCallRequestDto dataDtoIn)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>               

                if (dataDtoIn == null || dataDtoIn.SessionId == "")
                {
                    return BadRequest();
                }

                StatusResponseModel businessResponse;

                //Verificamos que el comercial tenga una llamada con ese sessionID
                GetCallIdBySessionIdResponseModel callModel = _boCallsService.GetCallIdBySessionId(dataDtoIn.SessionId, dataDtoIn.Record, true);

                _securityService.RequestLog("api/bo/calls/RecordCall: " + JsonConvert.SerializeObject(dataDtoIn) + " || response: " + JsonConvert.SerializeObject(callModel));

                switch (callModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        businessResponse = _boCallsService.RecordCall(callModel.OrigChannel, dataDtoIn.Record);
                        break;

                    case ResultStatus.NOT_FOUND:
                        businessResponse = new StatusResponseModel
                        {
                            Message = callModel.Message,
                            Status = ResultStatus.NOT_FOUND
                        };
                        break;

                    default:
                        businessResponse = new StatusResponseModel
                        {
                            Message = callModel.Message,
                            Status = ResultStatus.ERROR
                        };
                        break;
                }

                StatusResponseDto responseDto;
                switch (businessResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status200OK, responseDto);

                    case ResultStatus.BAD_REQUEST:
                        return StatusCode(StatusCodes.Status400BadRequest);

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_FOUND:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status404NotFound, responseDto);

                    case ResultStatus.CANNOT_CONNECT_TO_PBX:
                    case ResultStatus.EXTERNAL_SERVICE_ERROR:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, responseDto);

                    default:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Transferir una llamada activa a otro comercial.
        /// </summary>
        /// <param name="dataIn">Session ID del comercial que realiza la transferencia y el Peer a donde se va redireccionar la llamada.</param>
        /// <response code="200">Llamada transferida con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">El servicio externo no está disponible o retornó un error.</response>
        [HttpPatch("/api/calls/redirect")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        public IActionResult PatchCallRedirect([FromBody][Required] PatchRedirectCallRequestDto dataIn)
        {
            StatusResponseDto responseDto;

            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/PatchCallRedirect: " + JsonConvert.SerializeObject(dataIn));

                if (dataIn == null || dataIn.SessionId == "")
                {
                    return BadRequest();
                }
                // Se busca la llamada asociada al comercial segun el sessionId del mismo.
                GetCallIdBySessionIdResponseModel responseCallModel = _boCallsService.GetCallIdBySessionId(dataIn.SessionId, false);
                switch (responseCallModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        StatusResponseModel responseCallRedirect = _boCallsService.CallRedirect(responseCallModel.OrigChannel, responseCallModel.RedirectChannel, dataIn.Peer);
                        switch (responseCallRedirect.Status)
                        {
                            case ResultStatus.SUCCESS:
                                responseDto = _mapper.Map<StatusResponseDto>(responseCallRedirect);
                                return StatusCode(StatusCodes.Status200OK, responseDto);

                            case ResultStatus.NOT_FOUND:
                                responseDto = _mapper.Map<StatusResponseDto>(responseCallRedirect);
                                return StatusCode(StatusCodes.Status404NotFound, responseDto);

                            case ResultStatus.CANNOT_CONNECT_TO_PBX:
                            case ResultStatus.EXTERNAL_SERVICE_ERROR:
                                responseDto = _mapper.Map<StatusResponseDto>(responseCallRedirect);
                                return StatusCode(StatusCodes.Status503ServiceUnavailable, responseDto);

                            default:
                                responseDto = _mapper.Map<StatusResponseDto>(responseCallRedirect);
                                return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                        }

                    case ResultStatus.NOT_FOUND:
                        responseDto = _mapper.Map<StatusResponseDto>(responseCallModel);
                        return StatusCode(StatusCodes.Status404NotFound, responseDto);

                    default:
                        responseDto = _mapper.Map<StatusResponseDto>(responseCallModel);
                        return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                }

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor obtener los datos de comerciales de todas las call parts registradas.
        /// </summary>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("commercials-from-callparts")]
        [ProducesResponseType(typeof(List<FeatureDto>), StatusCodes.Status200OK)]
        public IActionResult GetCommercialsFromCallParts()
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetCommercialsFromCallParts: " + JsonConvert.SerializeObject(""));

                GetCommercialsResponseModel businessResult = _boCallsService.GetCommercialsFromCallParts();

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<FeatureDto> response = _mapper.Map<List<FeatureDto>>(businessResult.Commercials);
                        return Ok(response);
                    default:
                        // Error interno
                        return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor obtener los datos de clientes de todas las calls registradas.
        /// </summary>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("customers-from-calls")]
        [ProducesResponseType(typeof(List<CustomerFeatureDto>), StatusCodes.Status200OK)]
        public IActionResult GetCustomersFromCalls()
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/calls/GetCustomersFromCalls: " + JsonConvert.SerializeObject(""));

                GetCustomersResponseModel businessResult = _boCallsService.GetCustomersFromCalls();

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<CustomerFeatureDto> response = _mapper.Map<List<CustomerFeatureDto>>(businessResult.Customers);
                        return Ok(response);
                    default:
                        // Error interno
                        return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
