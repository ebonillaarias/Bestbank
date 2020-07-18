using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
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
    /// Servicios Autenticación Backoffice.
    /// </summary>
    [Produces("application/json")]
    [Route("api/bo/auth")]
    [ApiController]
    public class BOAuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IBOAuthServices _boAuthService;
        private readonly ISecurityService _securityService;

        /// <summary>
        /// Inicializa la instancia de la clase <see cref="BOAuthController"/>.
        /// </summary>
        public BOAuthController(IMapper mapper, IConfiguration config, IBOAuthServices authService, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _boAuthService = authService;
            _securityService = securityService;
        }

        /// <summary>
        /// Permite a un usuario backoffice (supervisor) iniciar sesión.
        /// </summary>
        /// <param name="data">datos para hacer la conexión.</param>
        /// <returns>Respuesta 200 OK inicio de sesión con éxito.</returns>
        /// <response code="200">Inicio de sesión con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">Acceso denegado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PostBOSigninResponseDto), StatusCodes.Status200OK)]
        public IActionResult Signin([FromBody][Required] PostBOSigninRequestDto data)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/auth/Signin: " + JsonConvert.SerializeObject(data));

                if (string.IsNullOrWhiteSpace(data.user) || string.IsNullOrWhiteSpace(data.password))
                {
                    return BadRequest();
                }
                else
                {
                    PostBOSigninRequestModel request = _mapper.Map<PostBOSigninRequestModel>(data);
                    PostBOSigninResponseModel dataSigninModel = _boAuthService.Signin(request);
                    switch (dataSigninModel.status)
                    {
                        case ResultStatus.SUCCESS:
                            PostBOSigninResponseDto responseSignin = _mapper.Map<PostBOSigninResponseDto>(dataSigninModel);
                            return Ok(responseSignin);

                        case ResultStatus.NOT_FOUND:
                        case ResultStatus.ACCESS_DENIED:
                            return StatusCode(StatusCodes.Status401Unauthorized);

                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
