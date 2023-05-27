using AutoMapper;
using Ifasanat.MeterMonitoringManagement.Domain;
using Ifasanat.MeterMonitoringManagement.Dto;

namespace Ifasanat.MeterMonitoringManagement.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
        }
    }
}
