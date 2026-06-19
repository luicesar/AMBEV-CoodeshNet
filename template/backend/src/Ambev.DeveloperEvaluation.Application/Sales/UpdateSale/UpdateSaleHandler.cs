using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        sale.SaleDate = command.SaleDate;
        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;
        sale.UpdatedAt = DateTime.UtcNow;

        sale.Items.Clear();
        foreach (var itemCmd in command.Items)
        {
            var item = new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = itemCmd.ProductId,
                ProductName = itemCmd.ProductName,
                Quantity = itemCmd.Quantity,
                UnitPrice = itemCmd.UnitPrice
            };
            sale.AddItem(item);
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var domainEvent = new SaleModifiedEvent(updatedSale.Id, updatedSale.SaleNumber);
        _logger.LogInformation("Domain Event: {EventName} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleModifiedEvent), domainEvent.SaleId, domainEvent.SaleNumber);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
