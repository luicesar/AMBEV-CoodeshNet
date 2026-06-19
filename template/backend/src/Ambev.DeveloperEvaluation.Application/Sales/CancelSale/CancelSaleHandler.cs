using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(ISaleRepository saleRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled.");

        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var domainEvent = new SaleCancelledEvent(sale.Id, sale.SaleNumber);
        _logger.LogInformation("Domain Event: {EventName} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleCancelledEvent), domainEvent.SaleId, domainEvent.SaleNumber);

        return new CancelSaleResult { Id = sale.Id, IsCancelled = sale.IsCancelled };
    }
}
