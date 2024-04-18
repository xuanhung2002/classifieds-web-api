using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Newtonsoft.Json;

namespace Common.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<AddPostDto, Post>()
               .ForMember(dest => dest.Images, opt => opt.Ignore()) // Không ánh xạ trực tiếp từ List<IFormFile> vào List<string>
               .ForMember(dest => dest.AddressJson, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Address))); // Ánh xạ AddressJson

                config.CreateMap<Post, AddPostDto>();
                config.CreateMap<Post, PostDto>();

                config.CreateMap<User, UserDto>();
            });


            return mappingConfig;
        }
    }
}
