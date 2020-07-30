using AutoMapper;
using inConcert.iMS.Api.DataTransferObjects;
using inConcert.iMS.Domain;
using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;

namespace inConcert.iMS.Api
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<PutCallsResponseModel, PutCallsResponse200Dto>().ReverseMap();
            CreateMap<CommercialModel, BOCommercialDto>().ReverseMap();
            CreateMap<AlternativeCommercialModel, BOAlternativeCommercialDto>().ReverseMap();
            CreateMap<CommercialModel, CommercialDto>().ReverseMap();
            CreateMap<BOCallPartsModel, BOCallPartsDto>().ReverseMap();
            CreateMap<CallsRecords, CallsRecodsResponseDto>().ReverseMap(); //Agregado
            CreateMap<BOCallModel, GetBOCallDetailsResponseDto>().ReverseMap();
            CreateMap<BOCallModel, GetBOCallDetailsResponseDto>()
               .ForMember(dest => dest.Interlocutor, opt => opt.MapFrom(src => src.Direction == CallDirection.Outbound ? src.CalledId : src.CallerId));

            CreateMap<BOCallModel, BOCallDto>().ReverseMap();
            CreateMap<BOCallModel, BOCallDto>()
               .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString() : ""))
               .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => Utilities.GetCallDirection(src.Direction)))
               .ForMember(dest => dest.Result, opt => opt.MapFrom(src => Utilities.GetCallResult(src.Result)));

            CreateMap<PostBOCallsRequestDto, PostBOCallsRequestModel>().ReverseMap();
            CreateMap<PutAlternativeCommercialRequestDto, PutAlternativeCommercialRequestModel>().ReverseMap();
            CreateMap<PutCommercialRequestDto, PutCommercialRequestModel>().ReverseMap();

            // mapping Auth
            CreateMap<PostSigninRequestDto, PostSigninRequestModel>().ReverseMap();
            CreateMap<PostForgotPasswordRequestDto, PostForgotPasswordRequestModel>().ReverseMap();
            CreateMap<PostSigninResponseDto, PostSigninResponseModel>()
                .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken.ToString()));
            CreateMap<PostSigninResponseModel, PostSigninResponseDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId));

            // mapping BackOffice-Auth
            CreateMap<PostBOSigninRequestDto, PostBOSigninRequestModel>().ReverseMap();
            CreateMap<PostBOSigninResponseDto, PostBOSigninResponseModel>()
                .ForMember(dest => dest.accessToken, opt => opt.MapFrom(src => src.access_token.ToString()));
            CreateMap<PostBOSigninResponseModel, PostBOSigninResponseDto>()
                .ForMember(dest => dest.access_token, opt => opt.MapFrom(src => src.accessToken));

            //mapping to string CustomerType and mapping CustomerType from string
            CreateMap<CustomerModel, CustomerDto>().ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType.ToString()));
            CreateMap<CustomerDto, CustomerModel>().ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => Utilities.GetCustomerType(src.CustomerType)));

            CreateMap<CallsCustomersModel, CustomerDto>().ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType.ToString()));
            CreateMap<CustomerDto, CallsCustomersModel>().ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => Utilities.GetCustomerType(src.CustomerType)));

            CreateMap<CustomersPhonesModel, PhoneNumberDto>().ReverseMap();

            //mapping from CustomerModel to CustomerDto and CustomerDto to CustomerModel
            CreateMap<CustomerLiteModel, CustomerLiteDto>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerId))
                                                            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CustomerName));
            CreateMap<CustomerLiteDto, CustomerLiteModel>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
                                                            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name));

            //mapping to string PhoneType and mapping PhoneType from string
            CreateMap<PhoneNumberModel, PhoneNumberDto>().ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.PhoneType.ToString()));
            CreateMap<PhoneNumberDto, PhoneNumberModel>().ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => Utilities.GetPhoneType(src.PhoneType)));

            //mapping to string CallResult and mapping CallResult from string
            CreateMap<PutCallsRequestModel, PutCallsRequestDto>().ForMember(dest => dest.CallResult, opt => opt.MapFrom(src => src.CallResult.ToString()));
            CreateMap<PutCallsRequestDto, PutCallsRequestModel>().ForMember(dest => dest.CallResult, opt => opt.MapFrom(src => Utilities.GetCallResultEnum(src.CallResult)));

            //mapping to string CallDirection and mapping CallDirection from string
            CreateMap<SiebelRequestNewCallModel, PostCallsRequestDto>().ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()));
            CreateMap<PostCallsRequestDto, SiebelRequestNewCallModel>().ForMember(dest => dest.Direction, opt => opt.MapFrom(src => Utilities.GetCallDirectionEnum(src.Direction)));

            CreateMap<PostBOCommercialsRequestModel, PostBOCommercialRequestDto>();
            CreateMap<PostBOCommercialRequestDto, PostBOCommercialsRequestModel>();

            CreateMap<BOSupervisorModel, BOSupervisorDto>().ReverseMap();
            CreateMap<PostBOSupervisorRequestModel, PostBOSupervisorRequestDto>().ReverseMap();
            CreateMap<PostBOSupervisorResponseModel, StatusResponseDto>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));

            CreateMap<StatusResponseModel, StatusResponseDto>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));

            CreateMap<PatchBOSupervisorsRequestModel, PatchBOSupervisorsRequestDto>().ReverseMap();

            CreateMap<PostCallsSetCustomerRequestModel, PostCallsSetCustomerRequestDto>().ReverseMap();

            CreateMap<PostUpdatePasswordRequestModel, PostUpdatePasswordRequestDto>().ReverseMap();

            CreateMap<CommercialModel, FeatureDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CommercialId.ToString()))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommercialName));

            CreateMap<CustomerModel, CustomerFeatureDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerId))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CustomerName));

            CreateMap<PostUpdatePasswordResponseModel, PostUpdatePasswordResponse200Dto>().ReverseMap();
            CreateMap<CallsRecentsModel, GetCallsRecentsByCommercialDto>().ReverseMap();
            CreateMap<CallsRecentsModel, GetCallsRecentsByCommercialDto>()
               .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => Utilities.GetCallDirection(src.Direction)))
               .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime.ToString("MM/dd/yyyy HH:mm:ss")))
               .ForMember(dest => dest.CallPartResult, opt => opt.MapFrom(src => Utilities.GetCallResult(src.CallPartResult)));
        }
    }
}
