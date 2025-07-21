using AutoMapper;
using Product.Backend.Application.Dto;

namespace Product.Backend.Application.MappingProfiles
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Domain.Product, ProductDto>().ReverseMap();
        }
    }
}
