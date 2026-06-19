using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesValidator : AbstractValidator<GetSalesCommand>
{
    public GetSalesValidator()
    {
        RuleFor(s => s.Page).GreaterThan(0).WithMessage("Page must be greater than 0.");
        RuleFor(s => s.PageSize).GreaterThan(0).LessThanOrEqualTo(100).WithMessage("PageSize must be between 1 and 100.");
    }
}
