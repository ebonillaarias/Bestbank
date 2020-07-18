using inConcert.iMS.BusinessLogic.Logs;
using inConcert.iMS.BusinessLogic.Services;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.ServiceAgent.FirebaseCM;
using inConcert.iMS.ServiceAgent.Siebel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace inConcert.iMS.iPBX.Api
{
   public static class ServiceCollectionExtensions
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="services"></param>
      /// <param name="configuration"></param>
      public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
      {
         services.AddScoped<ISiebelLog>(s => new SiebelLog(configuration.GetValue<string>("ExternalServicesHosts:Siebel:BaseAddress")));
         services.AddScoped<IGenericRepository<Commercials>, GenericRepository<Commercials>>();
         services.AddScoped<IGenericRepository<Calls>, GenericRepository<Calls>>();
         services.AddScoped<IGenericRepository<CallParts>, GenericRepository<CallParts>>();
         services.AddScoped<ISecurityService, SecurityService>();
         services.AddScoped<IGenericRepository<CallsCustomers>, GenericRepository<CallsCustomers>>();
         services.AddScoped<IGenericRepository<CustomersPhones>, GenericRepository<CustomersPhones>>();
         services.AddScoped<IGenericRepository<Sessions>, GenericRepository<Sessions>>();
         services.AddScoped<IGenericRepository<LogsGenerals>, GenericRepository<LogsGenerals>>();
            services.AddScoped<IStoreProcedureRepository, StoreProcedureRepository>();

            var pbxConnectionPwdEncrypted = configuration.GetSection("PBXConnection:Pwd").Value;
         var pbxConnectionPwdDecrypted = Utils.Decrypt(pbxConnectionPwdEncrypted);
         
         services.AddScoped<IBOCommercialsService>(s => new BOCommercialsService(s.GetService<IGenericRepository<Sessions>>(),
                                                                                 s.GetService<IGenericRepository<Commercials>>(),
                                                                                 s.GetService<IGenericRepository<AlternativeCommercials>>(),
                                                                                 s.GetService<IGenericRepository<LogsGenerals>>(),
                                                                                 s.GetService<ISecurityService>(),
                                                                                 configuration.GetSection("PBXConnection:PrivateServer").Value,
                                                                                 configuration.GetSection("PBXConnection:User").Value,
                                                                                 pbxConnectionPwdDecrypted,
                                                                                 configuration.GetSection("PBXConnection:ConnectPBX").Value));
         services.AddScoped<IBOCallsService>(c => new BOCallsService(c.GetService<IGenericRepository<Calls>>(),
                                                                     c.GetService<IGenericRepository<Commercials>>(),
                                                                     c.GetService<IGenericRepository<CallParts>>(),
                                                                     c.GetService<ISiebelClient>(),
                                                                     c.GetService<IFirebaseClient>(),
                                                                     c.GetService<ISiebelLog>(),
                                                                     c.GetService<IGenericRepository<CallsCustomers>>(),
                                                                     c.GetService<IGenericRepository<Sessions>>(),
                                                                     c.GetService<IGenericRepository<LogsGenerals>>(),
                                                                     c.GetService<IGenericRepository<CustomersPhones>>(),
                                                                     configuration.GetSection("PBXConnection:PrivateServer").Value,
                                                                     configuration.GetSection("PBXConnection:User").Value,
                                                                     pbxConnectionPwdDecrypted,
                                                                     configuration.GetSection("PBXConnection:ConnectPBX").Value,
                                                                     c.GetService<IStoreProcedureRepository>()));
      }
   }
}
