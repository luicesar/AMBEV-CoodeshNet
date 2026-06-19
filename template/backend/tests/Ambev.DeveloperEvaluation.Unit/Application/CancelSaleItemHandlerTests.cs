using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CancelSaleItemHandler>>();
        _handler = new CancelSaleItemHandler(_saleRepository, _logger);
    }

    [Fact(DisplayName = "Given valid sale and item When cancelling item Then item is cancelled")]
    public async Task Handle_ValidRequest_CancelsItem()
    {
        // Given
        var sale = SaleHandlerTestData.GenerateValidSale();
        var item = sale.Items.First();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = item.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.IsCancelled.Should().BeTrue();
        result.SaleId.Should().Be(sale.Id);
        result.ItemId.Should().Be(item.Id);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When cancelling item Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new CancelSaleItemCommand { SaleId = Guid.NewGuid(), ItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given non-existent item When cancelling item Then throws key not found exception")]
    public async Task Handle_ItemNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var sale = SaleHandlerTestData.GenerateValidSale();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = Guid.NewGuid() };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given cancelled sale When cancelling item Then throws invalid operation exception")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var cancelledSale = SaleHandlerTestData.GenerateCancelledSale();
        var command = new CancelSaleItemCommand { SaleId = cancelledSale.Id, ItemId = cancelledSale.Items.First().Id };

        _saleRepository.GetByIdAsync(cancelledSale.Id, Arg.Any<CancellationToken>()).Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Given empty ids When cancelling item Then throws validation exception")]
    public async Task Handle_EmptyIds_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleItemCommand { SaleId = Guid.Empty, ItemId = Guid.Empty };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
