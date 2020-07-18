using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.BusinessLogic.Services.Model;
using inConcert.iMS.DataAccess.Exceptions;
using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace inConcert.iMS.Api.Controllers
{
    /// <summary>
    /// Servicios de clientes.
    /// </summary>
    [Produces("application/json")]
    [Route("api/bo/commercials")]
    [ApiController]
    public class BOCommercialsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IBOCommercialsService _boCommercialService;
        private readonly INotifier _notifier;
        private readonly ISecurityService _securityService;

        public BOCommercialsController(IMapper mapper, IConfiguration config, IBOCommercialsService boCommercialService, INotifier notifier, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _boCommercialService = boCommercialService;
            _notifier = notifier;
            _securityService = securityService;
        }

        /// <summary>
        /// Permite a un supervisor buscar un listado de comerciales
        /// </summary>
        /// <param name="name">nombre del usuario (comercial) a filtrar.</param>
        /// <returns>Respuesta 200 OK inicio de sesión con éxito.</returns>
        /// <response code="200">Inicio de sesión con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BOCommercialDto>), StatusCodes.Status200OK)]
        public IActionResult GetCommercials([FromQuery] string name)
        {
            try
            {
                /// <summary>
                /// Metodo para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/commercials/GetCommercials: " + JsonConvert.SerializeObject(name));

                string strKeepAliveMaxTime = _config.GetValue<string>("KeepAlive:MaxTime");
                if (!uint.TryParse(strKeepAliveMaxTime, out uint uiKeepAliveMaxTime))
                {
                    uiKeepAliveMaxTime = 1; // valor por defecto
                }

                BOGetCommercialsResponseModel dataCommercialsModel = _boCommercialService.GetCommercials(name, uiKeepAliveMaxTime);
                switch (dataCommercialsModel.status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok(_mapper.Map<List<BOCommercialDto>>(dataCommercialsModel.commercials));
                    case ResultStatus.NOT_FOUND:
                        return NotFound();
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(401);
                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(403);
                    default:
                        return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor dar de alta un comercial
        /// </summary>
        /// <param name="data">datos para dar de alta al comercial</param>
        /// <response code="200">Comercial creado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="409">Conflicto al crear comercial.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Servicio no disponible.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status409Conflict)]
        public IActionResult CreateCommercial([FromBody][Required] PostBOCommercialRequestDto data)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Metodo para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/commercials/CreateCommercial: " + JsonConvert.SerializeObject(data));

                PostBOCommercialsRequestModel request = _mapper.Map<PostBOCommercialsRequestModel>(data);
                PostBOCommercialsResponseModel businessResponse = _boCommercialService.CreateCommercial(request);
                switch (businessResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        businessResponse.LinkAppAndroid = _config.GetValue<string>("AppAndroid:DownloadLink");
                        _notifier.Notify(new EmailData(EmailType.CommercialNew, businessResponse));
                        return Ok();

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_NULL:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_EMAIL:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_PEER:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_PBX:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_MOBILE:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_SIEBELID:
                    case ResultStatus.PEER_ALREADY_EXIST_IN_PBX:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status409Conflict, responseDto);

                    case ResultStatus.NOT_FOUND:
                        return NotFound();

                    case ResultStatus.CANNOT_CONNECT_TO_PBX:
                    case ResultStatus.EXTERNAL_SERVICE_ERROR:
                        responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, responseDto);

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, businessResponse.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor buscar un comercial en particular por id
        /// </summary>
        /// <param name="id">id del commercial a buscar.</param>
        /// <returns>Respuesta 200 OK se encontró el comercial buscado.</returns>
        /// <response code="200">Se encontró y devolvió el comercial buscado.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BOCommercialDto), StatusCodes.Status200OK)]
        public IActionResult GetCommercialsById(int id)
        {
            try
            {
                /// <summary>
                /// Metodo para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/commercials/GetCommercialsById: " + JsonConvert.SerializeObject(id));

                GetBOCommercialByIdResponseModel businessResult = _boCommercialService.GetCommercialById(id);
                switch (businessResult.status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok(_mapper.Map<BOCommercialDto>(businessResult.Commercial));
                    case ResultStatus.NOT_FOUND:
                        return NotFound();
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(401);
                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(403);
                    default:
                        return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Permite a un supervisor eliminar un comercial específico definido por su id
        /// </summary>
        /// <param id="id">id del comercial</param>
        /// <returns>Respuesta 200 OK inicio de sesión con éxito.</returns>
        /// <response code="200">Comercial eliminado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteCommercial([FromRoute][Required] int id)
        {
            try
            {
                /// <summary>
                /// Metodo para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/commercials/DeleteCommercial: " + JsonConvert.SerializeObject(id));

                StatusResponseModel deleteCommercialResponse = _boCommercialService.DeleteCommercial(id);
                switch (deleteCommercialResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok();

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_FOUND:
                        return NotFound();

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, deleteCommercialResponse.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite actualizar a un comercial
        /// </summary>
        /// <param name="id">id del comercial</param>
        /// <param name="data">Datos del comercial a actualizar</param>
        /// <returns>Respuesta 204 Sin contenido - Comercial actualizado correctamente.</returns>
        /// <response code="200">Comercial actualizado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="409">Datos duplicados.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Servicio no disponible.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult PutUpdateCommercial([FromRoute][Required] int id, [FromBody][Required] PutCommercialRequestDto data)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Metodo para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/commercials/PutUpdateCommercial: {id: " + JsonConvert.SerializeObject(id) + ", data: " + JsonConvert.SerializeObject(data) + "}");

                #region Validacion datos entrada
                bool error = false;
                responseDto = new StatusResponseDto();

                if (!error && (!ModelState.IsValid || id != data.CommercialId))
                {
                    responseDto.Status = "PAYLOAD_INVALID";
                    responseDto.Message = "The payload is invalid or check the commercial id on route and payload.";
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.SiebelId))
                {
                    responseDto.Status = "SIEBEL_ID_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "SiebelId");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.PBXPhoneNumber))
                {
                    responseDto.Status = "PBX_NUMBER_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "PBX Number");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.MobilePhoneNumber))
                {
                    responseDto.Status = "MOBILE_NUMBER_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "Mobile Number");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.CommercialName))
                {
                    responseDto.Status = "NAME_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "Name");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.CommercialEmail))
                {
                    responseDto.Status = "EMAIL_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "Email");
                    error = true;
                }

                if (!error && data.CommercialEmail.Length > Constants.Commercials_Email_MaxLen)
                {
                    responseDto.Status = "EMAIL_MAX_LEN";
                    responseDto.Message = string.Format("The maximum length of the field '{0}' is {1}.", "Email", Constants.Commercials_Email_MaxLen);
                    error = true;
                }

                if (!error && data.CommercialName.Length > Constants.Commercials_Name_MaxLen)
                {
                    responseDto.Status = "NAME_MAX_LEN";
                    responseDto.Message = string.Format("The maximum length of the field '{0}' is {1}.", "Name", Constants.Commercials_Name_MaxLen);
                    error = true;
                }

                if (!error && data.SiebelId.Length > Constants.Commercials_SiebelId_MaxLen)
                {
                    responseDto.Status = "SIEBEL_ID_MAX_LEN";
                    responseDto.Message = string.Format("The maximum length of the field '{0}' is {1}.", "Siebel Id", Constants.Commercials_SiebelId_MaxLen);
                    error = true;
                }

                if (!error && data.PBXPhoneNumber.Length > Constants.Commercials_PBXNumber_MaxLen)
                {
                    responseDto.Status = "PBX_NUMBER_MAX_LEN";
                    responseDto.Message = string.Format("The maximum length of the field '{0}' is {1}.", "PBX Number", Constants.Commercials_PBXNumber_MaxLen);
                    error = true;
                }

                if (!error && data.MobilePhoneNumber.Length > Constants.Commercials_MobilePhoneNumber_MaxLen)
                {
                    responseDto.Status = "MOBILE_NUMBER_MAX_LEN";
                    responseDto.Message = string.Format("The maximum length of the field '{0}' is {1}.", "Mobile Number", Constants.Commercials_MobilePhoneNumber_MaxLen);
                    error = true;
                }

                if (error)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                }
                #endregion

                var commercialModel = _mapper.Map<PutCommercialRequestModel>(data);
                StatusResponseModel businessResult = _boCommercialService.UpdateCommercial(id, commercialModel);
                responseDto = _mapper.Map<StatusResponseDto>(businessResult);
                switch (businessResult.Status)
                {
                    case ResultStatus.SUCCESS:
                        return StatusCode(StatusCodes.Status200OK, responseDto);

                    case ResultStatus.CANNOT_CONNECT_TO_PBX:
                    case ResultStatus.EXTERNAL_SERVICE_ERROR:
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, responseDto);

                    case ResultStatus.BAD_REQUEST:
                        return StatusCode(StatusCodes.Status400BadRequest, responseDto);

                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_EMAIL:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_PEER:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_PBX:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_MOBILE:
                    case ResultStatus.COMMERCIAL_ROW_DUPLICATE_SIEBELID:
                        return StatusCode(StatusCodes.Status409Conflict, responseDto);

                    case ResultStatus.NOT_FOUND:
                        return StatusCode(StatusCodes.Status404NotFound, responseDto);

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                }
            }
            catch (Exception e)
            {
                responseDto = new StatusResponseDto
                {
                    Status = "ERROR",
                    Message = "[BOCommercialsController-PutUpdateCommercial-Exception: " + e.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
            }
        }
    }
}
