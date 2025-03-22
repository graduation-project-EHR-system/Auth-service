using AutoMapper;
using Data.Layer.Dtos;
using Data.Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.MaritalStatus))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ReverseMap();
        }
    }
}
