
using AutoMapper;
using TestProject.WebAPI.Models;
using TestProject.WebAPI.Data;
namespace TestProject.WebAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}
