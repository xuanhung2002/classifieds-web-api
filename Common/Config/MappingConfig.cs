using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.BidDtos;
using Classifieds.Data.DTOs.WatchListDtos;
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

                // post
                config.CreateMap<PostAddDto, Post>()
               .ForMember(dest => dest.Images, opt => opt.Ignore()) // Không ánh xạ trực tiếp từ List<IFormFile> vào List<string>
               .ForMember(dest => dest.AddressJson, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Address))); // Ánh xạ AddressJson
                config.CreateMap<Post, PostAddDto>();
                config.CreateMap<Post, PostDto>();
                config.CreateMap<PostUpdateDto, Post>().ReverseMap();

                // user
                config.CreateMap<User, UserDto>();

                // category
                config.CreateMap<Category, CategoryDto>().ReverseMap();

                //watch list
                config.CreateMap<AddWatchPostDto, WatchList>();


                // bid
                config.CreateMap<Bid, BidDto>()
                    .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                    .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDto
                    {
                        // Map properties from User entity to UserDto
                        Name = src.User.Name,
                        PhoneNumber = src.User.PhoneNumber,
                        Email = src.User.Email,
                        AccountName = src.User.AccountName,
                        Avatar = src.User.Avatar,
                        Role = src.User.Role
                    }));

            });


            return mappingConfig;
        }
    }
}
