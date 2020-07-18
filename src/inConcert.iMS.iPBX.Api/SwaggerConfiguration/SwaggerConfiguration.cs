using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.SwaggerConfiguration
{
    /// <summary>
    /// Configuración Swagger
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SwaggerConfiguration
    {
        /// <summary>
        /// <para>InConcert API 1.0.0</para>
        /// </summary>
        public const string EndPointDescription = "iPBX API 1.7.7";

        /// <summary>
        /// <para>/swagger/v1/swagger.json</para>
        /// </summary>
        public const string EndPointUrl = "/swagger/v1/swagger.json";

        /// <summary>
        /// <para>Best Bank - InConcert</para>
        /// </summary>
        public const string ContactName = "Best Bank - InConcert";

        /// <summary>
        /// </summary>
        public const string ContactUrl = "url del contacto";

        /// <summary>
        /// <para>v1</para>
        /// </summary>
        public const string DocNameV1 = "v1";

        /// <summary>
        /// <para>Best Bank - InConcert - API Mobile Server</para>
        /// </summary>
        public const string DocInfoTitle = "Best Bank - iPBX - API Server";

        /// <summary>
        /// <para>1.0.0</para>
        /// </summary>
        public const string DocInfoVersion = "1.7.7";

        /// <summary>
        /// <para>InConcert API - Gestor de peticiones en ASP.NET Core 3</para>
        /// </summary>
        public const string DocInfoDescription = "iPBX API - Gestor de peticiones en ASP.NET Core 3";
    }
}
