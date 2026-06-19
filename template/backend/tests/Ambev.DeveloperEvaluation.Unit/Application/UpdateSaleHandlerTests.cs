using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid command When updating sale Then returns updated sale")]
    public async Task Handle_ValidCommand_ReturnsUpdatedSale()
    {
        // Given
        var existingSale = SaleHandlerTestData.GenerateValidSale();
        var command = SaleHandlerTestData.GenerateValidUpdateCommand();
        command.Id = existingSale.Id;
        var result = new UpdateSaleResult { Id = existingSale.Id };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateResult.Should().NotBeNull();
        updateResult.Id.Should().Be(existingSale.Id);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When updating sale Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand(); // empty — fails validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent sale id When updating Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = SaleHandlerTestData.GenerateValidUpdateCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given cancelled sale When updating Then throws invalid operation exception")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var cancelledSale = SaleHandlerTestData.GenerateCancelledSale();
        var command = SaleHandlerTestData.GenerateValidUpdateCommand();
        command.Id = cancelledSale.Id;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cancelled*");
    }
}
