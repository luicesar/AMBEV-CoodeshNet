using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Given sale When cancelling Then sale is marked as cancelled")]
    public void Cancel_Sale_MarksAsCancelled()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();

        // When
        sale.Cancel();

        // Then
        sale.IsCancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given sale with item When cancelling item Then total is recalculated")]
    public void RecalculateTotals_AfterItemCancelled_TotalUpdated()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        var originalTotal = sale.TotalAmount;
        var item = sale.Items.First();

        // When
        item.Cancel();
        sale.RecalculateTotals();

        // Then
        sale.TotalAmount.Should().Be(0m); // only item was cancelled
        sale.TotalAmount.Should().BeLessThan(originalTotal);
    }

    [Fact(DisplayName = "Given sale When adding item Then total is updated")]
    public void AddItem_NewItem_TotalIncreases()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        var initialTotal = sale.TotalAmount;
        var newItem = SaleTestData.GenerateSaleItemWithQuantity(2);
        newItem.UnitPrice = 50m;

        // When
        sale.AddItem(newItem);

        // Then
        sale.TotalAmount.Should().BeGreaterThan(initialTotal);
        sale.Items.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given sale When validating valid sale Then validation passes")]
    public void Validate_ValidSale_ReturnsValid()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();

        // When
        var result = sale.Validate();

        // Then
        result.IsValid.Should().BeTrue();
    }
}
