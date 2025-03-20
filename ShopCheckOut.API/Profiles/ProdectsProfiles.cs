using AutoMapper;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Profiles
{
    public class ProdectsProfiles : Profile
    {
        public ProdectsProfiles()
        {
            CreateMap<Products, ProductReadDto>();
            CreateMap<ProductCreateDto, Products>();
        }
    }
}
