using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Exceptions;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using inConcert.iMS.ServiceAgent.Exceptions;
using inConcert.iMS.ServiceAgent.PBX;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace inConcert.iMS.BusinessLogic.Services
{
    public class BOCommercialsService : IBOCommercialsService
    {
        private readonly IGenericRepository<Commercials> _commercialRepository;
        private readonly IGenericRepository<AlternativeCommercials> _alternativeCommercialsRepository;
        private readonly IGenericRepository<Sessions> _sessionRepository;
        private readonly IGenericRepository<LogsGenerals> _logsgeneralRepository;
        private readonly ISecurityService _securityService;
        private readonly string _pbxServer;
        private readonly string _pbxUser;
        private readonly string _pbxPwd;

        /// <summary>
        /// Indica si se debe realizar conexion con la PBX
        /// </summary>
        private readonly bool _pbxConnect;

        public BOCommercialsService(IGenericRepository<Sessions> sessionRepository, IGenericRepository<Commercials> commercialRepository, IGenericRepository<AlternativeCommercials> alternativeCommercialsRepository, IGenericRepository<LogsGenerals> logsgeneral, ISecurityService securityService, string pbxServer, string pbxUser, string pbxPwd, string pbxConnect)
        {
            _sessionRepository = sessionRepository;
            _commercialRepository = commercialRepository;
            _alternativeCommercialsRepository = alternativeCommercialsRepository;
            _logsgeneralRepository = logsgeneral;
            _securityService = securityService;
            _pbxServer = pbxServer;
            _pbxUser = pbxUser;
            _pbxPwd = pbxPwd;

            _pbxConnect = false;
            if (Int32.TryParse(pbxConnect, out int _aux))
            {
                _pbxConnect = _aux == 1 ? true : false;
            }
        }

        /// <summary>
        /// <para>
        /// Busca todos los comerciales en los cuales el campo 'Name' coincide y/o contiene la palabra
        /// clave indicada por el parametro <paramref name="strName"/>.
        /// </para>
        /// <para>
        /// Si <paramref name="strName"/> es NULL, string vacio o string blanco (todos espacios)
        /// se recuperan todos los comerciales.
        /// </para>
        /// </summary>
        /// <param name="strName">Palabra clave a buscar en los nombres de los comerciales.</param>
        /// <param name="pMaxTimeKeepAlive">Tiempo máximo (en minutos, 1 por defecto) por el cuál se asume que un comercial esta conectado tras el último KeepAlive.</param>
        /// <returns>
        /// Modelo <see cref="BOGetCommercialsResponseModel"/> con los datos de la respuesta.
        /// </returns>
        public BOGetCommercialsResponseModel GetCommercials(string strName = null, uint pMaxTimeKeepAlive = 1)
        {
            BOGetCommercialsResponseModel commercialsModel = new BOGetCommercialsResponseModel
            {
                commercials = new List<CommercialModel>(),
                status = ResultStatus.SUCCESS
            };

            try
            {
                // Lista que contendrá los comerciales que cuyos campo 'Name' contengan el filtro 'strName'.
                // Busqueda similar al LIKE.
                List<Commercials> commercialsDB = new List<Commercials>();

                // Si 'strName" es null o string vacio o string blanco => debo recuperar todos los comerciales
                // en otro caso => debo realizar el filtrado.
                if (string.IsNullOrWhiteSpace(strName))
                {
                    var tablaCommerciales = _commercialRepository.GetTable();
                    var commercials = tablaCommerciales.Include(c => c.Session);
                    commercialsDB.AddRange(commercials.Cast<Commercials>().ToList());
                }
                else
                {
                    // Elimino los espacios extras (tanto del inicio, entre palabras y al final)
                    // del parametro de entrada
                    string filtro = NormalizeWhiteSpace(strName);

                    // Declaro una variable Func y asigno una expresión lambda a la variable.               
                    var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
                    Func<Commercials, bool> selector = c => compareInfo.IndexOf(c.Name, filtro, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) > -1;

                    // Busco en el repositorio de acuerdo al 'selector' actual y lo agrego a la lista de comerciales
                    var tablaCommerciales = _commercialRepository.GetTable();
                    var commercials = tablaCommerciales.Include(c => c.Session).Where(selector);

                    commercialsDB.AddRange(commercials);
                }

                foreach (Commercials c in commercialsDB)
                {
                    commercialsModel.commercials.Add(new CommercialModel
                    {
                        CommercialId = c.Id,
                        CommercialName = String.Format("{0}", c.Name),
                        CommercialEmail = c.Email,
                        Peer = c.Peer,
                        SiebelId = c.SiebelId,
                        PBXPhoneNumber = c.PBXPhoneNumber,
                        MobilePhoneNumber = c.MobilePhoneNumber,
                        Active = c.Active,
                        Connected = c.Session != null // session exist
                                && !c.Session.EndDate.HasValue // no logout
                                && c.Session.LastKeepAlive.HasValue // LastKeepAlive != null => connected
                                && DateTimeOffset.Now.Subtract(c.Session.LastKeepAlive.Value).TotalMinutes <= pMaxTimeKeepAlive // (DateNow-LastKeepAlive) <= MaxTimeKeepAlive
                    });
                }

                return commercialsModel;
            }
            catch (Exception e)
            {
                return new BOGetCommercialsResponseModel
                {
                    status = ResultStatus.ERROR
                };
            }
        }

        /// <summary> 
        /// <para> 
        /// Busca y devuelve el comercial cuyo campo 'Id' coincide con el id indicado por el parametro <paramref id="intId"/>. 
        /// </para> 
        /// </summary> 
        /// <param id="intId">Identificador del comercial.</param> 
        /// <returns> 
        /// Modelo <see cref="GetBOCommercialByIdResponseModel"/> con los datos de la respuesta. 
        /// </returns> 
        public GetBOCommercialByIdResponseModel GetCommercialById(int id)
        {
            GetBOCommercialByIdResponseModel result = new GetBOCommercialByIdResponseModel
            {
                status = ResultStatus.SUCCESS
            };

            try
            {
                Commercials commercial = _commercialRepository.GetById(id);
                if (commercial != null)
                {
                    var alternativeCommercialsDB = new List<AlternativeCommercials>();
                    var alternativeCommercials = new List<AlternativeCommercialModel>();
                    Commercials c;

                    Func<AlternativeCommercials, bool> selector = c => c.CommercialId == commercial.Id;
                    alternativeCommercialsDB.AddRange(_alternativeCommercialsRepository.FindAll(selector).Cast<AlternativeCommercials>().ToList());

                    foreach (AlternativeCommercials alternativeCommercial in alternativeCommercialsDB)
                    {
                        c = _commercialRepository.GetById(alternativeCommercial.AlternativeCommercialId);

                        alternativeCommercials.Add(
                           new AlternativeCommercialModel
                           {
                               Commercial = new CommercialModel
                               {
                                   CommercialId = c.Id,
                                   CommercialName = String.Format("{0}", c.Name),
                                   CommercialEmail = c.Email,
                                   Peer = c.Peer,
                                   SiebelId = c.SiebelId,
                                   PBXPhoneNumber = c.PBXPhoneNumber,
                                   MobilePhoneNumber = c.MobilePhoneNumber,
                                   Active = c.Active
                               },
                               Order = alternativeCommercial.Order
                           }
                        );
                    }

                    result.Commercial = new CommercialModel
                    {
                        CommercialId = commercial.Id,
                        CommercialName = String.Format("{0}", commercial.Name),
                        CommercialEmail = commercial.Email,
                        Peer = commercial.Peer,
                        SiebelId = commercial.SiebelId,
                        PBXPhoneNumber = commercial.PBXPhoneNumber,
                        MobilePhoneNumber = commercial.MobilePhoneNumber,
                        Active = commercial.Active,
                        Alternatives = alternativeCommercials
                    };
                }
                else
                {
                    result.status = ResultStatus.NOT_FOUND;
                }
                return result;
            }
            catch (Exception e)
            {
                return new GetBOCommercialByIdResponseModel
                {
                    status = ResultStatus.ERROR
                };
            }
        }

        /// <summary> 
        /// <para> 
        /// Busca y devuelve el comercial cuyo campo 'Peer' coincide con el Peer indicado por el parametro <paramref name="peer"/>. 
        /// </para> 
        /// </summary> 
        /// <param name="peer">Identificador peer del comercial.</param> 
        /// <returns> 
        /// Modelo <see cref="GetBOCommercialByPeerResponseModel"/> con los datos de la respuesta. 
        /// </returns> 
        public GetBOCommercialByPeerResponseModel GetCommercialByPeer(int peer)
        {
            var result = new GetBOCommercialByPeerResponseModel
            {
                status = ResultStatus.SUCCESS
            };

            try
            {
                Commercials commercial = _commercialRepository.GetAll().SingleOrDefault(c => c.Peer == peer);

                if (commercial != null)
                {
                    result.Commercial = new CommercialModel
                    {
                        CommercialId = commercial.Id,
                        CommercialName = String.Format("{0}", commercial.Name),
                        CommercialEmail = commercial.Email,
                        Peer = commercial.Peer,
                        SiebelId = commercial.SiebelId,
                        PBXPhoneNumber = commercial.PBXPhoneNumber,
                        MobilePhoneNumber = commercial.MobilePhoneNumber,
                        Active = commercial.Active
                    };
                }
                else
                {
                    result.status = ResultStatus.NOT_FOUND;
                }
                return result;
            }
            catch (Exception e)
            {
                return new GetBOCommercialByPeerResponseModel
                {
                    status = ResultStatus.ERROR
                };
            }
        }

        /// <summary> 
        /// <para> 
        /// Elimina el comercial cuyo campo 'Id' coincide con el id indicado por el parametro <paramref id="intId"/>. 
        /// </para> 
        /// </summary> 
        /// <param name="id">Identificador del comercial.</param> 
        /// <returns> 
        /// Modelo <see cref="DeleteBOCommercialsResponseModel"/> con los datos de la respuesta. 
        /// </returns> 
        public StatusResponseModel DeleteCommercial(int id)
        {
            StatusResponseModel result = new StatusResponseModel
            {
                Status = ResultStatus.ERROR
            };

            IDbContextTransaction dbTransaction = null;
            try
            {
                var context = _commercialRepository.GetContext();
                using (dbTransaction = context.Database.BeginTransaction())
                {
                    // Issue ICCB-735
                    Sessions session = _sessionRepository.SingleOrDefault(s => s.CommercialId == id);
                    if (session != null)
                    {
                        _sessionRepository.Delete(session);
                        _sessionRepository.Save();
                    }

                    Commercials toDelete = _commercialRepository.SingleOrDefault(c => c.Id == id);
                    if (toDelete == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = string.Format("No records found for the commercial (id:{0}).", id);
                        return result;
                    }

                    RemoveThisComercialAsAlternative(id);
                    RemoveAlternativesOfThisComercial(id);

                    _commercialRepository.Delete(toDelete);
                    _commercialRepository.Save();

                    // Conexión a PBX
                    PbxConnection pbxConnection = null;
                    if (_pbxConnect)
                    {
                        pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);

                        // validacion instancia PBX
                        if (pbxConnection == null)
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                            result.Message = "[BOCommercialsService-UpdateCommercial] Could not instantiate PBX";
                            return result;
                        }

                        // validacion PBX inicializada
                        if (!pbxConnection.IsInitialized())
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                            result.Message = pbxMsg;
                            return result;
                        }

                        // petición Delete a la PBX
                        pbxConnection.Delete(toDelete.Peer, () => { });
                    }

                    dbTransaction.Commit();
                    result.Status = ResultStatus.SUCCESS;
                    return result;
                }
            }
            catch (AmiException amiEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[BOCommercialsService-DeleteCommercial-AmiException] " + amiEx.Message;
                return result;
            }
            catch (Exception e)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.ERROR;
                result.Message = "[BOCommercialsService-DeleteCommercial-Exception] " + e.Message;
                return result;
            }
        }

        /// <summary> 
        /// <para> 
        /// Da de alta el comercial pasado por parámetro <paramref name="commercialRequestModel"/>. 
        /// </para> 
        /// </summary> 
        /// <param name="commercialRequestModel">Datos del comercial que debe ser dado de alta.</param> 
        /// <returns> 
        /// Modelo <see cref="PostBOCommercialsResponseModel"/> con los datos de la respuesta. 
        /// </returns> 
        public PostBOCommercialsResponseModel CreateCommercial(PostBOCommercialsRequestModel commercialRequestModel)
        {
            PostBOCommercialsResponseModel result = new PostBOCommercialsResponseModel
            {
                Status = ResultStatus.ERROR
            };

            IDbContextTransaction dbTransaction = null;
            try
            {
                var context = _commercialRepository.GetContext();
                using (dbTransaction = context.Database.BeginTransaction())
                {
                    // Generate random pass
                    commercialRequestModel.CommercialPassword = (new Random().Next(10000000, 100000000)).ToString();

                    var toInsert = new Commercials()
                    {
                        Name = commercialRequestModel.CommercialName,
                        Email = commercialRequestModel.CommercialEmail,
                        PBXPhoneNumber = commercialRequestModel.PBXPhoneNumber,
                        MobilePhoneNumber = commercialRequestModel.MobilePhoneNumber,
                        Active = commercialRequestModel.Active,
                        Password = _securityService.HashData(commercialRequestModel.CommercialPassword),
                        Peer = GetPeer(),
                        SiebelId = commercialRequestModel.SiebelId,
                        PasswordChangeRequired = true,
                        AlternativeCommercials = new List<AlternativeCommercials>()
                    };

                    // Se da de alta en BD al nuevo comercial
                    _commercialRepository.Insert(toInsert);
                    _commercialRepository.Save();

                    foreach (AlternativeCommercialModel c in commercialRequestModel.Alternatives)
                    {
                        var alter = _commercialRepository.GetById(c.Commercial.CommercialId);
                        toInsert.AlternativeCommercials.Add(new AlternativeCommercials()
                        {
                            CommercialId = toInsert.Id,
                            AlternativeCommercialId = c.Commercial.CommercialId,
                            Order = c.Order
                        });
                    }

                    // Se agregan los comerciales alternativos al nuevo comercial
                    _commercialRepository.Save();

                    // Conexión a PBX
                    PbxConnection pbxConnection = null;
                    if (_pbxConnect)
                    {
                        pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);

                        // validacion instancia PBX
                        if (pbxConnection == null)
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                            result.Message = "[BOCommercialsService-CreateCommercial] Could not instantiate PBX";
                            return result;
                        }

                        // validacion PBX inicializada
                        if (!pbxConnection.IsInitialized())
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                            result.Message = "[BOCommercialsService-CreateCommercial] " + pbxMsg;
                            return result;
                        }

                        // petición a la PBX
                        try
                        {
                            pbxConnection.New(toInsert.Peer, toInsert.Name, toInsert.Password, () => { });
                        }
                        catch (AmiException ex)
                        {
                            dbTransaction.Rollback();

                            // Marchelo
                            // No estoy de acuerdo con retonar este error (lo correcto es utilizar el "catch (AmiException amiEx)" mas abajo)
                            // Simplemente lo dejo por consistencia con la capa HTTP.
                            result.Status = ResultStatus.PEER_ALREADY_EXIST_IN_PBX;
                            result.Message = "[BOCommercialsService-CreateCommercial] " + ex.Message;
                            return result;
                        }
                    }

                    result.Status = ResultStatus.SUCCESS;
                    result.Name = commercialRequestModel.CommercialName;
                    result.Email = commercialRequestModel.CommercialEmail;
                    result.Password = commercialRequestModel.CommercialPassword;
                    result.Peer = toInsert.Peer;

                    dbTransaction.Commit();
                    return result;
                }
            }
            catch (DbUpdateException updEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();

                SqlException sqlEx = updEx.GetBaseException() as SqlException;
                if (sqlEx != null)
                {
                    switch (sqlEx.Number)
                    {
                        case 515: // NOT NULL values
                                  // Determino nombre de la columna que no acepta valor NULL
                                  // EJ: "Cannot insert the value NULL into column 'SiebelId', table 'InConcert.dbo.Commercials'."
                            int indexStart = sqlEx.Message.IndexOf("'", 0) + 1;
                            int indexEnd = sqlEx.Message.IndexOf("'", indexStart);
                            string columName = sqlEx.Message[indexStart..indexEnd];

                            // Determino nombre de la tabla
                            indexStart = sqlEx.Message.IndexOf(".dbo.", 0) + 1;
                            indexEnd = sqlEx.Message.IndexOf("'", indexStart);
                            string tableName = sqlEx.Message[indexStart..indexEnd];

                            result.Status = ResultStatus.NOT_NULL;
                            result.Message = string.Format("Cannot insert the value NULL into column '{0}', table '{1}'.", columName, tableName);
                            break;

                        case 2601: // Duplicated key row error
                            string errorEmail = "IX_Commercials_Email";
                            string errorPeer = "IX_Commercials_Peer";
                            string errorPbx = "IX_Commercials_PBXPhoneNumber";
                            string errorMobile = "IX_Commercials_MobilePhoneNumber";
                            string errorSiebel = "IX_Commercials_SiebelId";

                            if (sqlEx.Message.Contains(errorEmail))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_EMAIL;
                                result.Message = "Email is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorPeer))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_PEER;
                                result.Message = "Peer number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorPbx))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_PBX;
                                result.Message = "Pbx phone number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorMobile))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_MOBILE;
                                result.Message = "Mobile phone number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorSiebel))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_SIEBELID;
                                result.Message = "Siebel ID is unique in table Commercials.";
                            }
                            else
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE;
                                result.Message = "[BOCommercialsService-CreateCommercial-SqlException] " + sqlEx.Message;
                            }
                            break;

                        default: // Otros errores no contemplados
                            result.Status = ResultStatus.ERROR;
                            result.Message = "[BOCommercialsService-CreateCommercial-SqlException] " + sqlEx.Message;
                            break;
                    }
                }
                else
                {
                    result.Status = ResultStatus.ERROR;
                    result.Message = "[BOCommercialsService-CreateCommercial-DbUpdateException] " + updEx.Message;
                }
                return result;
            }
            catch (AmiException amiEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();

                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[BOCommercialsService-CreateCommercial-AmiException] " + amiEx.Message;

                return result;
            }
            catch (Exception e)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();

                result.Status = ResultStatus.ERROR;
                result.Message = "[BOCommercialsService-CreateCommercial-Exception] " + e.Message;

                return result;
            }
        }

        private int GetPeer()
        {
            int result = -1;

            const int MIN_PEER = 1100;
            const int MAX_PEER = 2000;

            int current_peer = MIN_PEER;
            bool stop = false;

            BOGetCommercialsResponseModel _BOGetCommercialsResponseModel = GetCommercials();

            List<int> usedPeers = _BOGetCommercialsResponseModel.commercials.OrderBy(c => c.Peer).Select(item => item.Peer).ToList();

            while (current_peer < MAX_PEER && !stop)
            {
                if (usedPeers.Contains(current_peer))
                {
                    usedPeers.Remove(current_peer);
                    current_peer++;
                }
                else
                {
                    result = current_peer;
                    stop = true;
                }
            }

            return result;
        }

        private void RemoveThisComercialAsAlternative(int id)
        {
            var alternativeCommercialsWhitThisCommercialAsAlternative = new List<AlternativeCommercials>();
            Func<AlternativeCommercials, bool> selector = c => c.AlternativeCommercialId == id;
            alternativeCommercialsWhitThisCommercialAsAlternative.AddRange(_alternativeCommercialsRepository.FindAll(selector).ToList());

            List<int> comercialsId = alternativeCommercialsWhitThisCommercialAsAlternative.Select(ac => ac.CommercialId).ToList();

            _alternativeCommercialsRepository.DeleteRange(alternativeCommercialsWhitThisCommercialAsAlternative);
            _alternativeCommercialsRepository.Save();

            foreach (int comercialId in comercialsId)
            {
                var alter = _commercialRepository.GetById(comercialId);

                var newOrder = 1;
                foreach (AlternativeCommercials ac in alter.AlternativeCommercials)
                {
                    ac.Order = newOrder;

                    _alternativeCommercialsRepository.Update(ac);
                    _alternativeCommercialsRepository.Save();

                    newOrder++;
                }
            }
        }

        private void RemoveAlternativesOfThisComercial(int id)
        {
            var alternativeCommercialsDB = new List<AlternativeCommercials>();
            Func<AlternativeCommercials, bool> selector = c => c.CommercialId == id;
            alternativeCommercialsDB.AddRange(_alternativeCommercialsRepository.FindAll(selector).ToList());

            if (alternativeCommercialsDB.Count > 0)
            {
                _alternativeCommercialsRepository.DeleteRange(alternativeCommercialsDB);
                _alternativeCommercialsRepository.Save();
            }
        }

        /// <summary>
        /// Actualizar el comercial especificado
        /// </summary>
        public StatusResponseModel UpdateCommercial(int id, PutCommercialRequestModel commercial)
        {
            StatusResponseModel result = new StatusResponseModel
            {
                Status = ResultStatus.ERROR
            };

            IDbContextTransaction dbTransaction = null;
            try
            {
                var context = _commercialRepository.GetContext();

                using (dbTransaction = context.Database.BeginTransaction())
                {
                    var toUpdate = _commercialRepository.GetById(id);
                    if (toUpdate == null)
                    {
                        result.Status = ResultStatus.NOT_FOUND;
                        result.Message = string.Format("No records found for the commercial (id:{0}).", id);
                        return result;
                    }

                    var alternativeCommercials = _alternativeCommercialsRepository.FindAll(ac => ac.CommercialId == toUpdate.Id);
                    if (alternativeCommercials != null && alternativeCommercials.Count() > 0)
                    {
                        _alternativeCommercialsRepository.DeleteRange(alternativeCommercials);
                        _alternativeCommercialsRepository.Save();
                    }

                    toUpdate.MobilePhoneNumber = commercial.MobilePhoneNumber;
                    toUpdate.Name = commercial.CommercialName;
                    toUpdate.PBXPhoneNumber = commercial.PBXPhoneNumber;
                    toUpdate.Email = commercial.CommercialEmail;
                    toUpdate.Active = commercial.Active;
                    toUpdate.SiebelId = commercial.SiebelId;
                    toUpdate.AlternativeCommercials = commercial.Alternatives.Select(c => new AlternativeCommercials
                    {
                        AlternativeCommercialId = c.AlternativeCommercialId,
                        CommercialId = toUpdate.Id,
                        Order = c.Order
                    }).ToList();

                    _commercialRepository.Update(toUpdate);
                    _commercialRepository.Save();

                    // Conexión a PBX
                    PbxConnection pbxConnection = null;
                    if (_pbxConnect)
                    {
                        pbxConnection = new PbxConnection(_pbxServer, _pbxUser, _pbxPwd, out string pbxMsg);

                        // validacion instancia PBX
                        if (pbxConnection == null)
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                            result.Message = "[BOCommercialsService-UpdateCommercial] Could not instantiate PBX";
                            return result;
                        }

                        // validacion PBX inicializada
                        if (!pbxConnection.IsInitialized())
                        {
                            dbTransaction.Rollback();
                            result.Status = ResultStatus.CANNOT_CONNECT_TO_PBX;
                            result.Message = pbxMsg;
                            return result;
                        }

                        // petición Update a la PBX
                        pbxConnection.Update(toUpdate.Peer, toUpdate.Name, toUpdate.Password, () => { });
                    }

                    // Issue ICBB-650: Force logout
                    if (!commercial.Active)
                    {
                        Sessions currentSession = _sessionRepository.SingleOrDefault(item => item.CommercialId == id);

                        // Valid that there is an active session.
                        if (currentSession != null && currentSession.EndDate == null)
                        {
                            currentSession.EndDate = DateTimeOffset.UtcNow;
                            _sessionRepository.Update(currentSession);
                            _sessionRepository.Save();
                        }
                    }

                    dbTransaction.Commit();
                    result.Status = ResultStatus.SUCCESS;
                    return result;
                }
            }
            catch (DbUpdateException updEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                SqlException sqlEx = updEx.GetBaseException() as SqlException;
                if (sqlEx != null)
                {
                    switch (sqlEx.Number)
                    {
                        case 2601: // Duplicated key row error
                            string errorEmail = "IX_Commercials_Email";
                            string errorPeer = "IX_Commercials_Peer";
                            string errorPbx = "IX_Commercials_PBXPhoneNumber";
                            string errorMobile = "IX_Commercials_MobilePhoneNumber";
                            string errorSiebel = "IX_Commercials_SiebelId";

                            if (sqlEx.Message.Contains(errorEmail))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_EMAIL;
                                result.Message = "Email is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorPeer))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_PEER;
                                result.Message = "Peer number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorPbx))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_PBX;
                                result.Message = "Pbx phone number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorMobile))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_MOBILE;
                                result.Message = "Mobile phone number is unique in table Commercials.";
                            }
                            else if (sqlEx.Message.Contains(errorSiebel))
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE_SIEBELID;
                                result.Message = "Siebel ID is unique in table Commercials.";
                            }
                            else
                            {
                                result.Status = ResultStatus.COMMERCIAL_ROW_DUPLICATE;
                                result.Message = "[BOCommercialsService-UpdateCommercial-SqlException] " + sqlEx.Message;
                            }
                            break;

                        default: // Otros errores no contemplados
                            result.Status = ResultStatus.ERROR;
                            result.Message = "[BOCommercialsService-UpdateCommercial-SqlException] " + sqlEx.Message;
                            break;
                    }
                }
                else
                {
                    result.Status = ResultStatus.ERROR;
                    result.Message = "[BOCommercialsService-UpdateCommercial-DbUpdateException] " + updEx.Message;
                }
                return result;
            }
            catch (AmiException amiEx)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                result.Message = "[BOCommercialsService-UpdateCommercial-AmiException] " + amiEx.Message;
                return result;
            }
            catch (Exception e)
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                result.Status = ResultStatus.ERROR;
                result.Message = "[BOCommercialsService-UpdateCommercial-Exception] " + e.Message;
                return result;
            }
        }

        /// <summary>
        /// Dato un string elimina los espacios extras.
        /// </summary>
        /// <param name="input">String de donde se eliminarán los espacios extras</param>
        /// <returns>String sin espacios extras.</returns>
        private static string NormalizeWhiteSpace(string input)
        {
            var srcAux = input.Trim();
            int len = srcAux.Length;
            int index = 0;

            var src = srcAux.ToCharArray();
            bool skip = false;
            char ch;

            for (int i = 0; i < len; i++)
            {
                ch = src[i];
                switch (ch)
                {
                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':
                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':
                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':
                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':
                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        if (skip) continue;
                        src[index++] = ch;
                        skip = true;
                        continue;
                    default:
                        skip = false;
                        src[index++] = ch;
                        continue;
                }
            }

            return new string(src, 0, index);
        }

        private int sp_DeleteAlternativeCommercials(int pCommercialId, int AlternativeCommercialId)
        {
            int rpta = 0;
            try
            {
                var context = _commercialRepository.GetContext();
                using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteAlternativeCommercials", con))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pCommercialId", SqlDbType.Int).Value = pCommercialId;
                        cmd.Parameters.Add("@AlternativeCommercialId", SqlDbType.Int).Value = AlternativeCommercialId;

                        rpta = cmd.ExecuteNonQuery();
                    }
                }

                return rpta;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

    }
}