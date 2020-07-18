using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services
{
   /// <summary>
   /// Clase que implementa la interfaz <see cref="IBOAuthServices"/>.
   /// </summary>
   public class BOAuthServices : IBOAuthServices
   {
      private readonly IGenericRepository<Supervisors> _supervisorRepository;
      private readonly IGenericRepository<Sessions> _sessionRepository;
      private readonly IGenericRepository<LogsGenerals> _logsRepository;
      private readonly string _authKey;
      private readonly string _authIssuer;
      private readonly string _authAudience;

      /// <summary>
      /// Inicializa la instancia de la clase <see cref="BOAuthServices"/>.
      /// </summary>
      public BOAuthServices(IGenericRepository<Supervisors> supervisorRepository, IGenericRepository<Sessions> sessionRepository, IGenericRepository<LogsGenerals> logsRepository, string authKey, string authIssuer, string authAudience)
      {
         _supervisorRepository = supervisorRepository;
         _sessionRepository = sessionRepository;
         _logsRepository = logsRepository;
         _authKey = authKey;
         _authIssuer = authIssuer;
         _authAudience = authAudience;
      }

      /// <summary>
      /// Inicia el proceso de autenticación de un supervisor (usuario BackOffice)
      /// </summary>
      /// <param name="data">Modelo con los datos necesarios para la autenticación.</param>
      /// <returns>
      /// Modelo <see cref="PostBOSigninResponseModel"/> con los datos de la respuesta.
      /// </returns>
      /// <exception cref="ArgumentNullException">El parámetro 'data' no puede ser NULL.</exception>
      public PostBOSigninResponseModel Signin(PostBOSigninRequestModel data)
      {
         if (data == null)
         {
            throw new ArgumentNullException("El parámetro 'data' no puede ser NULL.");
         }

         var signinModel = new PostBOSigninResponseModel
         {
            status = ResultStatus.ACCESS_DENIED
         };

         try
         {
            Supervisors supervisor = _supervisorRepository.SingleOrDefault(item => item.Email == data.user);
            if (supervisor == null)
            {
               signinModel.status = ResultStatus.NOT_FOUND;
               return signinModel;
            }

            // validacion de password y estado de aprobacion
            var inHash = CreateHash(data.password);
            var dbHash = supervisor.Password;
            if (inHash != dbHash || supervisor.State == (int)SupervisorState.Pending)
            {
               return signinModel;
            }

            signinModel.status = ResultStatus.SUCCESS;
            signinModel.accessToken = GenerateToken(supervisor.Email, supervisor.State.ToString());
            InsertLogSignIn(supervisor, signinModel);
         }
         catch (ArgumentNullException e)
         {
            return new PostBOSigninResponseModel
            {
               status = ResultStatus.ERROR
            };
         }
         catch (ArgumentException e)
         {
            return new PostBOSigninResponseModel
            {
               status = ResultStatus.ERROR
            };
         }
         catch (Exception e)
         {
            return new PostBOSigninResponseModel
            {
               status = ResultStatus.ERROR
            };
         }

         return signinModel;
      }

      /// <summary>
      /// Genera un hash a partir de un string indicado por el parámetro <paramref name="str"/>
      /// </summary>
      /// <param name="str">String a codificar</param>
      /// <returns>
      /// String con el hash calculado
      /// </returns>
      private static string CreateHash(string str)
      {
         using (SHA256 sha256Hash = SHA256.Create())
         {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
               builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
         }
      }

      /// <summary>
      /// Crea un token el cual contiene el email del usuario.
      /// </summary>
      /// <param name="userEmail">Email del usuario.</param>
      /// <returns>
      /// String con el token generado.
      /// </returns>
      /// <exception cref="ArgumentNullException">El parámetro 'userEmail' no puede ser NULL.</exception>
      /// <exception cref="ArgumentException">El parámetro 'userEmail' no puede ser una cadena vacía ni puede contener solo espacios.</exception>
      private string GenerateToken(string userEmail, string state)
      {
         if (userEmail == null)
         {
            throw new ArgumentNullException("El parámetro 'userEmail' no puede ser NULL.");
         }

         if (string.IsNullOrWhiteSpace(userEmail))
         {
            throw new ArgumentException("El parámetro 'userEmail' no puede ser una cadena vacía ni puede contener solo espacios.");
         }

         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authKey));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

         var claims = new[]
         {
            new Claim(ClaimTypes.Name, userEmail),
            new Claim(ClaimTypes.Role, state)
         };

         var token = new JwtSecurityToken(
                            issuer: _authIssuer,
                            audience: _authAudience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(8760)),
                            signingCredentials: creds);
         var aux = new JwtSecurityTokenHandler();
         var aux2 = aux.WriteToken(token);
         return aux2;
      }

        public void InsertLogSignIn(Supervisors supervisor, PostBOSigninResponseModel signinModel)
        {
            if (supervisor != null)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Peticion Supervisor",
                    Description = "Inicio De Sesion",
                    HourLog = dtNow,
                    UserId = supervisor.Id,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            }
            else if (supervisor == null)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Respuesta Supervisor",
                    Description = "No se encuentra registrado",
                    HourLog = dtNow,
                    UserId = null,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();

            }            
            else if (signinModel.status == ResultStatus.SUCCESS)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Respuesta",
                    Description = "Inico De Sesion Exitoso",
                    HourLog = dtNow,
                    UserId = supervisor.Id,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            }
        }

    }
}
