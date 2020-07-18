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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;

namespace inConcert.iMS.Api.Controllers
{
    /// <summary>
    /// Servicios de Supervisores.
    /// </summary>
    [Produces("application/json")]
    [Route("api/bo/supervisors")]
    [ApiController]
    [Authorize(Roles = "2")]
    public class BOSupervisorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IBOSupervisorsService _boSupervisorService;
        private readonly INotifier _notifier;
        private readonly ISecurityService _securityService;

        public BOSupervisorsController(IMapper mapper, IConfiguration config, IBOSupervisorsService boSupervisorService, INotifier notifier, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _boSupervisorService = boSupervisorService;
            _notifier = notifier;
            _securityService = securityService;
        }

        /// <summary>
        /// Retorna una listado de supervisores (<see cref="BOSupervisorDto"/>) cuyo nombre o email contenga el valor indicado por
        /// el parametro <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Filtro (por nombre) para buscar supervisores.</param>
        /// <response code="200">Solicitud OK. Se retorna lista de supervisores.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BOSupervisorDto>), StatusCodes.Status200OK)]
        public IActionResult GetSupervisors([FromQuery] string name)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/supervisors/GetSupervisors: " + JsonConvert.SerializeObject(name));

                bool withSuperuser = false;
                BOGetSupervisorsResponseModel responseModel = _boSupervisorService.GetSupervisors(name, withSuperuser);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        if (withSuperuser)
                        {
                            string userLogged = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                            responseModel.Supervisors.RemoveAll(s => string.Equals(s.Email, userLogged));
                        }
                        return Ok(_mapper.Map<List<BOSupervisorDto>>(responseModel.Supervisors));

                    case ResultStatus.BAD_REQUEST:
                        // Datos entrada incorrectos
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        // No autenticado
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        // Autenticado, pero sin permisos
                        return StatusCode(StatusCodes.Status403Forbidden);

                    default:
                        return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Alta de Supervisor.
        /// </summary>
        /// <param name="data">Datos para dar de alta al supervisor.</param>
        /// <response code="200">Supervisor creado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="409">Conflicto al crear supervisor.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status409Conflict)]
        [AllowAnonymous]
        public IActionResult CreateSupervisor([FromBody][Required] PostBOSupervisorRequestDto data)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/supervisors/CreateSupervisor: " + JsonConvert.SerializeObject(data));

                PostBOSupervisorRequestModel request = _mapper.Map<PostBOSupervisorRequestModel>(data);
                PostBOSupervisorResponseModel businessResponse = _boSupervisorService.CreateSupervisor(request);

                switch (businessResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok();

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_NULL:
                    case ResultStatus.SUPERVISOR_ROW_DUPLICATE:
                    case ResultStatus.SUPERVISOR_ROW_DUPLICATE_EMAIL:
                        StatusResponseDto responseDto = _mapper.Map<StatusResponseDto>(businessResponse);
                        return StatusCode(StatusCodes.Status409Conflict, responseDto);

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
        /// Aprobacion de Alta Supervisor.
        /// </summary>
        /// <param name="id">ID del supervisor a aprobar.</param>
        /// <response code="200">Supervisor aprobado/denegado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="404">No encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public IActionResult PatchUpdateSupervisor([FromRoute][Required] int id, [FromBody][Required] PatchBOSupervisorsRequestDto data)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/supervisors/PatchUpdateSupervisor: {id: " + JsonConvert.SerializeObject(id) + ", data: " + JsonConvert.SerializeObject(data) + "}");

                if (!ModelState.IsValid || id != data.Id)
                {
                    return BadRequest("The payload is invalid or check the supervisor id on route and payload");
                }

                PatchBOSupervisorsRequestModel request = _mapper.Map<PatchBOSupervisorsRequestModel>(data);
                StatusResponseModel businessResponse = _boSupervisorService.ApproveSupervisor(request);
                switch (businessResponse.Status)
                {
                    case ResultStatus.SUCCESS:
                        _notifier.Notify(new EmailData(EmailType.SupervisorState, request));
                        return Ok();

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.NOT_FOUND:
                        return StatusCode(StatusCodes.Status404NotFound);

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
        /// Permite a un supervisor SUPERUSUARIO eliminar otro supervisor, tenga o no 
        /// aprobacion pendiente.
        /// </summary>
        /// <param name="id">ID del supervisor</param>
        /// <returns>Respuesta 200 OK inicio de sesión con éxito.</returns>
        /// <response code="200">Supervisor eliminado correctamente.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="409">Conflicto al realizar la solicitud.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status409Conflict)]
        public IActionResult DeleteSupervisor([FromRoute][Required] int id)
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/bo/supervisors/DeleteSupervisor: " + JsonConvert.SerializeObject(id));

                StatusResponseModel responseModel = _boSupervisorService.DeleteSupervisor(id);
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok();

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    case ResultStatus.CONFLICT:
                        StatusResponseDto responseDto = _mapper.Map<StatusResponseDto>(responseModel);
                        return StatusCode(StatusCodes.Status409Conflict, responseDto);

                    case ResultStatus.NOT_FOUND:
                        return NotFound();

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, responseModel.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }

        }
    }
}