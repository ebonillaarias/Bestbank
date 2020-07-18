using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.BusinessLogic.Services.Model;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using inConcert.iMS.ServiceAgent.Exceptions;
using inConcert.iMS.ServiceAgent.PBX;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace inConcert.iMS.BusinessLogic.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IGenericRepository<Commercials> _commercialRepository;
        private readonly IGenericRepository<Sessions> _sessionRepository;
        private readonly IGenericRepository<LogsGenerals> _logsRepository;
        private readonly ISecurityService _securityService;
        //private readonly ILogger<AuthServices> _log;
        private readonly string _authKey;
        private readonly string _authIssuer;
        private readonly string _authAudience;


        /// <summary>
        /// Indica si se debe realizar conexion con la PBX
        /// </summary>
        private readonly bool _pbxConnect;
        private readonly string _pbxServer;
        private readonly string _pbxUser;
        private readonly string _pbxPwd;

        public AuthServices(IGenericRepository<Commercials> commercialRepository, IGenericRepository<Sessions> sessionRepository,IGenericRepository<LogsGenerals> logsRepository, ISecurityService securityService,string authKey, string authIssuer, string authAudience,
           string pbxServer, string pbxUser, string pbxPwd, string pbxConnect)
        {
            _commercialRepository = commercialRepository;
            _sessionRepository = sessionRepository;
            _logsRepository = logsRepository;
            _securityService = securityService;
            //_log = log;
            _authKey = authKey;
            _authIssuer = authIssuer;
            _authAudience = authAudience;


            _pbxServer = pbxServer;
            _pbxUser = pbxUser;
            _pbxPwd = pbxPwd;

            _pbxConnect = false;
            if (Int32.TryParse(pbxConnect, out int _aux))
            {
                _pbxConnect = _aux == 1 ? true : false;
            }
        }

        public PostSigninResponseModel SignIn(PostSigninRequestModel data)
        {
            var signinModel = new PostSigninResponseModel
            {
                Status = ResultStatus.ACCESS_DENIED
            };

            var commercial = new Commercials();
            var currentSession = new Sessions();
                       
            try
            {
                commercial = _commercialRepository.SingleOrDefault(item => item.Email.Equals(data.user, StringComparison.InvariantCultureIgnoreCase));
                
                if (commercial == null)                
                {                    
                    signinModel.Status = ResultStatus.NOT_FOUND;
                    InsertLogSignIn(currentSession, commercial, signinModel);
                    return signinModel;                    
                }
                else if (!commercial.Active)
                {
                    signinModel.Status = ResultStatus.NOT_AUTHORIZED;
                    return signinModel;
                }

                var inHash = _securityService.HashData(data.password);
                var dbHash = commercial.Password;
                if (inHash != dbHash)
                {
                    return signinModel;
                }

                string sessionId = CreateSessionId();
                DateTime dtNow = DateTime.UtcNow;
                currentSession = _sessionRepository.SingleOrDefault(item => item.CommercialId == commercial.Id);
                if (currentSession == null)
                {
                    Sessions sessionModel = new Sessions
                    {
                        IdExternal = sessionId,
                        CommercialId = commercial.Id,
                        StartDate = dtNow,
                        LastKeepAlive = dtNow,
                        FirebaseToken = data.firebase_token
                    };
                    _sessionRepository.Insert(sessionModel);
                    InsertLogSignIn(currentSession, commercial, signinModel);
                }
                else
                {
                    currentSession.StartDate = dtNow;
                    currentSession.LastKeepAlive = dtNow;
                    currentSession.EndDate = null;
                    currentSession.IdExternal = sessionId;
                    currentSession.FirebaseToken = data.firebase_token;
                }
                _sessionRepository.Save();
                InsertLogSignIn(currentSession,commercial, signinModel);

                signinModel.SessionId = sessionId;
                signinModel.Peer = commercial.Peer;
                signinModel.Password = commercial.Password;
                signinModel.Status = ResultStatus.SUCCESS;
                signinModel.AccessToken = GenerateToken(commercial, sessionId);
                
            }
            catch (Exception  ex)
            {
                string Error = ex.Message;
                InsertLogError(Error);
                return new PostSigninResponseModel
                {
                    Status = ResultStatus.NOT_FOUND
                };
                


            }
            return signinModel;
        }

        public StatusResponseModel SignOut(int id)
        {
            StatusResponseModel result = new StatusResponseModel
            {
                Status = ResultStatus.ACCESS_DENIED
            };

            try
            {
                Sessions currSession = _sessionRepository.SingleOrDefault(item => item.CommercialId == id);
                if (currSession == null)
                {
                    result.Status = ResultStatus.ACCESS_DENIED;
                    InsertLogSinOut(currSession, result);
                }
                else
                {
                    result.Status = ResultStatus.SUCCESS;

                    currSession.EndDate = DateTime.UtcNow;
                    InsertLogSinOut(currSession,result);
                    _sessionRepository.Save();
                    
                }
            }
            catch (Exception e)
            {               
               
                result.Status = ResultStatus.ERROR;
                string Error = e.Message;
                InsertLogError(Error);
                if (e.InnerException != null)
                    result.Message = " InnerExcep.: " + e.InnerException.Message;
            }
            return result;
        }

        public StatusResponseModel KeepAlive(int id)
        {
            StatusResponseModel result = new StatusResponseModel
            {
                Status = ResultStatus.ACCESS_DENIED
            };

            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction dbTransaction = null;
            try
            {
                var context = _sessionRepository.GetContext();
                using (dbTransaction = context.Database.BeginTransaction())
                {
                    Sessions currSession = _sessionRepository.SingleOrDefault(item => item.CommercialId == id);
                    if (currSession == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND; //Si no se encuentra la sessión se está intentando mantener una sesión que ya fue cerrada. Debe dar error. El front debería desautenticarse y mostrar la pantalla de login.
                        result.Message = "No existe una sesión activa para este comercial.";
                    }
                    else if (currSession.EndDate != null)
                    {
                        //Session finished
                        result.Status = ResultStatus.ACCESS_DENIED;
                    }
                    else
                    {
                        result.Status = ResultStatus.SUCCESS;

                        currSession.LastKeepAlive = DateTime.UtcNow;
                        _sessionRepository.Save();
                    }

                    dbTransaction.Commit();
                    return result;
                }
            }
            catch (Exception e)
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Rollback();
                }

                result.Status = ResultStatus.ERROR;
                result.Message = "Excep.: " + e.Message;
                if (e.InnerException != null)
                    result.Message = " InnerExcep.: " + e.InnerException.Message;
            }

            return result;
        }

        public PostUpdatePasswordResponseModel ChangePassword(PostUpdatePasswordRequestModel data)
        {
            PostUpdatePasswordResponseModel result = new PostUpdatePasswordResponseModel
            {
                Status = ResultStatus.ERROR
            };

            IDbContextTransaction dbTransaction = null;
            try
            {
                DbContext context = _commercialRepository.GetContext();
                using (dbTransaction = context.Database.BeginTransaction())
                {
                    // Controlar que contraseña actual coincida con token guardado en la DB
                    var incommingCurrentPasswordHash = _securityService.HashData(data.Password);
                    var dbCommercial = _commercialRepository.GetById(data.CommercialId);
                    if (dbCommercial == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = string.Format("No records found for the commercial (id:{0}).", data.CommercialId);
                        return result;
                    }

                    if (!Equals(incommingCurrentPasswordHash, dbCommercial.Password))
                    {
                        result.Status = ResultStatus.CONFLICT;
                        result.Message = "Passwords do not match.";
                        return result;
                    }

                    // Encriptar nueva contraseña
                    string encryptedPass = _securityService.HashData(data.NewPassword);
                    dbCommercial.Password = encryptedPass;

                    // Actualizar nueva contraseña
                    _commercialRepository.Update(dbCommercial);
                    _commercialRepository.Save();

                    // Conexión a PBX
                    PbxConnection pbxConnection = null;
                    if (_pbxConnect)
                    {
                        pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);

                        if (!pbxConnection.IsInitialized())
                        {
                            result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                            result.Message = pbxMsg;
                            return result;
                        }

                        // validacion instancia PBX
                        if (pbxConnection == null)
                        {
                            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                            result.Message = "[AuthService-ForgotPassword] Could not instantiate PBX";
                            return result;
                        }

                        // petición Update a la PBX
                        pbxConnection.Update(dbCommercial.Peer, dbCommercial.Name, dbCommercial.Password, () => { });
                    }

                    result.Status = ResultStatus.SUCCESS;
                    result.Message = "Ok";
                    result.Password = encryptedPass;
                    dbTransaction.Commit();

                    return result;
                }
            }
            catch (AmiException amiEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[AuthService-ForgotPassword-AmiException] " + amiEx.Message;
                return result;
            }
            catch (Exception e)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.ERROR;
                result.Message = "[AuthService-ForgotPassword-Exception] " + e.Message;
                return result;
            }
        }

        public ForgotPasswordResponseModel ForgotPassword(PostForgotPasswordRequestModel data)
        {
            var result = new ForgotPasswordResponseModel
            {
                Status = ResultStatus.ACCESS_DENIED
            };

            var commercial = new Commercials();
            var currentSession = new Sessions();

            try
            {
                InsertLogForgot( commercial);
                commercial = _commercialRepository.SingleOrDefault(item => item.Email.Equals(data.User, StringComparison.InvariantCultureIgnoreCase));
                if (commercial == null)
                {
                    result.Status = ResultStatus.NOT_FOUND;
                    return result;
                }

                if (commercial.Active == false)
                {
                    result.Status = ResultStatus.ACCESS_DENIED;
                    return result;
                }

                Sessions currSession = _sessionRepository.SingleOrDefault(item => item.CommercialId == commercial.Id);
                if (currSession != null)
                {
                    _sessionRepository.Delete(currSession);
                    _sessionRepository.Save();

                }

                var tempPassword = new Random().Next(10000000, 100000000);
                commercial.Password = _securityService.HashData(tempPassword.ToString());
                commercial.PasswordChangeRequired = true;
                _commercialRepository.Update(commercial);

                // Conexión a PBX
                PbxConnection pbxConnection = null;
                if (_pbxConnect)
                {
                    pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);
                    if (!pbxConnection.IsInitialized())
                    {
                        result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                        result.Message = pbxMsg;
                        return result;
                    }
                }
                if (pbxConnection != null && _pbxConnect)
                {
                    try
                    {
                        pbxConnection.Update(commercial.Peer, commercial.Name, commercial.Password, () => { });
                    }
                    catch (AmiException ex)
                    {
                        result.Status = ResultStatus.PEER_ALREADY_EXIST_IN_PBX;
                        result.Message = ex.Message;
                        return result;
                    }

                }

                result.CommercialEmail = commercial.Email;
                result.CommercialName = commercial.Name;
                result.CommercialPassword = tempPassword.ToString();
                _commercialRepository.Save();
                result.Status = ResultStatus.SUCCESS;
            }
            catch (Exception e)
            {
                return new ForgotPasswordResponseModel
                {
                    Status = ResultStatus.ERROR
                };
            }
            return result;
        }

        private string CreateSessionId()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(2, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private string GenerateToken(Commercials commercial, string sessionId)
        {
            if (commercial == null)
            {
                throw new NullReferenceException();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(ClaimTypes.Name, commercial.Name),
               new Claim(ClaimTypes.Email, commercial.Email),
               new Claim("Peer", commercial.Peer.ToString()),
               new Claim("SessionId", sessionId),
               new Claim("UserId", commercial.Id.ToString()),
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

        public void InsertLogSignIn( Sessions currentSession, Commercials comercial, PostSigninResponseModel signinModel)
        {
            if (comercial != null)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Peticion",
                    Description = "Inicio De Sesion",
                    HourLog = dtNow,
                    UserId = currentSession.CommercialId,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            }
            else if (comercial == null)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Respuesta",
                    Description = "No se encuentra registrado",
                    HourLog = dtNow,
                    UserId = null,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();

            }
            else if (!comercial.Active)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Respuesta",
                    Description = "No Autorizado",
                    HourLog = dtNow,
                    UserId = currentSession.CommercialId,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            }
            else if (signinModel.Status == ResultStatus.SUCCESS)
            {
                DateTime dtNow = DateTime.UtcNow;

                LogsGenerals LogPetition = new LogsGenerals
                {
                    TypeLog = "Log De Respuesta",
                    Description = "Inico De Sesion Exitoso",
                    HourLog = dtNow,
                    UserId = currentSession.CommercialId,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            }           
        }

        public void InsertLogSinOut(Sessions currSession , StatusResponseModel result )
        {
            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log De Peticion",
                Description = "Cerrar Sesion",
                HourLog = dtNow,
                UserId = currSession.CommercialId,
                CallsId = null

            };
            _logsRepository.Insert(LogPetition);
            _logsRepository.Save();

            if (result.Status == ResultStatus.SUCCESS)
            {
                LogsGenerals LogPetitions = new LogsGenerals
                {
                    TypeLog = "Log Respuesta",
                    Description = "Cerrar Sesion Exitoso",
                    HourLog = dtNow,
                    UserId= currSession.CommercialId,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetitions);
                _logsRepository.Save();

            }
            else if (result.Status == ResultStatus.ACCESS_DENIED)
            {
                LogsGenerals LogPetitions = new LogsGenerals
                {
                    TypeLog = "Log  Respuesta",
                    Description = "Cerrar Sesion Fallido",
                    HourLog = dtNow,
                    UserId = null,
                    CallsId = null

                };
                _logsRepository.Insert(LogPetitions);
                _logsRepository.Save();
            }
        }

        public void InsertLogForgot( Commercials commercial)
        {
            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log De Peticion",
                Description = "Recuperar Contraseña",
                HourLog = dtNow,
                UserId = commercial.Id,
                CallsId = null

            };
            _logsRepository.Insert(LogPetition);
            _logsRepository.Save();          
        }

        public void InsertLogError(string Error)
        {
            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetitions = new LogsGenerals
            {
                TypeLog = "Log Error",
                Description = Error,
                HourLog = dtNow,
                UserId = null,
                CallsId = null

            };
            _logsRepository.Insert(LogPetitions);
            _logsRepository.Save();
        }
    }
}
