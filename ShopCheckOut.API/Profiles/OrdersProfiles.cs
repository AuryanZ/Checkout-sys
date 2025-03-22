using AutoMapper;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Profiles
{
    public class OrdersProfiles : Profile
    {
        public OrdersProfiles()
        {
            CreateMap<OrdersModel, OrderCreateDto>();

            CreateMap<OrderItems, ItemUpdateDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            CreateMap<OrderItems, ItemUpdateDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            CreateMap<OrdersModel, OrderUpdateDto>()
                .ForMember(dest => dest.OrderItems,
                           opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src => src.TotalAmount));

            CreateMap<OrdersModel, OrderCheckoutDto>()
                .ForMember(dest => dest.OrderItems,
                           opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src => src.TotalAmount));
        }
    }
}