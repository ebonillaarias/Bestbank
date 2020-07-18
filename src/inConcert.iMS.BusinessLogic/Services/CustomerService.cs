using inConcert.iMS.BusinessLogic.Logs;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using inConcert.iMS.ServiceAgent.Siebel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.BusinessLogic.Services
{
   public class CustomerService : ICustomerService
   {
      private readonly ISiebelClient _siebelClient;
      private readonly ISiebelLog _siebelLog;
      private readonly IGenericRepository<CallsCustomers> _callsCustomersRepository;
      private readonly IGenericRepository<CustomersPhones> _callsCustomersPhoneRepository;
      private readonly IGenericRepository<Commercials> _commercialRepository;
      private readonly IGenericRepository<Sessions> _sessionsRepository;
      private readonly IGenericRepository<Calls> _callsRepository;
      private readonly IGenericRepository<CallParts> _callPartsRepository;

      public CustomerService(ISiebelClient siebelClient, ISiebelLog siebelLog, IGenericRepository<CallsCustomers> callsCustomersRepository, IGenericRepository<CustomersPhones> callsCustomersPhoneRepository, IGenericRepository<Commercials> commercialRepository, IGenericRepository<Sessions> sessionsRepository, IGenericRepository<CallParts> callPartsRepository, IGenericRepository<Calls> callsRepository)
      {
         _siebelClient = siebelClient;
         _siebelLog = siebelLog;
         _callsCustomersRepository = callsCustomersRepository;
         _callsCustomersPhoneRepository = callsCustomersPhoneRepository;
         _commercialRepository = commercialRepository;
         _callsRepository = callsRepository;
         _callPartsRepository = callPartsRepository;
         _sessionsRepository = sessionsRepository;

      }

      private async Task<GetCustomersResult> GetCustomersBySiebelIdProcess(string endPoint, string siebelId)
      {
         GetCustomersResult result = new GetCustomersResult();

         SAResponseData siebelResponse = await _siebelClient.GetCustomers(endPoint, siebelId).ConfigureAwait(false);
         #region SiebelLog
         string strSiebelRequest = System.Text.Json.JsonSerializer.Serialize(siebelId);
         string strSiebelResponse = System.Text.Json.JsonSerializer.Serialize(siebelResponse);
         _siebelLog.Write(endPoint, strSiebelRequest, strSiebelResponse);
         #endregion

         if (siebelResponse == null || string.IsNullOrWhiteSpace(siebelResponse.returnCode))
         {
            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
            result.Message = "Error desconocido en Servicio Siebel";
            return result;
         }

         // Control de errores
         if (Enum.TryParse(siebelResponse.returnCode, out SiebelResponse resultSRCode))
         {
            switch (resultSRCode)
            {
               case SiebelResponse.OK:
                  break;

               case SiebelResponse.ECTI002:
                  result.Status = ResultStatus.NOT_FOUND;
                  return result;

               default:
                  result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
                  result.Message = "[" + siebelResponse.returnCode + "] " + siebelResponse.returnMsg;
                  return result;
            }
         }
         else
         {
            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
            result.Message = "[" + siebelResponse.returnCode + "] " + siebelResponse.returnMsg;
            return result;
         }

         List<CustomerModel> customerList = new List<CustomerModel>();
         if (siebelResponse.outputData != null &&
             siebelResponse.outputData.ListOfCustomer != null &&
             siebelResponse.outputData.ListOfCustomer.Customer != null &&
             siebelResponse.outputData.ListOfCustomer.Customer.Count > 0)
         {
            siebelResponse.outputData.ListOfCustomer.Customer.OrderBy(c => c.CustomerName).ToList().ForEach(customer =>
            {
               List<PhoneNumberModel> phoneNumberListAux = new List<PhoneNumberModel>();
               if (customer.ListOfPhoneNumber != null && customer.ListOfPhoneNumber.PhoneNumber != null)
               {
                  customer.ListOfPhoneNumber.PhoneNumber.ForEach(phoneNumber =>
                  {
                     phoneNumberListAux.Add(new PhoneNumberModel
                     {
                        PhoneNumber = phoneNumber.PhoneNumber,
                        PhoneType = Utilities.GetPhoneType(phoneNumber.PhoneType)
                     });
                  });
               }

               customerList.Add(new CustomerModel
               {
                  CustomerId = customer.CustomerId,
                  CustomerName = customer.CustomerName,
                  CustomerType = Utilities.GetCustomerType(customer.CustomerType),
                  PhoneNumberList = phoneNumberListAux
               });
            });
         }

         result.Status = ResultStatus.SUCCESS;
         result.CustomerList = customerList;
         return result;
      }

      public async Task<GetCustomersResult> GetCustomersBySiebelId(string endPoint, string userId)
      {
         GetCustomersResult result = new GetCustomersResult();

         try
         {
            result = await GetCustomersBySiebelIdProcess(endPoint, userId).ConfigureAwait(false);
            return result;
         }
         catch (Exception e)
         {
            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
            result.Message = "[CustomerService-GetCustomersBySiebelId-Exception] " + e.Message;
            return result;
         }
      }

      public async Task<GetCustomersResult> GetCustomersByCommercialId(string endPoint, string commercialId)
      {
         GetCustomersResult result = new GetCustomersResult();

         try
         {
            var siebelId = _commercialRepository.SingleOrDefault(item => item.Id.Equals(int.Parse(commercialId))).SiebelId;
            result = await GetCustomersBySiebelIdProcess(endPoint, siebelId).ConfigureAwait(false);
            return result;
         }
         catch (Exception e)
         {
            result.Status = ResultStatus.EXTERNAL_SERVICE_ERROR;
            result.Message = "[Excep.] " + e.Message;
            return result;
         }
      }

      /// <summary>
      /// Retorna el listado de clientes segun la llamada indicada
      /// </summary>
      /// <param name="id">Id de la llamada.</param>
      /// <returns>
      /// Modelo <see cref="GetCustomersByCallResponseModel"/> con los datos de la respuesta.
      /// </returns>
      public GetCustomersByCallResponseModel GetCustomersByCallId(int id)

      {
         var responseModel = new GetCustomersByCallResponseModel

         {
            Status = ResultStatus.SUCCESS
         };
         try
         {
            var context = _callsCustomersRepository.GetContext();
            using (var dbTransaction = context.Database.BeginTransaction())
            {

               var tableCustomer = _callsCustomersRepository.GetTable();
               List<CallsCustomers> callCustomersDB = tableCustomer.Include(c => c.CustomerPhone).Where(c => c.CallId == id).OrderBy(c => c.CustomerName).ToList();

               if (callCustomersDB == null || callCustomersDB.Count == 0)
               {
                  responseModel.Status = ResultStatus.NOT_FOUND;
                  responseModel.Message = "No data for this call.";
                  return responseModel;
               }
               responseModel.Customers = callCustomersDB.ConvertAll(cp =>
                     new CallsCustomersModel
                     {
                        CallId = cp.CallId,
                        CustomerId = cp.CustomerId,
                        CustomerName = cp.CustomerName,
                        CustomerType = Utilities.GetCustomerType(cp.CustomerType),
                        PhoneNumberList = new List<CustomersPhonesModel>(cp.CustomerPhone.ConvertAll(cpm =>
                                                                                   new CustomersPhonesModel()
                                                                                   {
                                                                                      Id = cpm.Id,
                                                                                      CustomerId = cpm.CustomerId,
                                                                                      PhoneNumber = cpm.PhoneNumber,
                                                                                      PhoneType = cpm.PhoneType,
                                                                                   })),

                     });

            }
         }
         catch (Exception e)
         {
            responseModel.Status = ResultStatus.ERROR;
            responseModel.Message = e.Message;
         }
         return responseModel;
      }
   }
}
