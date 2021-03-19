using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dto => dto.FullAddress,
                    opt => opt.MapFrom(company => string.Join(' ', company.Address, company.Country))
                );

            CreateMap<Employee, EmployeeDto>();

            CreateMap<CompanyForCreationDto, Company>();

            CreateMap<EmployeeForCreationDto, Employee>();
        }
    }
}
