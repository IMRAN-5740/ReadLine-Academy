using AutoMapper;
using ReadLineAcademy.Models.EntityModels;

namespace ReadLineAcademy.Web.AutoMapperProfile
{
    public class ReadLineAcademyProfile:Profile
    {
        public ReadLineAcademyProfile()
        {
            CreateMap<Product,Product>();
        }
    }
}
