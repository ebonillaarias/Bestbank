using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.BusinessLogic.Services.Model;
using inConcert.iMS.Domain;
using inConcert.iMS.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace inConcert.iMS.Api.Controllers
{
    /// <summary>
    /// Servicios de clientes.
    /// </summary>
    [Produces("application/json")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuthServices _authService;
        private readonly INotifier _notifier;
        private readonly ISecurityService _securityService;
        public AuthController(IMapper mapper, IConfiguration config, IAuthServices authService, INotifier notifier, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _authService = authService;
            _notifier = notifier;
            _securityService = securityService;
        }

        /// <summary>
        /// Permite a un usuario iniciar sesion
        /// </summary>
        /// <param name="data">datos para hacer la conexión.</param>
        /// <returns>Respuesta 200 OK inicio de sesión con éxito.</returns>
        /// <response code="200">Inicio de sesión con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="403">Acceso no autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PostSigninResponseDto), StatusCodes.Status200OK)]
        public IActionResult SignIn([FromBody][Required] PostSigninRequestDto data)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/auth/SignIn: " + JsonConvert.SerializeObject(data));

                if (string.IsNullOrWhiteSpace(data.user) || string.IsNullOrWhiteSpace(data.firebase_token))
                {
                    return BadRequest();
                }
                else
                {
                    PostSigninRequestModel request = _mapper.Map<PostSigninRequestModel>(data);
                    PostSigninResponseModel dataSigninModel = _authService.SignIn(request);
                    switch (dataSigninModel.Status)
                    {
                        case ResultStatus.SUCCESS:
                            // Al comercial/appmobile se le devuelve la IP publica de la PBX
                            dataSigninModel.PbxHost = _config.GetValue<string>("PBXConnection:PublicServer");
                            PostSigninResponseDto responseSignin = _mapper.Map<PostSigninResponseDto>(dataSigninModel);
                            return Ok(responseSignin);
                        case ResultStatus.NOT_FOUND:
                            return BadRequest();
                        case ResultStatus.ACCESS_DENIED:
                            return StatusCode(StatusCodes.Status401Unauthorized);
                        case ResultStatus.NOT_AUTHORIZED:
                            return StatusCode(StatusCodes.Status403Forbidden);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un usuario comercial cerrar sesion.
        /// </summary>
        /// <param name="id">ID del commercial cuya sesión debe ser finalizada.</param>
        /// <response code="200">Sesión finalizada con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("sign-out/{id}")]
        public IActionResult SignOut([FromRoute][Required] int id)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/auth/SignOut: " + JsonConvert.SerializeObject(id));

                StatusResponseModel responseModel = _authService.SignOut(id);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok();
                    case ResultStatus.NOT_FOUND:
                        return BadRequest();
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un usuario indicar que aún está conectado
        /// </summary>
        /// <param name="id">ID del commercial cuya sesión debe mantenerse activa.</param>
        /// <returns>Respuesta 200 OK mantenimiento de sesión con éxito.</returns>
        /// <response code="200">Mantenimiento de sesión con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("keep-alive/{id}")]
        [ProducesResponseType(typeof(PostSigninResponseDto), StatusCodes.Status200OK)]
        public IActionResult KeepAlive([FromRoute][Required] int id)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                //_securityService.RequestLog("api/auth/KeepAlive: " + JsonConvert.SerializeObject(id));

                StatusResponseModel responseModel = _authService.KeepAlive(id);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok();
                    case ResultStatus.NOT_FOUND:
                        return StatusCode(StatusCodes.Status404NotFound, responseModel.Message);
                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite a un usuario comercial actualizar su contrasenia.
        /// </summary>
        /// <param name="data">Datos para actualizar contraseña.</param>
        /// <response code="200">Contraseña actualizada con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="409">Constraseña actual incorrecta.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Error con el servicio externo.</response>
        [HttpPatch("change-password")]
        [ProducesResponseType(typeof(PostUpdatePasswordResponse200Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult ChangePassword([FromBody][Required] PostUpdatePasswordRequestDto data)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/auth/ChangePassword: " + JsonConvert.SerializeObject(data));

                #region Validacion datos entrada
                bool error = false;
                responseDto = new StatusResponseDto();

                if (!error && string.IsNullOrWhiteSpace(data.Password))
                {
                    responseDto.Status = "PASSWORD_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "password");
                    error = true;
                }

                if (!error && string.IsNullOrWhiteSpace(data.NewPassword))
                {
                    responseDto.Status = "NEW_PASSWORD_NULL_OR_WHITESPACE";
                    responseDto.Message = string.Format("The '{0}' field is required.", "new_password");
                    error = true;
                }

                if (error)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, responseDto);
                }
                #endregion

                var changePasswordModel = _mapper.Map<PostUpdatePasswordRequestModel>(data);
                PostUpdatePasswordResponseModel responseModel = _authService.ChangePassword(changePasswordModel);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        PostUpdatePasswordResponse200Dto response200Dto = _mapper.Map<PostUpdatePasswordResponse200Dto>(responseModel);
                        return StatusCode(StatusCodes.Status200OK, response200Dto);

                    case ResultStatus.CONFLICT:
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status409Conflict, responseDto);

                    case ResultStatus.NOT_FOUND:
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status404NotFound, responseDto);

                    case ResultStatus.EXTERNAL_SERVICE_ERROR:
                    case ResultStatus.CANNOT_CONNECT_TO_PBX:
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, responseDto);

                    default:
                        responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
                }
            }
            catch (Exception e)
            {
                responseDto = new StatusResponseDto
                {
                    Status = "ERROR",
                    Message = "[AuthController-ChangePassword-Exception: " + e.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
            }
        }

        /// <summary>
        /// Permite a un usuario solicitar el envio de una contraseña nueva
        /// </summary>
        /// <param name="data">Datos para solicitar el reenvio de contraseña.</param>
        /// <returns>Respuesta 200 OK reenvio de contraseña con éxito.</returns>
        /// <response code="200">Envio de contraseña con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public IActionResult ForgotPassword([FromBody][Required] PostForgotPasswordRequestDto data)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/auth/ForgotPassword: " + JsonConvert.SerializeObject(data));

                if (string.IsNullOrWhiteSpace(data.User))
                {
                    return BadRequest();
                }
                else
                {
                    var request = _mapper.Map<PostForgotPasswordRequestModel>(data);
                    ForgotPasswordResponseModel responseModel = _authService.ForgotPassword(request);

                    switch (responseModel.Status)
                    {
                        case ResultStatus.SUCCESS:
                            _notifier.Notify(new EmailData(EmailType.CommercialPasswordNew, responseModel));
                            responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                            return StatusCode(StatusCodes.Status200OK, responseDto);
                        case ResultStatus.NOT_FOUND:
                        case ResultStatus.ACCESS_DENIED:
                            return StatusCode(StatusCodes.Status401Unauthorized);
                        case ResultStatus.CANNOT_CONNECT_TO_PBX:
                            return StatusCode(StatusCodes.Status503ServiceUnavailable);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Permite crear un Hash encriptado según una cadena indicada
        /// </summary>
        /// <param name="data">Cadena de texto a convertir.</param>
        /// <returns>Respuesta 200 OK cadena convertida con éxito.</returns>
        /// <response code="200">Cadena convertida con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("generate-hash")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
        public IActionResult GenerateHash([FromBody][Required] PostCreateHash data)
        {
            StatusResponseDto responseDto;
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                //_securityService.RequestLog("api/auth/GenerateHash: " + JsonConvert.SerializeObject(data));

                if (string.IsNullOrWhiteSpace(data.Data))
                {
                    return BadRequest();
                }
                else
                {
                    var request = _mapper.Map<String>(data.Data);
                    string response = Utils.Encrypt(request);
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        responseDto = new StatusResponseDto
                        {
                            Message = response,
                            Status = ResultStatus.SUCCESS.ToString()
                        };
                        return StatusCode(StatusCodes.Status200OK, responseDto);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError);

                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
