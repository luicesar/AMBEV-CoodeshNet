using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

public class GetSalesRequestValidator : AbstractValidator<GetSalesRequest>
{
    public GetSalesRequestValidator()
    {
        RuleFor(s => s.Page).GreaterThan(0);
        RuleFor(s => s.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
