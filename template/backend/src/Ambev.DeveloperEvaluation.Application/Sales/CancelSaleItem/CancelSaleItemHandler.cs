using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(ISaleRepository saleRepository, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot cancel an item on a cancelled sale.");

        var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {request.ItemId} not found in sale {request.SaleId}");

        if (item.IsCancelled)
            throw new InvalidOperationException("Item is already cancelled.");

        item.Cancel();
        sale.RecalculateTotals();
        sale.UpdatedAt = DateTime.UtcNow;

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var domainEvent = new ItemCancelledEvent(sale.Id, item.Id, item.ProductName);
        _logger.LogInformation("Domain Event: {EventName} - SaleId: {SaleId}, ItemId: {ItemId}, Product: {ProductName}",
            nameof(ItemCancelledEvent), domainEvent.SaleId, domainEvent.ItemId, domainEvent.ProductName);

        return new CancelSaleItemResult
        {
            SaleId = sale.Id,
            ItemId = item.Id,
            IsCancelled = item.IsCancelled,
            NewSaleTotal = sale.TotalAmount
        };
    }
}
