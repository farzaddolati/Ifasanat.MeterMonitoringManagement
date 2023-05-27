using AutoMapper;
using Ifasanat.MeterMonitoringManagement.Domain;
using Ifasanat.MeterMonitoringManagement.Dto;

namespace Ifasanat.MeterMonitoringManagement.Mapping
{
    public class CityMappingProfile : Profile
    {
        public CityMappingProfile()
        {
            CreateMap<City, CityDto>().ReverseMap();
        }
    }
}
