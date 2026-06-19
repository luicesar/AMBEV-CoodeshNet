using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<Sale, SaleListItemResult>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));
    }
}
