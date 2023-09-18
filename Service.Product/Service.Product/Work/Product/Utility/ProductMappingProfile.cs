using AutoMapper;
using Service.Product.Shared.Database.Entity;
using Service.Product.Work.Model;

namespace Service.Product.Work.Utility;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Database.Product -> Model.Retrieve.ProductList
        CreateMap<dbProduct, ProductListRetrieveModel>()
            .ForMember(dest => dest.CategoryName, opts => opts.MapFrom(src => src.Category.CategoryName));

        // Database.Product -> Model.Retrieve.ProductDetail
        CreateMap<dbProduct, ProductDetailRetrieveModel>()
            .ForMember(dest => dest.CategoryName, opts => opts.MapFrom(src => src.Category.CategoryName));

        // Model.Update.Product.Detail -> Database.Product
        CreateMap<ProductDetailUpdateModel, dbProduct>();

        // Model.Create.Product.Detail -> Database.Product
        CreateMap<ProductDetailCreateModel, dbProduct>();
    }

}