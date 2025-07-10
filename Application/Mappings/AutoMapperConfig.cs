using Application.DTO;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<RegisterUserDTO, User>();
            })
            .CreateMapper();
        }
    }
}
