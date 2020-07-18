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

namespace inConcert.iMS.Api.Controllers
{
    /// <summary>
    /// Servicios de comerciales.
    /// </summary>
    [Produces("application/json")]
    [Route("api/commercials")]
    [ApiController]
    public class CommercialsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ICommercialService _commercialService;
        private readonly ISecurityService _securityService;

        public CommercialsController(IMapper mapper, IConfiguration config, ICommercialService commercialService, ISecurityService securityService)
        {
            _mapper = mapper;
            _config = config;
            _commercialService = commercialService;
            _securityService = securityService;
        }

        /// <summary>
        /// Obtiene el listado de comerciales del sistema.
        /// </summary>
        /// <returns>Respuesta 200 OK Comerciales obtenidos con éxito.</returns>
        /// <response code="200">Comerciales obtenidos con éxito.</response>
        /// <response code="400">Datos de entrada incorrectos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CommercialDto>), StatusCodes.Status200OK)]
        public IActionResult GetCommercials()
        {
            try
            {
                /// <summary>
                /// Método para guardar request en logs
                /// </summary>
                _securityService.RequestLog("api/commercials/GetCommercials: " + "");

                GetCommercialsResponseModel responseModel = _commercialService.GetCommercials();
                switch (responseModel.Status)
                {
                    case ResultStatus.SUCCESS:
                        return Ok(_mapper.Map<List<CommercialDto>>(responseModel.Commercials));

                    case ResultStatus.BAD_REQUEST:
                        return BadRequest();

                    case ResultStatus.ACCESS_DENIED:
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    case ResultStatus.NOT_AUTHORIZED:
                        return StatusCode(StatusCodes.Status403Forbidden);

                    default:
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