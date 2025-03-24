using AutoMapper;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Profiles
{
    public class DiscountsProfiles : Profile
    {
        public DiscountsProfiles()
        {
            CreateMap<AddDiscountRequest, DiscountsModel>()
                .ForMember(dest => dest.minQuantity, opt => opt.MapFrom(src => src.minQuantity))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .IncludeAllDerived();

            CreateMap<AddDiscountRequest, PercentageDiscount>()
                    .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage));

            CreateMap<AddDiscountRequest, FixedPriceDiscount>()
                .ForMember(dest => dest.FixedPrice, opt => opt.MapFrom(src => src.FixedPrice));

            CreateMap<AddDiscountRequest, BuyXGetYDiscount>()
                .ForMember(dest => dest.FreeItem, opt => opt.MapFrom(src => src.FreeItem));

        }
    }
}
