using System.Diagnostics.CodeAnalysis;

namespace inConcert.iMS.Api.DataTransferObjects
{
    [ExcludeFromCodeCoverage]
    public class PostLoginRequestDto
    {
        /// <summary>
        /// Email del usuario
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Password { get; set; }
    }
}
