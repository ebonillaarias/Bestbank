using System.Diagnostics.CodeAnalysis;

namespace inConcert.iMS.Api.DataTransferObjects
{
    [ExcludeFromCodeCoverage]
    public class PostLoginResponse200Dto
    {
        /// <summary>
        /// El token contiene email y user id del usuario.
        /// </summary>
        public string Access_Token { get; set; }
    }
}
