using AutoMapper;

namespace SmartChargingApi.Data;
public class SmartChargingMappingProfile : Profile
{
    public SmartChargingMappingProfile()
    {
        CreateMap<ConnectorModel, Connector>().ReverseMap();        
        CreateMap<ChargeStationModel, ChargeStation>().
            ForMember(dest => dest.Connectors, opt => opt.MapFrom(src => src.Connectors)).ReverseMap();
        CreateMap<GroupModel, Group>().ReverseMap();       
    }   
}

