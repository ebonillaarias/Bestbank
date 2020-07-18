using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace inConcert.iMS.BusinessLogic.Services
{
   public class CommercialService : ICommercialService
   {
      private readonly IGenericRepository<Commercials> _commercialRepository;

      public CommercialService(IGenericRepository<Commercials> commercialRepository)
      {
         _commercialRepository = commercialRepository;
      }

      /// <summary>
      /// Permite obtener el listado de comerciales.
      /// </summary>
      public GetCommercialsResponseModel GetCommercials()
      {
         GetCommercialsResponseModel modelReturn = new GetCommercialsResponseModel()
         {
            Status = ResultStatus.ERROR
         };

         try
         {
            // Lista que contendrá los comerciales.
            List<Commercials> commercialsDB = new List<Commercials>();

            // Busco en el repositorio y lo agrego a la lista de comerciales
            commercialsDB.AddRange(_commercialRepository.FindAll(c=> c.Active == true).OrderBy(c => c.Name).ToList());
            modelReturn.Commercials = commercialsDB.ConvertAll(c =>
               new CommercialModel()
               {
                  CommercialId = c.Id,
                  CommercialName = String.Format("{0}", c.Name),
                  CommercialEmail = c.Email,
                  Peer = c.Peer,
                  SiebelId = c.SiebelId,
                  PBXPhoneNumber = c.PBXPhoneNumber,
                  MobilePhoneNumber = c.MobilePhoneNumber,
                  Active = c.Active
               });

            modelReturn.Status = ResultStatus.SUCCESS;
            return modelReturn;
         }
         catch (Exception e)
         {
            modelReturn.Message = e.Message;
            if (e.InnerException != null)
               modelReturn.Message += " InnerException: " + e.InnerException.Message;

            return modelReturn;
         }
      }
   }
}
