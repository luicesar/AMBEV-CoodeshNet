using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemCommandValidator : AbstractValidator<CancelSaleItemCommand>
{
    public CancelSaleItemCommandValidator()
    {
        RuleFor(s => s.SaleId).NotEqual(Guid.Empty);
        RuleFor(s => s.ItemId).NotEqual(Guid.Empty);
    }
}
