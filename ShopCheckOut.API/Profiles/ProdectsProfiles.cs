using AutoMapper;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Profiles
{
    public class ProdectsProfiles : Profile
    {
        public ProdectsProfiles()
        {
            CreateMap<ProductsModel, ProductReadDto>()
                .ForMember(dest => dest.PriceInfo,
                opt => opt.MapFrom(src => $"{src.Price} per {src.PriceUnit}"));

            CreateMap<ProductCreateDto, ProductsModel>();
        }
    }
}
