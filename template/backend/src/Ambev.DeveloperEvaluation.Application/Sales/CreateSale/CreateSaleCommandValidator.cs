using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(s => s.SaleNumber).NotEmpty().MaximumLength(50);
        RuleFor(s => s.SaleDate).NotEqual(default(DateTime));
        RuleFor(s => s.CustomerId).NotEqual(Guid.Empty);
        RuleFor(s => s.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.BranchId).NotEqual(Guid.Empty);
        RuleFor(s => s.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Items).NotEmpty().WithMessage("Sale must have at least one item.");
        RuleForEach(s => s.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEqual(Guid.Empty);
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
