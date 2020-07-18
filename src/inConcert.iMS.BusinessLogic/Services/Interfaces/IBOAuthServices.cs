using inConcert.iMS.Domain;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
   /// <summary>
   /// Proporciona una interfaz de Autenticación
   /// para los usuarios de Backoffice.
   /// </summary>
   public interface IBOAuthServices
   {
      /// <summary>
      /// Inicia el proceso de autenticación de un supervisor (usuario BackOffice)
      /// </summary>
      /// <param name="data">Modelo con los datos necesarios para la autenticación.</param>
      /// <returns>
      /// Modelo <see cref="PostBOSigninResponseModel"/> con los datos de la respuesta.
      /// </returns>
      /// <exception cref="ArgumentNullException">El parámetro 'data' no puede ser NULL.</exception>
      PostBOSigninResponseModel Signin(PostBOSigninRequestModel data);
   }
}