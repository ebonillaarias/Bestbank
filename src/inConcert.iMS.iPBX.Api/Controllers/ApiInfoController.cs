using System;
using System.Diagnostics;
using inConcert.iMS.iPBX.Api.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inConcert.iMS.iPBX.Api.Controllers
{
   [Produces("application/json")]
   [Route("api/info")]
   [ApiController]
   public class ApiInfoController : ControllerBase
    {
      /// <summary>
      /// Retorna la version de la API.
      /// </summary>
      /// <response code="200">String con versión (x.y.z) de API.</response>
      /// <response code="500">Error al obtener la versión.</response>
      [HttpGet("version")]
      [AllowAnonymous]
      [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(StatusResponseDto), StatusCodes.Status500InternalServerError)]
      public IActionResult GetApiVersion()
      {
         try
         {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;

            return StatusCode(StatusCodes.Status200OK, version);
         }
         catch (Exception e)
         {
            StatusResponseDto responseDto = new StatusResponseDto
            {
               Status = "ERROR",
               Message = "[ApiInfoController-GetApiVersion-Exception: " + e.Message
            };
            return StatusCode(StatusCodes.Status500InternalServerError, responseDto);
         }
      }
   }
}