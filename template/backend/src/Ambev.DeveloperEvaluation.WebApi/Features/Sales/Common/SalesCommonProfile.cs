using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

public class SalesCommonProfile : Profile
{
    public SalesCommonProfile()
    {
        CreateMap<SaleItemResult, SaleItemResponse>();
    }
}
