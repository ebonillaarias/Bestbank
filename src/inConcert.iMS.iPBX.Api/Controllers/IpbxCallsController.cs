using AutoMapper;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using inConcert.iMS.iPBX.Api.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.Controllers
{
    /// <summary>
    /// Servicios para gestionar una llamada desde la PBX.
    /// </summary>
    [Produces("application/json")]
    [Route("api/ipbxcalls/")]
    [ApiController]
    public class IpbxCallsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IBOCallsService _boCallsService;
        private readonly IBOCommercialsService _boCommercialService;
        private readonly ISecurityService _securityService;
        public IpbxCallsController(IMapper mapper, IConfiguration config, IBOCallsService boCallsService, IBOCommercialsService boCommercialService, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _boCallsService = boCallsService;
            _boCommercialService = boCommercialService;
            _securityService = securityService;
        }

        /// <summary>
        /// La PBX notifica a iMS el inicio de una llamada entrante con este método.
        /// </summary>
        /// <param name="CallerId">Teléfono llamante. Es el número de teléfono de un cliente.</param>
        /// <param name="CalledId">Teléfono llamado.</param>
        /// <returns>Respuesta 200 OK Notificación de llamada entrante satisfactoria.</returns>
        /// <response code="200">Notificación de llamada entrante satisfactoria.</response>
        [HttpGet("StartInboundCall")]
        [ProducesResponseType(typeof(ResponseStartInboundCallDto), StatusCodes.Status200OK)]
        public async System.Threading.Tasks.Task<IActionResult> StartInboundCall([FromQuery][Required] string CallerId, [FromQuery][Required] string CalledId)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/StartInboundCall: {CallerId: " + CallerId + ", CalledId: " + CalledId + "}");

                if (string.IsNullOrWhiteSpace(CallerId) || string.IsNullOrWhiteSpace(CalledId))
                {
                    return Ok(new ResponseStartInboundCallDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                else
                {
                    string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointNewCall");
                    var res = await _boCallsService.StartInboundCall(endPoint, new PostCallsStartInboundCallRequestModel()
                    {
                        CallerId = CallerId,
                        CalledId = CalledId
                    }, bool.Parse(_config.GetSection("iPBXConfiguration:FilterGetCallsByConnected").Value));

                    switch (res.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(new ResponseStartInboundCallDto
                            {
                                Status = "Ok",
                                Message = "Success",
                                CallId = res.CallId.ToString(),
                                PeerList = res.PeerList.Count > 1
                               ? res.PeerList.First().ToString() + res.PeerList.Skip(1).Aggregate("", (total, next) => total + "-" + next)
                               : (res.PeerList.Count > 0
                                  ? res.PeerList.First().ToString()
                                  : "")
                            });

                        case ResultStatus.BAD_REQUEST:
                            return Ok(new ResponseStartInboundCallDto
                            {
                                Status = "BadRequest",
                                Message = "Datos de entrada incorrectos."
                            });

                        case ResultStatus.NOT_FOUND:
                            return Ok(new ResponseStartInboundCallDto
                            {
                                Status = "NotFound",
                                Message = res.Message
                            });

                        default:
                            return Ok(new ResponseStartInboundCallDto
                            {
                                Status = "Error",
                                Message = "Error en el servicio."
                            });
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new ResponseStartInboundCallDto
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// La PBX notifica a iMS el inicio de una llamada saliente con este método.
        /// </summary>
        /// <param name="Peer">Es el Peer del comercial que inicia la llamada.</param>
        /// <param name="CalledId">Teléfono llamado.</param>
        /// <returns>Respuesta 200 OK Notificación de llamada saliente satisfactoria.</returns>
        /// <response code="200">Notificación de llamada saliente satisfactoria.</response>
        [HttpGet("StartOutboundCall")]
        [ProducesResponseType(typeof(ResponseStartOutboundCallDto), StatusCodes.Status200OK)]
        public async System.Threading.Tasks.Task<IActionResult> StartOutboundCall([FromQuery][Required] string Peer, [FromQuery][Required] string CalledId)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/StartOutboundCall: {Peer: " + Peer + ", CalledId: " + CalledId + "}");

                if (string.IsNullOrWhiteSpace(Peer) || string.IsNullOrWhiteSpace(CalledId))
                {
                    return Ok(new ResponseStartOutboundCallDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                else
                {
                    string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointNewCall");
                    PostCallsStartOutboundCallResponseModel res = await _boCallsService.StartOutboundCall(endPoint, new PostCallsStartOutboundCallRequestModel()
                    {
                        Peer = Peer,
                        CalledId = CalledId
                    });

                    switch (res.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(new ResponseStartOutboundCallDto
                            {
                                CallId = res.CallId.ToString(),
                                Message = "Success",
                                Status = "Ok",
                                PBXPhoneNumber = res.PBXPhoneNumber,
                                MobilePhoneNumber = res.MobilePhoneNumber
                            });

                        case ResultStatus.BAD_REQUEST:
                            return Ok(new ResponseStartOutboundCallDto
                            {
                                Status = "BadRequest",
                                Message = "Datos de entrada incorrectos."
                            });

                        case ResultStatus.NOT_FOUND:
                            return Ok(new ResponseStartOutboundCallDto
                            {
                                Status = "NotFound",
                                Message = res.Message
                            });

                        default:
                            return Ok(new ResponseStartOutboundCallDto
                            {
                                Status = "Error",
                                Message = "Error en el servicio."
                            });
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new ResponseStartOutboundCallDto
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// La PBX notifica a iMS el fin de una llamada con este método.
        /// </summary>
        /// <param name="callId"> Identificador único de llamada.</param>
        /// <returns>Respuesta 200 OK Notificación de fin de llamada.</returns>
        /// <response code="200">Notificación de fin de llamada satisfactoria.</response>
        [HttpGet("EndCall")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> EndCall([FromQuery][Required] string callId)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/EndCall: {callId: " + callId + "}");

                if (string.IsNullOrWhiteSpace(callId) || !int.TryParse(callId, out int pCallId))
                {
                    return Ok(new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                string endPoint = _config.GetValue<string>("ExternalServicesHosts:Siebel:EndPointEndCall");
                StatusResponseModel resUpdTable = await _boCallsService.EndCallUpdateDBState(pCallId, endPoint);
                switch (resUpdTable.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Ok",
                            Message = "Success"
                        });

                    case ResultStatus.NOT_FOUND:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "NotFound",
                            Message = resUpdTable.Message
                        });

                    case ResultStatus.ERROR:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Error",
                            Message = resUpdTable.Message
                        });
                    case ResultStatus.EXTERNAL_SERVICE_ERROR:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Error",
                            Message = resUpdTable.Message
                        });

                    default:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Error",
                            Message = "Error en el servicio."
                        });
                }
            }
            catch (FormatException e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "BadRequest",
                    Message = "Formato de datos de entrada incorrecto."
                });
            }
            catch (Exception e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// La PBX notifica a iMS el inicio de una parte de una llamada.
        /// </summary>
        /// <param name="CallId"> Identificador único de llamada.</param>
        /// <param name="CallPartNumber"> Número secuencial de cada parte de una llamada, comenzando en 1.</param>
        /// <param name="Peer"> Peer al que va asociada esta parte de la llamada.</param>
        /// <param name="OrigChannel"> Identificador de canal SIP de la llamada.</param>
        /// <param name="RedirectChannel"> Nombre del canal SIP a utilizar como parámetro RedirectChannel .</param>
        /// <returns>Respuesta 200 OK Notificación de inicio de una parte de llamada.</returns>
        /// <response code="200">Notificación de fin de una parte de llamada satisfactoria.</response>
        [HttpGet("StartCallPart")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartCallPart([FromQuery][Required] string CallId, [FromQuery][Required] string CallPartNumber,
            [FromQuery][Required] string Peer, [FromQuery][Required] string OrigChannel, [FromQuery][Required] string RedirectChannel)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/StartCallPart: {CallId: " + CallId + ", CallPartNumber: " + CallPartNumber + ", Peer: " + Peer + ", OrigChannel: " + OrigChannel + ", RedirectChannel: " + RedirectChannel + "}");

                if (string.IsNullOrWhiteSpace(CallId)
                    || string.IsNullOrWhiteSpace(Peer)
                    || string.IsNullOrWhiteSpace(CallPartNumber)
                    || string.IsNullOrWhiteSpace(OrigChannel)
                    || string.IsNullOrWhiteSpace(RedirectChannel))
                {
                    return Ok(new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                else
                {
                    var getCommercialByPeerResult = _boCommercialService.GetCommercialByPeer(int.Parse(Peer));

                    if (getCommercialByPeerResult.status == ResultStatus.NOT_FOUND)
                    {
                        return Ok(new StatusResponseDto()
                        {
                            Status = "NotFound",
                            Message = "No se encuentra Comercial con Peer " + Peer + "."
                        });
                    }

                    if (getCommercialByPeerResult.status == ResultStatus.ERROR)
                    {
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Error",
                            Message = "Error en el servicio 'GetCommercialByPeer'."
                        });
                    }

                    string endPoint = _config.GetValue<string>("ExternalServicesHosts:FCM:EndPointSend");
                    StatusResponseModel serviceResult = await _boCallsService.StartCallPart(endPoint, new PostCallsStartCallPartRequestModel()
                    {
                        CallId = int.Parse(CallId),
                        CallPartNumber = CallPartNumber,
                        Peer = int.Parse(Peer),
                        CommercialId = getCommercialByPeerResult.Commercial.CommercialId,
                        CommercialName = getCommercialByPeerResult.Commercial.CommercialName,
                        OrigChannel = OrigChannel,
                        RedirectChannel = RedirectChannel,
                        StartDate = DateTimeOffset.UtcNow
                    });

                    switch (serviceResult.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(new StatusResponseDto
                            {
                                Status = "Ok",
                                Message = "Success"
                            });

                        case ResultStatus.BAD_REQUEST:
                            return Ok(new StatusResponseDto
                            {
                                Status = "BadRequest",
                                Message = "Datos de entrada incorrectos."
                            });

                        default:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "Error",
                                Message = serviceResult.Message
                            });
                    }
                }
            }
            catch (FormatException e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "BadRequest",
                    Message = "Formato de datos de entrada incorrecto."
                });
            }
            catch (Exception e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// La PBX notifica a iMS que debe actualizar el RedirectChannel asociado a una parte de una llamada.
        /// </summary>
        /// <param name="CallId"> Identificador único de llamada.</param>
        /// <param name="CallPartNumber"> Número secuencial de cada parte de una llamada, comenzando en 1.</param>
        /// <param name="RedirectChannel"> Nombre del canal SIP a utilizar como parámetro RedirectChannel .</param>
        /// <returns>Respuesta 200 OK Notificación de actualizar el RedirectChannel.</returns>
        /// <response code="200">Notificación de actualizar el RedirectChannel satisfactoria.</response>
        [HttpGet("UpdCallPartRedirectChannel")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public IActionResult UpdCallPartRedirectChannel([FromQuery][Required] string CallId, [FromQuery][Required] string CallPartNumber, [FromQuery][Required] string RedirectChannel)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/UpdCallPartRedirectChannel: {CallId: " + CallId + ", CallPartNumber: " + CallPartNumber + ", RedirectChannel: " + RedirectChannel + "}");

                if (string.IsNullOrWhiteSpace(CallId) || string.IsNullOrWhiteSpace(CallPartNumber) || string.IsNullOrWhiteSpace(RedirectChannel))
                {
                    return Ok(new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                else
                {
                    var serviceResult = _boCallsService.UpdateCallPartRedirectChannel(new PostCallsUpdateCallPartRedirectChannelRequestModel()
                    {
                        CallId = int.Parse(CallId),
                        CallPartNumber = CallPartNumber,
                        RedirectChannel = RedirectChannel
                    });

                    switch (serviceResult.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "Ok",
                                Message = "Success"
                            });

                        case ResultStatus.BAD_REQUEST:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "BadRequest",
                                Message = "Datos de entrada incorrectos."
                            });

                        case ResultStatus.NOT_FOUND:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "NotFound",
                                Message = serviceResult.Message
                            });

                        default:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "Error",
                                Message = "Error en el servicio."
                            });
                    }
                }
            }
            catch (FormatException e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "BadRequest",
                    Message = "Formato de datos de entrada incorrecto."
                });
            }
            catch (Exception e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// La PBX notifica a iMS el fin de una parte de una llamada.
        /// </summary>
        /// <param name="CallId"> Identificador único de llamada.</param>
        /// <param name="CallPartNumber"> Número secuencial de cada parte de una llamada, comenzando en 1.</param>
        /// <param name="CallResult"> Posibles valores “NoAnswer”, “Rejected”, “Completed” y “Transferred”.</param>
        /// <param name="RejectionReason">  Motivo asociado al rechazo de la llamada. Aplica para CallResult=Rejected.Vacío si no aplica.</param>
        /// <param name="CallPartEndedBy"> Posibles valores “Comercial” y “Cliente” .</param>
        /// <returns>Respuesta 200 OK Notificación de fin de una parte de llamada.</returns>
        /// <response code="200">Notificación de fin de una parte de llamada satisfactoria.</response>
        [HttpGet("EndCallPart")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public IActionResult EndCallPart([FromQuery][Required] string CallId, [FromQuery][Required] string CallPartNumber, [FromQuery][Required] string CallResult, [FromQuery][Required] string CallPartEndedBy, [FromQuery] string RejectionReason)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/EndCallPart: {CallId: " + CallId + ", CallPartNumber: " + CallPartNumber + ", CallResult: " + CallResult + ", CallPartEndedBy: " + CallPartEndedBy + ", RejectionReason: " + RejectionReason + "}");

                if (string.IsNullOrWhiteSpace(CallId)
                    || string.IsNullOrWhiteSpace(CallPartNumber)
                    || string.IsNullOrWhiteSpace(CallResult)
                    || string.IsNullOrWhiteSpace(CallPartEndedBy)
                    || (!string.IsNullOrWhiteSpace(CallResult) && CallResult == "Rejected" && string.IsNullOrWhiteSpace(RejectionReason))
                    )
                {
                    return Ok(new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }
                else
                {
                    var serviceResult = _boCallsService.UpdateCallPartEnd(new PostCallsEndCallPartRequestModel()
                    {
                        CallId = int.Parse(CallId),
                        CallPartNumber = CallPartNumber,
                        CallResult = CallResult,
                        CallPartEndedBy = CallPartEndedBy,
                        RejectionReason = RejectionReason
                    });

                    switch (serviceResult.Status)
                    {
                        case ResultStatus.SUCCESS:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "Ok",
                                Message = "Success"
                            });

                        case ResultStatus.BAD_REQUEST:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "BadRequest",
                                Message = "Datos de entrada incorrectos."
                            });

                        case ResultStatus.NOT_FOUND:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "BadRequest",
                                Message = serviceResult.Message
                            });

                        default:
                            return Ok(new StatusResponseDto()
                            {
                                Status = "Error",
                                Message = "Error en el servicio."
                            });
                    }
                }
            }
            catch (FormatException e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "BadRequest",
                    Message = "Formato de datos de entrada incorrecto."
                });
            }
            catch (Exception e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// Permite subir a iMS el fichero de audio que contiene la grabación de una parte de la llamada.
        /// </summary>
        /// <param name="dataIn"> Datos para subir el fichero de audio.</param>
        /// <returns>Respuesta 200 OK Notificación ya subido el fichero de audio.</returns>
        /// <response code="200">Notificación satisfactoria de subida de fichero de audio.</response>
        [HttpPost("UploadCallPartFile")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public IActionResult UploadCallPartFile([FromForm][Required] RequestUploadCallPartFileDto dataIn)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/ipbxcalls/UploadCallPartFile: " + JsonConvert.SerializeObject(dataIn));

                if (!ModelState.IsValid || dataIn == null || dataIn.FileContent == null ||
                    string.IsNullOrWhiteSpace(dataIn.CallId) || string.IsNullOrWhiteSpace(dataIn.CallPartNumber))
                {
                    return Ok(new StatusResponseDto
                    {
                        Status = "BadRequest",
                        Message = "Datos de entrada incorrectos."
                    });
                }

                string uploadDir = _config.GetValue<string>("FileServer:Dir").Trim();
                if (string.IsNullOrEmpty(uploadDir))
                {
                    return Ok(new StatusResponseDto()
                    {
                        Status = "Error",
                        Message = "En archivo appsettings no existe variable 'FileServer:Dir' o la misma es vacía."
                    });
                }
                PostCallsUploadCallPartFileRequestModel request = new PostCallsUploadCallPartFileRequestModel()
                {
                    CallId = dataIn.CallId,
                    CallPartNumber = dataIn.CallPartNumber,
                    PathToUpload = string.Format("{0}", uploadDir),
                    FileContent = dataIn.FileContent
                };

                // subida de archivo
                List<string> allowExtentions = _config.GetValue<string>("FileServer:FileType").Split(',').Select(s => s.Trim()).ToList();
                allowExtentions.Remove("");
                StatusResponseModel resUpdFile = _boCallsService.UploadCallPartFile_UploadFile(request, allowExtentions);
                switch (resUpdFile.Status)
                {
                    case ResultStatus.SUCCESS:
                        // continuo con la actualizacion de la bd
                        break;

                    case ResultStatus.BAD_REQUEST:
                    case ResultStatus.ERROR:
                    default:
                        StatusResponseDto responseDto = _mapper.Map<StatusResponseDto>(resUpdFile);
                        return Ok(responseDto);
                }

                // actualizo tabla CallParts
                StatusResponseModel resUpdTable = _boCallsService.UploadCallPartFile_UpdateTable(request);
                switch (resUpdTable.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok(new StatusResponseDto()
                        {
                            Status = "Ok",
                            Message = "Success"
                        });

                    case ResultStatus.BAD_REQUEST:
                    case ResultStatus.ERROR:
                    case ResultStatus.NOT_FOUND:
                    default:
                        StatusResponseDto responseDto = _mapper.Map<StatusResponseDto>(resUpdTable);
                        return Ok(responseDto);
                }
            }
            catch (Exception e)
            {
                return Ok(new StatusResponseDto()
                {
                    Status = "Error",
                    Message = e.Message
                });
            }
        }

    }
}
