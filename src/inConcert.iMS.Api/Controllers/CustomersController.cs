using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ICustomerService _customerService;
        private readonly IBOCallsService _callService;
        private readonly ISecurityService _securityService;
        public CustomersController(IMapper mapper, IConfiguration config, ICustomerService customerService, IBOCallsService callService, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _customerService = customerService;
            _callService = callService;
            _securityService = securityService;
        }

        /// <summary>
        /// Permite obtener los clientes del usuario comercial indicado por su siebel Id.
        /// </summary>
        /// <param name="userid">Id. de usuario (comercial) (siebel Id).</param>
        /// <returns>Respuesta 200 OK Clientes obtenidos con éxito.</returns>
        /// <response code="200">Clientes obtenidos con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("by-siebel-id")]
        [ProducesResponseType(typeof(GetCustomerResponse200Dto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomers([FromQuery][Required] string userid)
        {
            StatusResponseDto errorStatus;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/customers/GetCustomers: " + userid);

                if (string.IsNullOrEmpty(userid) || string.IsNullOrWhiteSpace(userid))
                {
                    return BadRequest();
                }
                else
                {
                    string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointGetCustomers");
                    GetCustomersResult dataCustomersModel = await _customerService.GetCustomersBySiebelId(endPoint, userid);

                    switch (dataCustomersModel.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(_mapper.Map<List<CustomerDto>>(dataCustomersModel.CustomerList));

                        case ResultStatus.NOT_FOUND:
                            return StatusCode(StatusCodes.Status404NotFound);

                        case ResultStatus.EXTERNAL_SERVICE_ERROR:
                            errorStatus = _mapper.Map<StatusResponseDto>(dataCustomersModel);
                            return StatusCode(StatusCodes.Status500InternalServerError, errorStatus);

                        default:
                            errorStatus = _mapper.Map<StatusResponseDto>(dataCustomersModel);
                            return StatusCode(StatusCodes.Status500InternalServerError, errorStatus);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite obtener los clientes del usuario comercial indicado por su id.
        /// </summary>
        /// <param name="commercialId">Id. del comercial.</param>
        /// <returns>Respuesta 200 OK Clientes obtenidos con éxito.</returns>
        /// <response code="200">Clientes obtenidos con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(GetCustomerResponse200Dto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomersByCommercialId([FromQuery][Required] string commercialId)
        {
            StatusResponseDto errorStatus;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/customers/GetCustomersByCommercialId: " + commercialId);

                if (string.IsNullOrEmpty(commercialId) || string.IsNullOrWhiteSpace(commercialId))
                {
                    return BadRequest();
                }
                else
                {
                    string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointGetCustomers");
                    var dataCustomersModel = await _customerService.GetCustomersByCommercialId(endPoint, commercialId);

                    switch (dataCustomersModel.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(_mapper.Map<List<CustomerDto>>(dataCustomersModel.CustomerList));

                        case ResultStatus.NOT_FOUND:
                            return StatusCode(StatusCodes.Status404NotFound);

                        case ResultStatus.EXTERNAL_SERVICE_ERROR:
                            errorStatus = _mapper.Map<StatusResponseDto>(dataCustomersModel);
                            return StatusCode(StatusCodes.Status500InternalServerError, errorStatus);

                        default:
                            errorStatus = _mapper.Map<StatusResponseDto>(dataCustomersModel);
                            return StatusCode(StatusCodes.Status500InternalServerError, errorStatus);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un comercial obtener la lista de clientes en base a la llamada iniciada previamente.
        /// </summary>
        /// <param name="id">SessionId del comerial.</param>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No existe la llamada con ese Id de sesion.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("session/{id}")]
        [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
        public IActionResult GetClientsBySession([FromRoute][Required] string id)
        {
            StatusResponseDto errorStatus;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/customers/GetClientsBySession: " + id);

                GetCustomersByCallResponseModel businessResult = new GetCustomersByCallResponseModel();
                GetCallIdBySessionIdResponseModel callModel = _callService.GetCallIdBySessionId(id);
                if (callModel != null && callModel.Status == ResultStatus.SUCCESS)
                {
                    businessResult = _customerService.GetCustomersByCallId(callModel.CallId);
                }
                else
                {
                    businessResult.Status = ResultStatus.NOT_FOUND;
                    businessResult.Message = callModel.Message;
                }

                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        List<CustomerDto> response = _mapper.Map<List<CustomerDto>>(businessResult.Customers);
                        return Ok(response);
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_FOUND:
                        errorStatus = _mapper.Map<StatusResponseDto>(businessResult);
                        return StatusCode(StatusCodes.Status404NotFound, errorStatus);

                    default:
                        errorStatus = _mapper.Map<StatusResponseDto>(businessResult);
                        return StatusCode(StatusCodes.Status500InternalServerError, errorStatus);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
