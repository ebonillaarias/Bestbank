using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.BusinessLogic.Logs;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using inConcert.iMS.ServiceAgent.Exceptions;
using inConcert.iMS.ServiceAgent.PBX;
using inConcert.iMS.ServiceAgent.Siebel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using inConcert.iMS.ServiceAgent.FirebaseCM;

namespace inConcert.iMS.BusinessLogic.Services
{
    public class BOCallsService : IBOCallsService
    {
        private readonly IGenericRepository<Calls> _callsRepository;
        private readonly IGenericRepository<Commercials> _commercialRepository;
        private readonly IGenericRepository<CallParts> _callPartsRepository;
        private readonly IGenericRepository<CallsCustomers> _callsCustomersRepository;
        private readonly IGenericRepository<CustomersPhones> _customersPhonesRepository;
        private readonly IGenericRepository<Sessions> _sessionRepository;
        private readonly IGenericRepository<LogsGenerals> _logsRepository;
        private readonly ISiebelClient _siebelClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly ISiebelLog _siebelLog;
        private readonly string _pbxServer;
        private readonly string _pbxUser;
        private readonly string _pbxPwd;
        private readonly bool _pbxConnect;
        private readonly IStoreProcedureRepository _storeProcedureRepository;

        public BOCallsService(
           IGenericRepository<Calls> callsRepository,
           IGenericRepository<Commercials> commercialRepository,
           IGenericRepository<CallParts> callPartsRepository,
           ISiebelClient siebelClient,
           IFirebaseClient firebaseClient,
           ISiebelLog siebelLog,
           IGenericRepository<CallsCustomers> callsCustomersRepository,
           IGenericRepository<Sessions> sessionRepository,
           IGenericRepository<LogsGenerals> logsRepository,
           IGenericRepository<CustomersPhones> customersPhonesRepository,
           string pbxServer,
           string pbxUser,
           string pbxPwd,
           string pbxConnect,
           IStoreProcedureRepository storeProcedureRepository)
        {
            _callsRepository = callsRepository;
            _commercialRepository = commercialRepository;
            _callPartsRepository = callPartsRepository;
            _callsCustomersRepository = callsCustomersRepository;
            _sessionRepository = sessionRepository;
            _logsRepository = logsRepository;
            _siebelClient = siebelClient;
            _firebaseClient = firebaseClient;
            _siebelLog = siebelLog;
            _customersPhonesRepository = customersPhonesRepository;
            _pbxServer = pbxServer;
            _pbxUser = pbxUser;
            _pbxPwd = pbxPwd;
            _pbxConnect = false;
            _storeProcedureRepository = storeProcedureRepository;
            if (Int32.TryParse(pbxConnect, out int _aux))
            {
                _pbxConnect = _aux == 1 ? true : false;
            }

        }

        /// <summary>
        /// Dado un comercial retorna información sobre las llamadas en las que participó.
        /// Si <paramref name="days"/> es 0 (cero) retorna todas las llamadas, en otro caso
        /// se devuelve las llamadas de los últimos n dias indicado por dicho parámetro.
        /// </summary>
        /// <param name="commercialId">Id del comercial.</param>
        /// <param name="days">Últimos días a considerar.</param>
        /// <returns>
        /// Modelo <see cref="GetBOCallsByCommercialResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public GetBOCallsByCommercialResponseModel GetCallsByCommercials(int commercialId, int days)
        {
            GetBOCallsByCommercialResponseModel returnModel = new GetBOCallsByCommercialResponseModel();

            #region Validacion datos entrada
            if (commercialId < 0 || days < 0)
            {
                returnModel.Status = ResultStatus.BAD_REQUEST;
                returnModel.Message = string.Format("The parameter {0} or {1} is negative.", nameof(commercialId), nameof(days));
                return returnModel;
            }
            #endregion

            try
            {
                DbSet<CallParts> tableCallsPart = _callPartsRepository.GetTable();
                IQueryable<CallParts> queryCallParts = tableCallsPart.Include(cp => cp.Call)
                                                   .Where(cp => cp.CommercialId == commercialId &&
                                                                (days > 0 ? cp.Call.StartDate.Date >= DateTimeOffset.Now.AddDays(-days).Date : true)
                                                         );
                List<CallParts> resultParts = queryCallParts.OrderByDescending(cp => cp.Call.StartDate).ThenByDescending(cp => cp.StartDate).ToList();

                // Armo resultado a devolver
                returnModel.ListCustomer = new List<CallsRecentsModel>();
                returnModel.Status = ResultStatus.SUCCESS;

                foreach (CallParts cp in resultParts)
                {
                    CallsRecentsModel callModel = new CallsRecentsModel();
                    callModel.CalledNumber = cp.Call.Direction == CallDirection.Inbound ? cp.Call.CallerId : cp.Call.CalledId;
                    callModel.DateTime = cp.StartDate;
                    callModel.Direction = cp.Call.Direction;
                    callModel.CallPartResult = cp.CallResult;
                    callModel.Customer = new CustomerModel()
                    {
                        CustomerId = cp.Call.CustomerId,
                        CustomerName = cp.Call.CustomerName,
                        CustomerType = CustomerType.Business,
                        PhoneNumberList = new List<PhoneNumberModel>()
                    };

                    returnModel.ListCustomer.Add(callModel);
                }
                return returnModel;
            }
            catch (Exception e)
            {
                return new GetBOCallsByCommercialResponseModel
                {
                    Status = ResultStatus.ERROR,
                    Message = "[Excep.] " + e.Message
                };
            }
        }

        /// <summary>
        /// Retorna información sobre las llamadas, en el cual los filtros <paramref name="data.Result"/> y
        /// <paramref name="data.Commercials"/> aplican UNICAMENTE sobre la ultima parte de la llamada;
        /// mientras que el resto de los filtros (StarDate, EndDate y Client) aplican sobre la llamada en si (tabla Calls)
        /// </summary>
        /// <param name="data">Modelo con los datos a filtrar.</param>
        /// <returns>
        /// Modelo <see cref="PostBOCallsResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public PostBOCallsResponseModel GetCallsByLastPart(PostBOCallsRequestModel data)
        {
            PostBOCallsResponseModel returnModel = new PostBOCallsResponseModel
            {
                Status = ResultStatus.BAD_REQUEST
            };

            #region Validacion datos entrada
            // 'Result' debe ser válido
            if (!string.IsNullOrWhiteSpace(data.Result))
            {
                var resultLower = data.Result.ToLower();
                if (string.Compare(data.Result.ToLower(), "completed") != 0 &&
                    string.Compare(data.Result.ToLower(), "noanswer") != 0 &&
                    string.Compare(data.Result.ToLower(), "rejected") != 0 &&
                    string.Compare(data.Result.ToLower(), "transferred") != 0)
                {
                    return returnModel;
                }
            }

            // Validacion fechas
            DateTime? filterDateStart = null;
            DateTime? filterDateEnd = null;
            if (!string.IsNullOrWhiteSpace(data.Start))
            {
                if (DateTime.TryParse(data.Start, new CultureInfo("pt-PT"), 0, out DateTime startDate))
                {
                    filterDateStart = startDate;
                }
                else
                {
                    return returnModel;
                }
            }

            if (!string.IsNullOrWhiteSpace(data.End))
            {
                if (DateTime.TryParse(data.End, new CultureInfo("pt-PT"), 0, out DateTime endDate))
                {
                    filterDateEnd = endDate;
                }
                else
                {
                    return returnModel;
                }
            }
            #endregion

            try
            {
                // De la tabla CallParts obtengo los ID de las llamadas en las que la ultima parte de la llamada
                // cumplen con los filtros: CommercialId y Result
                List<int> listCallId = _callPartsRepository
                   .GetAll().GroupBy(cp => cp.CallId)
                   .SelectMany(a =>
                                 a.Where(b =>
                                           Convert.ToInt32(b.CallPartNumber) == a.Max(c => Convert.ToInt32(c.CallPartNumber)) // verifico que sea en la ultima callpart
                                           &&
                                           (data.Commercials.Count > 0 ? data.Commercials.Contains(b.CommercialId) : true) // valido comercialID en la ultima callpart
                                           &&
                                           (!string.IsNullOrWhiteSpace(data.Result) ? b.CallResult == Utilities.GetCallResultEnum(data.Result) : true) // valido CallRestul en la ultima parte de la llamada
                                        )
                              )
                   .Select(cp => cp.CallId)
                   .ToList();

                // Recupero las llamadas aplicando el resto de los filtros (StartDate y EndDate)
                List<Calls> callDB = _callsRepository
                   .FindAll(c =>
                               listCallId.Contains(c.Id)
                               &&
                               (filterDateStart.HasValue ? c.StartDate.Date >= filterDateStart.Value.Date : true)
                               &&
                               (filterDateEnd.HasValue ? c.StartDate.Date <= filterDateEnd.Value.Date : true)
                               &&
                               (!string.IsNullOrWhiteSpace(data.ClientID) ? string.Equals(c.CustomerId, data.ClientID, StringComparison.InvariantCultureIgnoreCase) : true)
                           )
                   .ToList();

                // Armo resultado a devolver
                returnModel.Calls = new List<BOCallModel>();
                returnModel.Status = ResultStatus.SUCCESS;

                foreach (Calls c in callDB)
                {
                    // obtengo la primera parte de la llamada (para poder completar  los campos 'Result' y 'Transferred' si aplica)
                    string maximum = c.CallParts.Max(cp => cp.CallPartNumber);
                    CallParts auxCallPart = c.CallParts.FirstOrDefault(cp => cp.CallPartNumber.Equals(maximum));

                    string callDuration = "";
                    if (c.EndDate.HasValue)
                    {
                        TimeSpan durationTimeSpan = c.EndDate.Value - c.StartDate;
                        callDuration = durationTimeSpan.TotalHours.ToString("00") + ":" +
                           durationTimeSpan.Minutes.ToString("00") + ":" +
                           durationTimeSpan.Seconds.ToString("00");
                    }

                    returnModel.Calls.Add(new BOCallModel
                    {
                        Id = c.Id,
                        CallerId = c.CallerId,
                        CalledId = c.CalledId,
                        Direction = c.Direction,
                        StartDate = c.StartDate,
                        Duration = callDuration,
                        EndDate = c.EndDate,
                        CustomerName = c.CustomerName,
                        CallParts = c.CallParts,
                        CustomerId = c.CustomerId,
                        Result = auxCallPart.CallResult,
                        Transferred = auxCallPart.CommercialName
                    });
                }

                return returnModel;
            }
            catch (Exception e)
            {
                return new PostBOCallsResponseModel
                {
                    Status = ResultStatus.ERROR
                };
            }
        }

        /// <summary>
        /// Recupera información de una llamada y las partes de la misma.
        /// </summary>
        /// <param name="id">ID de la llamada de la cual se desea obtener información.</param>
        /// <param name="url">URL donde se alojan los audios de las partes de la llamada.</param>
        /// <returns>
        /// Modelo <see cref="GetBOCallDetailsResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public GetBOCallDetailsResponseModel GetCallDetails(int id, string url)
        {
            GetBOCallDetailsResponseModel responseModel = new GetBOCallDetailsResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            try
            {
                // Recupero informacion de la llamada
                Calls callDB = _callsRepository.GetById(id);
                if (callDB == null)
                {
                    responseModel.Status = ResultStatus.NOT_FOUND;
                    responseModel.Message = string.Format("No records found for the call (id:{0}).", id);
                }
                else
                {
                    // Recupero las call parts (lista ordenada asc por CallPartNumber, y por Id)
                    List<CallParts> callPartsDB = _callPartsRepository.FindAll(cp => cp.CallId == id).OrderBy(cp => Convert.ToInt32(cp.CallPartNumber)).ThenBy(cp => cp.Id).ToList();

                    // Armo informacion principal sobre la llamada (tabla Calls)
                    responseModel.Info = new BOCallModel
                    {
                        Id = callDB.Id,
                        CallerId = callDB.CallerId,
                        CalledId = callDB.CalledId,
                        Direction = callDB.Direction,
                        StartDate = callDB.StartDate,
                        EndDate = callDB.EndDate,
                        CustomerName = callDB.CustomerName,
                        Duration = callDB.EndDate.HasValue ?
                                      string.Format("{0:00}:{1:00}:{2:00}", (callDB.EndDate.Value - callDB.StartDate).TotalHours, (callDB.EndDate.Value - callDB.StartDate).Minutes, (callDB.EndDate.Value - callDB.StartDate).Seconds) :
                                      string.Empty,
                        Result = callPartsDB.Count == 0 ? CallResult.NoAnswer : callPartsDB.Last().CallResult,
                        Transferred = callPartsDB.Count == 0 ? "" : callPartsDB.Last().CommercialName
                    };

                    // Armo informacion sobre cada una de las partes de la llamada
                    // previamente, si corresponde, se elimina la (ultima) parte "InProgress"
                    if (callPartsDB.Count > 0 && callPartsDB.Last().CallResult == CallResult.InProgress)
                    {
                        callPartsDB.RemoveAt(callPartsDB.Count - 1);
                    }
                    responseModel.Parts = callPartsDB.ConvertAll(cp =>
                       new BOCallPartsModel
                       {
                           CallPart = Convert.ToInt32(cp.CallPartNumber),
                           StartDate = cp.StartDate,
                           EndDate = cp.EndDate,
                           Path = !string.IsNullOrWhiteSpace(cp.FilePath) ? Path.Combine(url, cp.FilePath) : ""
                       });
                    responseModel.Status = ResultStatus.SUCCESS;
                }
                return responseModel;
            }
            catch (ArgumentNullException argNullEx)
            {
                responseModel.Status = ResultStatus.ERROR;
                responseModel.Message = "[BOCallsService-GetCallDetails-ArgumentNullException]: " + argNullEx.Message;
                return responseModel;
            }
            catch (FormatException formatEx)
            {
                responseModel.Status = ResultStatus.ERROR;
                responseModel.Message = "[BOCallsService-GetCallDetails-FormatException]: " + formatEx.Message;
                return responseModel;
            }
            catch (ArgumentException argEx)
            {
                responseModel.Status = ResultStatus.ERROR;
                responseModel.Message = "[BOCallsService-GetCallDetails-ArgumentException]: " + argEx.Message;
                return responseModel;
            }
            catch (Exception e)
            {
                responseModel.Status = ResultStatus.ERROR;
                responseModel.Message = "[BOCallsService-GetCallDetails-Exception]: " + e.Message;
                if (e.InnerException != null)
                    responseModel.Message += " [InnerException]: " + e.InnerException.Message;
                return responseModel;
            }
        }

        /// <summary>
        /// Retorna información sobre las llamadas.
        /// </summary>
        /// <param name="data">Modelo con los datos a filtrar.</param>
        /// <returns>
        /// Modelo <see cref="GetBOCallsCustomersByCommercialsResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public GetBOCallsCustomersByCommercialsResponseModel GetCustomersByCommercials(List<int> data)
        {
            var customers = new List<CustomerLiteModel>();
            var result = new GetBOCallsCustomersByCommercialsResponseModel
            {
                Customers = customers,
                Status = ResultStatus.SUCCESS
            };

            // Obtengo los comercials identificados por id
            var commercials = new HashSet<Commercials>();
            foreach (int id in data)
            {
                Commercials commercial = _commercialRepository.GetById(id);
                if (commercial != null)
                {
                    commercials.Add(commercial);
                }
            }

            foreach (Commercials comm in commercials)
            {
                var callCustomers = _callsRepository.FindAll(call =>
                                                                (
                                                                 ((call.CallerId == comm.MobilePhoneNumber || call.CallerId == comm.PBXPhoneNumber) && call.Direction == CallDirection.Outbound)
                                                                 ||
                                                                 ((call.CalledId == comm.MobilePhoneNumber || call.CalledId == comm.PBXPhoneNumber) && call.Direction == CallDirection.Inbound)
                                                                )
                                                                &&
                                                                !string.IsNullOrWhiteSpace(call.CustomerId)
                                                             )
                                                    .GroupBy(call => call.CustomerId)
                                                    .Select(g => g.First())
                                                    .Select(call => new CustomerLiteModel()
                                                    {
                                                        CustomerId = call.CustomerId,
                                                        CustomerName = call.CustomerName
                                                    });

                result.Customers = result.Customers.Union(callCustomers).GroupBy(call => call.CustomerId).Select(g => g.First()).ToList();
            }
            return result;
        }

        public async Task<StatusResponseModel> StartCallPart(string endPoint, PostCallsStartCallPartRequestModel data)
        {
            var result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            var context = _callPartsRepository.GetContext();
            using (var dbTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    _callPartsRepository.Insert(new CallParts()
                    {
                        CallId = data.CallId,
                        CallPartNumber = data.CallPartNumber,
                        Peer = data.Peer,
                        CommercialId = data.CommercialId,
                        CommercialName = data.CommercialName,
                        OrigChannel = data.OrigChannel,
                        RedirectChannel = data.RedirectChannel,
                        StartDate = data.StartDate
                    });
                    _callPartsRepository.Save();

                    dbTransaction.Commit();
                    result.Status = ResultStatus.SUCCESS;
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    result.Status = ResultStatus.ERROR;
                    result.Message = "[BOCallsService-StartCallPart-Exception] " + e.Message;
                    if (e.InnerException != null)
                        result.Message += " InnerExcep.: " + e.InnerException.Message;
                    return result;
                }
            }

            #region SendPushNotification-ICBB-854
            string firebaseToken = string.Empty;
            try
            {
                Sessions session = _sessionRepository.SingleOrDefault(s => s.CommercialId == data.CommercialId);
                if (session != null && session.StartDate != null && session.EndDate == null) // session activa
                {
                    firebaseToken = session.FirebaseToken;
                    if (!string.IsNullOrWhiteSpace(firebaseToken))
                    {
                        FCMResponse firebaseResponse = await _firebaseClient.Send(endPoint, firebaseToken).ConfigureAwait(false);
                        #region FirebaseLog
                        string strFirebaseResponse = System.Text.Json.JsonSerializer.Serialize(firebaseResponse);
                        _siebelLog.Write("", firebaseToken, strFirebaseResponse, ISiebelLog.LOGTYPE_FIREBASE);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                // Marcelo Torterolo
                // Re-Utilizo el arhivo de LOGs de siebel a los efectos de registrar la excepción
                #region FirebaseLog
                string message = "[BOCallsService-StartCallPart-FCMException] " + e.Message;
                if (e.InnerException != null)
                    message += " InnerExcep.: " + e.InnerException.Message;
                _siebelLog.Write("", firebaseToken, message, ISiebelLog.LOGTYPE_FIREBASE);
                #endregion
            }
            #endregion

            return result;
        }

        public PostCallsUpdateCallPartRedirectChannelResponseModel UpdateCallPartRedirectChannel(PostCallsUpdateCallPartRedirectChannelRequestModel data)
        {
            var result = new PostCallsUpdateCallPartRedirectChannelResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            try
            {
                var callPart = _callPartsRepository.SingleOrDefault(c => c.CallId == data.CallId && c.CallPartNumber == data.CallPartNumber);
                if (callPart == null)
                {
                    result.Status = ResultStatus.NOT_FOUND;
                    result.Message = "No se encontró la parte de llamada con identificada con CallId = " + data.CallId + " y CallPartNumber = " + data.CallPartNumber + ".";
                }

                callPart.RedirectChannel = data.RedirectChannel;

                _callPartsRepository.Update(callPart);
                _callPartsRepository.Save();

                result.Status = ResultStatus.SUCCESS;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private CallResult? GetCallResult(string callResult)
        {
            switch (callResult)
            {
                case "InProgress":
                    return CallResult.InProgress;
                case "Completed":
                    return CallResult.Completed;
                case "NoAnswer":
                    return CallResult.NoAnswer;
                case "Rejected":
                    return CallResult.Rejected;
                case "Transferred":
                    return CallResult.Transferred;
                default:
                    return null;
            }
        }

        private CallPartEndedBy? GetCallPartEndedBy(string callEndedBy)
        {
            switch (callEndedBy)
            {
                case "Cliente":
                    return CallPartEndedBy.Cliente;
                case "Comercial":
                    return CallPartEndedBy.Comercial;
                default:
                    return null;
            }
        }

        // UpdateCallPartEnd
        public PostCallsEndCallPartResponseModel UpdateCallPartEnd(PostCallsEndCallPartRequestModel data)
        {
            var result = new PostCallsEndCallPartResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            try
            {
                var callPart = _callPartsRepository.SingleOrDefault(c => c.CallId == data.CallId && c.CallPartNumber == data.CallPartNumber);

                if (callPart == null)
                {
                    result.Status = ResultStatus.NOT_FOUND;
                    result.Message = "No se encuentró la parte de llamada con identificada con CallId = " + data.CallId + " y CallPartNumber = " + data.CallPartNumber + ".";
                }

                var callResult = GetCallResult(data.CallResult);
                var callPartEndedBy = GetCallPartEndedBy(data.CallPartEndedBy);

                if (!callResult.HasValue || !callPartEndedBy.HasValue)
                {
                    result.Status = ResultStatus.BAD_REQUEST;
                }
                else
                {
                    callPart.EndDate = DateTimeOffset.UtcNow;
                    callPart.CallResult = callResult.Value;
                    callPart.CallPartEndedBy = callPartEndedBy.Value;
                    callPart.RejectionReason = data.RejectionReason;

                    _callPartsRepository.Update(callPart);
                    _callPartsRepository.Save();

                    result.Status = ResultStatus.SUCCESS;
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<StatusResponseModel> EndCallUpdateDBState(int inCallId, string host)
        {
            StatusResponseModel result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            var context = _callsRepository.GetContext();
            using (var dbTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    DbSet<Calls> tableCalls = _callsRepository.GetTable();
                    Calls call = tableCalls.Include(c => c.CallParts).SingleOrDefault(c => c.Id == inCallId);
                    if (call == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = string.Format("No records found for the call (id:{0}).", inCallId);
                    }
                    else
                    {
                        CallParts lastPart = call.CallParts.OrderBy(cp => Convert.ToInt32(cp.CallPartNumber)).LastOrDefault();

                        // actualizo fecha de finalizacion de la llamada
                        call.EndDate = DateTimeOffset.UtcNow;

                        _callsRepository.Update(call);
                        _callsRepository.Save();



                        //Si ya tengo el Cliente y EndDate mando a Siebel
                        if (!string.IsNullOrWhiteSpace(call.CustomerId))
                        {
                            var outCallData = new SiebelRequestEndCallModel()
                            {
                                CustomerId = call.CustomerId,
                                InteractionId = call.Id.ToString(),
                                StartDate = string.Format("{0:MM/dd/yyyy HH:mm:ss}", call.StartDate),
                                EndDate = string.Format("{0:MM/dd/yyyy HH:mm:ss}", call.EndDate),
                                CallResult = lastPart != null ? Utilities.GetCallResult(lastPart.CallResult) : "NoAnswer",
                                RejectionReason = lastPart != null ? (lastPart.CallResult == CallResult.Rejected ? lastPart.RejectionReason : "") : "",
                                //Se agrego el parametro CallPartEndedBy que en base de datos indica quien finalizo la llamada
                                CallPartEndedBy = lastPart.CallPartEndedBy.ToString()
                            };
                            SetForceCustomerID(outCallData);
                            StatusResponseModel reponseSiebel = await EndCallSiebel(host, outCallData).ConfigureAwait(false);

                            // remove flag "REJECTED_CALL"
                            if (string.Equals(call.CustomerId, Constants.REJECTED_CALL))
                            {
                                call.CustomerId = string.Empty;
                                _callsRepository.Update(call);
                                _callsRepository.Save();
                            }

                        }

                        dbTransaction.Commit();
                        result.Status = ResultStatus.SUCCESS;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    result.Status = ResultStatus.ERROR;
                    result.Message = "[BOCallsService-EndCallUpdateDBState-Exception]: " + ex.Message;
                    return result;
                }
            }
        }

        private void SetForceCustomerID(SiebelRequestEndCallModel data)
        {
            if (string.Equals(Constants.REJECTED_CALL, data.CustomerId))
            {
                data.CustomerId = string.Empty;
            }

        }

        public async Task<PostCallsStartInboundCallResponseModel> StartInboundCall(string host, PostCallsStartInboundCallRequestModel data, bool getPeersOnlyConnected = true)
        {
            var result = new PostCallsStartInboundCallResponseModel()
            {
                Status = ResultStatus.ERROR
            };
            var context = _callsRepository.GetContext();
            using (var dbTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    var table = _commercialRepository.GetTable();
                    var commercial = table.Include(c => c.AlternativeCommercials).Include(c => c.Session).SingleOrDefault(c => c.PBXPhoneNumber == data.CalledId);
                    if (commercial == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = "Comercial identificado con PBXPhoneNumber = " + data.CalledId + " no encontrado.";
                        return result;
                    }

                    var siebelId = commercial.SiebelId;
                    result.PeerList = new List<int>();

                    if (!getPeersOnlyConnected || commercial.Session != null && commercial.Session.StartDate != null && commercial.Session.EndDate == null)
                    {
                        result.PeerList.Add(commercial.Peer);
                    }

                    result.PeerList.AddRange(commercial.AlternativeCommercials
                                               .Where(ac => table.Include(f => f.Session).SingleOrDefault(acC =>
                                                     acC.Id == ac.AlternativeCommercialId
                                                     && (!getPeersOnlyConnected || acC.Session != null && acC.Session.StartDate != null && acC.Session.EndDate == null)) != null)
                                               .OrderBy(ac => ac.Order)
                                               .Select(ac =>
                                                  table.Include(f => f.Session).Single(acC => acC.Id == ac.AlternativeCommercialId).Peer
                                               )
                                            );

                    var call = new Calls()
                    {
                        CallerId = data.CallerId,
                        CalledId = data.CalledId,
                        Direction = CallDirection.Inbound,
                        StartDate = DateTimeOffset.UtcNow
                    };

                    _callsRepository.Insert(call);
                    _callsRepository.Save();

                    result.Status = ResultStatus.SUCCESS;
                    result.CallId = call.Id;

                    var commercials = new Commercials()
                    {
                        Id = commercial.Id
                    };

                    InsertLogCallIn(call, commercials);

                    SiebelRequestNewCallModel postCallSibel = new SiebelRequestNewCallModel()
                    {
                        InteractionId = call.Id.ToString(),
                        CalledId = data.CalledId,
                        CallerId = data.CallerId,
                        Direction = CallDirection.Inbound,
                        UserId = commercial.SiebelId
                    };

                    SAResponseData responseNewCall = await _siebelClient.NewCall(host, postCallSibel).ConfigureAwait(false);
                    #region SiebelLog
                    string strSiebelRequest = System.Text.Json.JsonSerializer.Serialize(postCallSibel);
                    string strSiebelResponse = System.Text.Json.JsonSerializer.Serialize(responseNewCall);
                    _siebelLog.Write(host, strSiebelRequest, strSiebelResponse);
                    #endregion

                    try
                    {
                        if (responseNewCall != null &&
                            responseNewCall.outputData != null &&
                            responseNewCall.outputData.ListOfCustomer != null &&
                            responseNewCall.outputData.ListOfCustomer.Customer != null &&
                            responseNewCall.outputData.ListOfCustomer.Customer.Count > 0)
                        {
                            if (responseNewCall.outputData.ListOfCustomer.Customer.Count > 0) 
                            {
                                var objCustomer = responseNewCall.outputData.ListOfCustomer.Customer.FirstOrDefault();
                                
                                call.CustomerId = objCustomer.CustomerId;
                                call.CustomerName = objCustomer.CustomerName;

                                _callsRepository.Update(call);
                                _callsRepository.Save();
                            }

                            foreach (SACustomerModel element in responseNewCall.outputData.ListOfCustomer.Customer.Where(cc => cc.CustomerId != null))
                            {
                                CallsCustomers callInternal = new CallsCustomers()
                                {
                                    CustomerId = element.CustomerId,
                                    CallId = call.Id,
                                    CustomerName = element.CustomerName,
                                    CustomerType = element.CustomerType,
                                    CustomerPhone = new List<CustomersPhones>(),
                                };

                                foreach (var item in element.ListOfPhoneNumber.PhoneNumber.Where(p => p.PhoneNumber != null && p.PhoneType != null))
                                {
                                    CustomersPhones cp = new CustomersPhones()
                                    {
                                        CustomerId = element.CustomerId,
                                        PhoneNumber = item.PhoneNumber,
                                        PhoneType = item.PhoneType
                                    };
                                    callInternal.CustomerPhone.Add(cp);
                                }

                                _callsCustomersRepository.Insert(callInternal);
                                _callsCustomersRepository.Save();
                            }
                        }
                    }
                    catch (DbUpdateException insertCustomerError)
                    {
                        dbTransaction.Rollback();
                        var resultError = new PostCallsStartInboundCallResponseModel()
                        {
                            Status = ResultStatus.ERROR,
                            Message = insertCustomerError.Message
                        };

                        return resultError;
                    }

                    dbTransaction.Commit();
                    return result;
                }
                catch (DbUpdateException updEx)
                {
                    dbTransaction.Rollback();
                    var resultErrorInsert = new PostCallsStartInboundCallResponseModel()
                    {
                        Status = ResultStatus.ERROR,
                        Message = updEx.Message
                    };

                    return resultErrorInsert;
                }
            }
        }

        public async Task<PostCallsStartOutboundCallResponseModel> StartOutboundCall(string host, PostCallsStartOutboundCallRequestModel data)
        {
            PostCallsStartOutboundCallResponseModel result = new PostCallsStartOutboundCallResponseModel()
            {
                Status = ResultStatus.ERROR
            };
            var context = _callsRepository.GetContext();
            using (var dbTransaction = context.Database.BeginTransaction())
            {

                try
                {
                    var commercial = _commercialRepository.SingleOrDefault(c => c.Peer == int.Parse(data.Peer));
                    if (commercial == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = "No se encuentra el comercial identificado con Peer = " + data.Peer + ".";
                        return result;
                    }

                    var call = new Calls()
                    {
                        CallerId = commercial.PBXPhoneNumber, //TODO: Confirmar si este es el telefono esperado.
                        CalledId = data.CalledId,
                        Direction = CallDirection.Outbound,
                        StartDate = DateTimeOffset.UtcNow
                    };

                    _callsRepository.Insert(call);
                    _callsRepository.Save();

                    result.Status = ResultStatus.SUCCESS;
                    result.CallId = call.Id;
                    result.PBXPhoneNumber = commercial.PBXPhoneNumber;
                    result.MobilePhoneNumber = commercial.MobilePhoneNumber;

                    var commercials = new Commercials()
                    {
                        Id = commercial.Id
                    };

                    InsertLogCallOut(call, commercials);

                    SiebelRequestNewCallModel postCallSibel = new SiebelRequestNewCallModel()
                    {
                        InteractionId = call.Id.ToString(),
                        CalledId = call.CalledId,
                        CallerId = call.CallerId,
                        Direction = CallDirection.Outbound,
                        UserId = commercial.SiebelId
                    };

                    //TODO; CustomerId de donde se saca para enviarlo a Siebel?
                    SAResponseData responseNewCall = await _siebelClient.NewCall(host, postCallSibel).ConfigureAwait(false);
                    #region SiebelLog
                    string strSiebelRequest = System.Text.Json.JsonSerializer.Serialize(postCallSibel);
                    string strSiebelResponse = System.Text.Json.JsonSerializer.Serialize(responseNewCall);
                    _siebelLog.Write(host, strSiebelRequest, strSiebelResponse);
                    #endregion

                    if (responseNewCall == null || string.IsNullOrWhiteSpace(responseNewCall.returnCode))
                    {
                        dbTransaction.Rollback();
                        result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                        result.Message = "Error desconocido en Servicio Siebel";
                        return result;
                    }
                    dbTransaction.Commit();
                    return result;
                }
                catch (FormatException e)
                {
                    dbTransaction.Rollback();
                    result.Status = ResultStatus.BAD_REQUEST;
                    return result;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    return result;
                }
            }
        }

        public StatusResponseModel UploadCallPartFile_UpdateTable(PostCallsUploadCallPartFileRequestModel data)
        {
            StatusResponseModel result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR,
                Message = ""
            };

            if (!int.TryParse(data.CallId, out int pCallId) || !int.TryParse(data.CallPartNumber, out int pPartNumber))
            {
                result.Status = ResultStatus.BAD_REQUEST;
                result.Message = "Datos de entrada incorrectos: CallId y/o CallPartNumber no son números enteros.";
                return result;
            }

            DbContext context = _callPartsRepository.GetContext();
            using (IDbContextTransaction dbTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Nombre del archivo a guardar:
                    // CallId(4 digitos) + PartNumber(2digitos) + Nombre (del archivo)
                    string fileName = string.Format("{0:D4}_{1:D2}_{2}", pCallId, pPartNumber, data.FileContent.FileName);

                    CallParts toUpdate = _callPartsRepository.SingleOrDefault(cp => cp.CallId == pCallId && Convert.ToInt32(cp.CallPartNumber) == pPartNumber);
                    if (toUpdate == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = string.Format("No se ecuentra registro en tabla CallParts (CallId:{0}, CallPartNumber:{1}).", pCallId, pPartNumber);
                        return result;
                    }

                    toUpdate.FilePath = fileName;

                    _callPartsRepository.Update(toUpdate);
                    _callPartsRepository.Save();

                    dbTransaction.Commit();
                    result.Status = ResultStatus.SUCCESS;
                    return result;
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    result.Status = ResultStatus.ERROR;
                    result.Message = e.Message;
                    return result;
                }
            }
        }

        public StatusResponseModel UploadCallPartFile_UploadFile(PostCallsUploadCallPartFileRequestModel data, List<string> allowExtentions)
        {
            StatusResponseModel result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR,
                Message = ""
            };

            if (!int.TryParse(data.CallId, out int pCallId) || !int.TryParse(data.CallPartNumber, out int pPartNumber))
            {
                result.Status = ResultStatus.BAD_REQUEST;
                result.Message = "Datos de entrada incorrectos.";
                return result;
            }

            try
            {
                // validacion directorio
                if (!Directory.Exists(data.PathToUpload))
                {
                    result.Status = ResultStatus.ERROR;
                    result.Message = string.Format("No existe el directorio: '{0}'", Path.GetFullPath(data.PathToUpload));
                    return result;
                }

                // Nombre del archivo a guardar:
                // CallId(4 digitos) + PartNumber(2digitos) + Nombre (del archivo)
                string fileName = string.Format("{0:D4}_{1:D2}_{2}", pCallId, pPartNumber, data.FileContent.FileName);
                string pathFile = Path.Combine(data.PathToUpload, fileName);

                if (Path.GetExtension(pathFile) == "")
                {
                    result.Status = ResultStatus.ERROR;
                    result.Message = string.Format("No se permite archivos sin extension.");
                    return result;
                }

                if (!allowExtentions.Contains(Path.GetExtension(pathFile)))
                {
                    result.Status = ResultStatus.ERROR;
                    result.Message = string.Format("Extension de archivo no permitida: '{0}'", Path.GetExtension(pathFile));
                    return result;
                }

                // Create new local file and copy contents of uploaded file
                using (var localFile = File.OpenWrite(pathFile))
                using (var uploadedFile = data.FileContent.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                result.Status = ResultStatus.SUCCESS;
                return result;
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.ERROR;
                result.Message = e.Message;
                return result;
            }
        }

        public async Task<StatusResponseModel> SetCustomer(PostCallsSetCustomerRequestModel data, CallDirection direction, string host)
        {
            var result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR
            };

            IDbContextTransaction dbTransaction = null;
            try
            {
                GetCallIdBySessionIdResponseModel callModel = GetCallIdBySessionId(data.SessionId, false);


                if (callModel == null || callModel.Status != ResultStatus.SUCCESS)
                {
                    result.Status = ResultStatus.NOT_FOUND;
                    result.Message = callModel.Message;
                    return result;
                }

                DbContext context = _callsRepository.GetContext();
                using (dbTransaction = context.Database.BeginTransaction())
                {
                    var call = _callsRepository.GetById(callModel.CallId);

                    if (string.IsNullOrEmpty(call.CustomerId))
                    {
                        call.CustomerId = data.CustomerId;
                        call.CustomerName = data.CustomerName;

                        _callsRepository.Update(call);
                        _callsRepository.Save();
                    }

                    if (direction == CallDirection.Inbound)
                    {
                        // Eliminar call customers y sus customersPhones
                        var callsCustomersTable = _callsCustomersRepository.GetTable();
                        var callsCustomers = callsCustomersTable.Include(cc => cc.CustomerPhone).Where(cc => cc.CallId == callModel.CallId);
                        foreach (var cc in callsCustomers.ToList())
                        {
                            _customersPhonesRepository.DeleteRange(cc.CustomerPhone.ToList());
                        }
                        _customersPhonesRepository.Save();
                        _callsCustomersRepository.DeleteRange(callsCustomers.ToList());
                        _callsCustomersRepository.Save();
                    }

                    //Mandamos el dato a Siebel si tenemos el endDate
                    if (call.EndDate != null)
                    {
                        DbSet<CallParts> tableCallsPart = _callPartsRepository.GetTable();
                        var queryCallParts = tableCallsPart.Where(c => c.CallId == callModel.CallId);

                        List<CallParts> resultParts = queryCallParts.OrderBy(c => c.CallId)
                                                                        .ThenBy(c => Convert.ToInt32(c.CallPartNumber))
                                                                        .ToList();

                        SiebelRequestEndCallModel outCallData = new SiebelRequestEndCallModel()
                        {
                            CustomerId = call.CustomerId,
                            InteractionId = call.Id.ToString(),
                            StartDate = string.Format("{0:MM/dd/yyyy HH:mm:ss}", call.StartDate),
                            EndDate = string.Format("{0:MM/dd/yyyy HH:mm:ss}", call.EndDate),
                            CallResult = "NoAnswer",
                            RejectionReason = ""
                        };

                        if (resultParts.Count > 0)
                        {
                            CallParts lastPart = resultParts.Last();
                            outCallData.CallResult = Utilities.GetCallResult(lastPart.CallResult);
                            outCallData.RejectionReason = lastPart.CallResult == CallResult.Rejected ? lastPart.RejectionReason : "";
                        }

                        SetForceCustomerID(outCallData);
                        StatusResponseModel reponseSiebel = await EndCallSiebel(host, outCallData).ConfigureAwait(false);
                        switch (reponseSiebel.Status)
                        {
                            case ResultStatus.SUCCESS:
                                // remove flag "REJECTED_CALL"
                                if (string.Equals(call.CustomerId, Constants.REJECTED_CALL))
                                {
                                    call.CustomerId = string.Empty;
                                    _callsRepository.Update(call);
                                    _callsRepository.Save();
                                }
                                break;
                            default:
                                result.Message = reponseSiebel.Message;
                                break;
                        }
                    }

                    dbTransaction.Commit();
                    result.Status = ResultStatus.SUCCESS;
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Rollback();
                }

                result.Status = ResultStatus.ERROR;
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Transferir una llamada activa a otro comercial.
        /// </summary>
        /// <param name="inOrigChannel">Nombre del canal SIP que originó la llamada.</param>
        /// <param name="inRedirectChannel">Nombre del canal SIP que debe redirigirse a otro peer.</param>
        /// <param name="inPeer">Peer del comercial al cual transferir.</param>
        /// <returns>
        /// Modelo <see cref="StatusResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public StatusResponseModel CallRedirect(string inOrigChannel, string inRedirectChannel, int inPeer)
        {
            StatusResponseModel result = new StatusResponseModel()
            {
                Status = ResultStatus.ERROR,
                Message = ""
            };

            PbxConnection pbxConnection = null;
            try
            {
                // Validacion de que exista comercial al cual transferir la llamada
                Commercials commercial = _commercialRepository.SingleOrDefault(c => c.Peer == inPeer);
                if (commercial == null)
                {
                    result.Status = ResultStatus.NOT_FOUND;
                    result.Message = string.Format("No se encuentra el comercial identificado con Peer = {0}.", inPeer);
                    return result;
                }

                // Conexion a PBX
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
                        result.Message = "No se pudo instanciar PBX";
                        return result;
                    }

                    // petición RedirectCall a la PBX
                    pbxConnection.RedirectCall(inOrigChannel, inRedirectChannel, inPeer, () => { });
                }
                InsertlogTranfers(commercial);
                result.Status = ResultStatus.SUCCESS;
                return result;
            }
            catch (AmiException ex)
            {
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[AMIExcep.] " + ex.Message;
                return result;
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.ERROR;
                result.Message = "[Excep.] " + e.Message;
                return result;
            }
        }

        /// <summary>
        /// Permite obtener el id de la llamada en base a una session del comercial.
        /// </summary>
        /// <param name="id">SessionId del comercial.</param>
        /// <param name="inboundOnly">Booleano para indicar si solo debe tener en cuenta las llamadas entrantes (true) o todas (false).</param>
        /// /// <param name="isRecordCall">Booleano para indicar si la petición proviene del ws /api/calls/record (true) (false).</param>
        /// <returns></returns>
        public GetCallIdBySessionIdResponseModel GetCallIdBySessionId(string id, bool inboundOnly = true, bool isRecordCall = false)
        {
            var responseModel = new GetCallIdBySessionIdResponseModel

            {
                Status = ResultStatus.SUCCESS
            };

            try
            {
                var context = _callsRepository.GetContext();
                var sessionDB = _sessionRepository.SingleOrDefault(c => c.IdExternal == id);
                if (sessionDB == null)
                {
                    responseModel.Status = ResultStatus.NOT_FOUND;
                    responseModel.Message = string.Format("No se encontró la sesion identificada Id = {0}.", id);
                    return responseModel;
                }

                var tableCallsPart = _callPartsRepository.GetTable();
                var queryCallParts = tableCallsPart.Include(cp => cp.Call)
                                                   .Where(cp => cp.CommercialId == sessionDB.CommercialId);
                if (inboundOnly)
                {
                    queryCallParts = queryCallParts.Where(cp => cp.Call.Direction == CallDirection.Inbound);
                }

                List<CallParts> resultParts = queryCallParts.OrderBy(c => c.CallId)
                                                             .ThenBy(c => Convert.ToInt32(c.CallPartNumber))
                                                            .ToList();

                if (resultParts != null && resultParts.Count > 0)
                {
                    CallParts lastPart = resultParts.Last();
                    responseModel.CallId = lastPart.CallId;
                    responseModel.OrigChannel = lastPart.OrigChannel;
                    responseModel.RedirectChannel = lastPart.RedirectChannel;
                    responseModel.Status = ResultStatus.SUCCESS;

                    if (isRecordCall)
                    {
                        //Guardar datos de la grabacion en CallsRecords
                        CallsRecords callsrecords = new CallsRecords();
                        callsrecords.CallId = lastPart.CallId;
                        callsrecords.CallPartNumber = lastPart.CallPartNumber;
                        callsrecords.Record = inboundOnly;

                        _storeProcedureRepository.SaveCallRecord(callsrecords, context.Database.GetDbConnection().ConnectionString);
                    }
                }
                else
                {
                    responseModel.Status = ResultStatus.NOT_FOUND;
                    responseModel.Message = string.Format("No se encontraron llamadas con la session Id = {0}.", id);
                }

                

            }
            catch (Exception e)
            {
                responseModel.Status = ResultStatus.ERROR;
                responseModel.Message = "Excep.: " + e.Message;
                if (e.InnerException != null)
                {
                    responseModel.Message = " InnerExcep.: " + e.InnerException.Message;
                }
            }
            return responseModel;
        }

        public StatusResponseModel RecordCall(string origChannel, bool record)
        {
            var result = new StatusResponseModel();
            PbxConnection pbxConnection = null;

            try
            {
                // Conexion a PBX
                if (_pbxConnect)
                {
                    pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);
                    if (!pbxConnection.IsInitialized())
                    {
                        result.Message = pbxMsg;
                        result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                        return result;
                    }

                    // validacion instancia PBX
                    if (pbxConnection == null)
                    {
                        result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                        result.Message = "No se pudo instanciar PBX";
                        return result;
                    }

                    if (record)
                    {
                        pbxConnection.ResumeCall(origChannel, () => { });
                    }
                    else
                    {
                        pbxConnection.PauseCall(origChannel, () => { });
                    }
                }

                result.Status = ResultStatus.SUCCESS;
                return result;
            }
            catch (AmiException ae)
            {
                result.Message = ae.Message;
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                return result;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.Status = ResultStatus.ERROR;
                return result;
            }
        }
        private async Task<StatusResponseModel> EndCallSiebel(string inEndPoint, SiebelRequestEndCallModel inCallData)
        {
            StatusResponseModel result = new StatusResponseModel();

            try
            {
                // Don't invoke Siebel: More information see task Jira ICBB-852
                //SE HABILITO EL 08072020
                //SAStatusResponse response = await _siebelClient.EndCall(inEndPoint, inCallData).ConfigureAwait(false);

                // ICBB-852
                // Simulates OK invocation to Siebel.
                SAStatusResponse response = new SAStatusResponse()
                {
                    returnCode = "0",
                    returnMsg = "[ENDCALL] - No se notifica a Siebel."
                };

                #region SiebelLog
                string strSiebelRequest = System.Text.Json.JsonSerializer.Serialize(inCallData);
                string strSiebelResponse = System.Text.Json.JsonSerializer.Serialize(response);

                // ICBB-852
                // Don't write log.
                _siebelLog.Write(inEndPoint, strSiebelRequest, strSiebelResponse);
                #endregion

                if (response == null || string.IsNullOrWhiteSpace(response.returnCode))
                {
                    result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                    result.Message = "Error desconocido en Servicio Siebel";
                    return result;
                }

                // Control de errores
                if (Enum.TryParse(response.returnCode, out SiebelResponse resultSRCode))
                {
                    switch (resultSRCode)
                    {
                        case SiebelResponse.OK:
                            result.Status = ResultStatus.SUCCESS;
                            break;

                        default:
                            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                            result.Message = "[" + response.returnCode + "] " + response.returnMsg;
                            return result;
                    }
                    return result;
                }
                else
                {
                    // Errores no contemplados en la clase "SiebelResponse"
                    result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                    result.Message = "[" + response.returnCode + "] " + response.returnMsg;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[Excep.] " + e.Message;
                return result;
            }
        }

        /// <summary>
        /// Retorna los comerciales obteniendolos de las callParts.
        /// </summary>
        /// <returns>
        /// Modelo <see cref="GetCommercialsResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public GetCommercialsResponseModel GetCommercialsFromCallParts()
        {
            var result = new GetCommercialsResponseModel()
            {
                Commercials = null,
                Status = ResultStatus.ERROR
            };

            try
            {
                result.Commercials = _callsRepository.GetTable().Include(c => c.CallParts)
                   .Where(c => c.CallParts != null && c.CallParts.Count > 0)
                   .SelectMany(c => c.CallParts)
                   .Where(cp => cp.CommercialId != 0 && !string.IsNullOrEmpty(cp.CommercialName))
                   .ToList()
                   .GroupBy(cp => cp.CommercialId)
                   .Select(g => new CommercialModel()
                   {
                       CommercialId = g.Key,
                       CommercialName = g.FirstOrDefault() != null ? g.FirstOrDefault().CommercialName : ""
                   })
                   .ToList();

                result.Status = ResultStatus.SUCCESS;
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.ERROR;
            }

            return result;
        }

        /// <summary>
        /// Retorna los clientes obteniendolos de las calls.
        /// </summary>
        /// <returns>
        /// Modelo <see cref="GetCustomersFromCalls"/> con los datos de la respuesta.
        /// </returns>
        public GetCustomersResponseModel GetCustomersFromCalls()
        {
            var result = new GetCustomersResponseModel()
            {
                Customers = null,
                Status = ResultStatus.ERROR
            };

            try
            {
                result.Customers = _callsRepository.GetAll()
                   .Where(c => !string.IsNullOrEmpty(c.CustomerId) && !string.IsNullOrEmpty(c.CustomerName))
                   .ToList()
                   .GroupBy(c => c.CustomerId)
                   .Where(g => g.Count() > 0)
                   .Select(g => new CustomerModel()
                   {
                       CustomerId = g.Key,
                       CustomerName = g.LastOrDefault().CustomerName
                   })
                   .OrderBy(c => Utilities.RemoveDiacritics(c.CustomerName))
                   .ToList();

                result.Status = ResultStatus.SUCCESS;
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.ERROR;
            }

            return result;
        }
        public void InsertLogCallIn(Calls call, Commercials commercials)
        {
            
            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log Llamada",
                Description = "Llamada Entrante",
                HourLog = dtNow,
                UserId = commercials.Id,
                CallsId = call.Id

             };
                _logsRepository.Insert(LogPetition);
                _logsRepository.Save();
            
        }
        public void InsertLogCallOut(Calls call, Commercials commercials)
        {

            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log Llamada",
                Description = "Llamada Saliente",
                HourLog = dtNow,
                UserId = commercials.Id,
                CallsId = call.Id

            };
            _logsRepository.Insert(LogPetition);
            _logsRepository.Save();

        }
        public void InsertlogTranfers(Commercials commercial)
        {

            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log Llamada",
                Description = "Tranferencia De Llamada",
                HourLog = dtNow,
                UserId = commercial.Id,
                CallsId = null

            };
            _logsRepository.Insert(LogPetition);
            _logsRepository.Save();

        }

    }
}