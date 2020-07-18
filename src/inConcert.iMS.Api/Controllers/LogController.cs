using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inConcert.iMS.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        public LogController(ISecurityService securityService) 
        {
            _securityService = securityService;
        }

        /// <summary>
        /// Permite guardar log sobre la tabla logsgeneral en bd.
        /// </summary>
        /// <param name="dataDtoIn">Datos que se deben filtrar.</param>
        /// <response code="200">Soliciutd OK.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("insert-log")]
        [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertLog([FromBody][Required] PostLogRequestDto data)
        {
            StatusResponseDto responseDto;

            try
            {
                responseDto = new StatusResponseDto();

                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = data.TypeLog,
                    Description = data.Description,
                    HourLog = dtNow,
                    UserId = data.UserId,
                    CallsId = data.CallsId

                };

                _securityService.InsertLog(LogPetition);

                responseDto.Status = "OK";
                return StatusCode(StatusCodes.Status200OK, responseDto);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

    }
}