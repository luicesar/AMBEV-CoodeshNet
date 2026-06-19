using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

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

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        var domainEvent = new SaleCreatedEvent(createdSale.Id, createdSale.SaleNumber, createdSale.SaleDate);
        _logger.LogInformation("Domain Event: {EventName} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleCreatedEvent), domainEvent.SaleId, domainEvent.SaleNumber);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}
