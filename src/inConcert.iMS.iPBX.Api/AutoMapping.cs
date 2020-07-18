using AutoMapper;
using inConcert.iMS.Domain;
using inConcert.iMS.iPBX.Api.DataTransferObjects;

namespace inConcert.iMS.iPBX.Api
{
   public class AutoMapping : Profile
   {
      public AutoMapping()
      {
         CreateMap<StatusResponseModel, StatusResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
      }
   }
}
