using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //user
            this.CreateMap<UserDto, User>();

            //product
            this.CreateMap<ProductDto, Product>();
            this.CreateMap<Product, ExportProductsInRange>()
               .ForMember(d => d.SellerFullName, mo => mo.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            //category
            this.CreateMap<CategoryDto, Category>();

            //categoryProduct
            this.CreateMap<CategoryProductDto, CategoryProduct>();
        }
    }
}
