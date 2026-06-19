using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Given quantity below 4 When applying discount Then discount is zero")]
    public void ApplyDiscount_QuantityBelowFour_NoDiscount()
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(3);

        // When
        item.ApplyDiscount();

        // Then
        item.Discount.Should().Be(0m);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When applying discount Then discount is 10%")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void ApplyDiscount_QuantityBetween4And9_TenPercentDiscount(int quantity)
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);

        // When
        item.ApplyDiscount();

        // Then
        item.Discount.Should().Be(0.10m);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When applying discount Then discount is 20%")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void ApplyDiscount_QuantityBetween10And20_TwentyPercentDiscount(int quantity)
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);

        // When
        item.ApplyDiscount();

        // Then
        item.Discount.Should().Be(0.20m);
    }

    [Fact(DisplayName = "Given quantity above 20 When applying discount Then throws domain exception")]
    public void ApplyDiscount_QuantityAbove20_ThrowsDomainException()
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(21);

        // When
        var act = () => item.ApplyDiscount();

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("*20*");
    }

    [Fact(DisplayName = "Given item with discount When calculating total Then total reflects discount")]
    public void CalculateTotal_WithDiscount_AppliesCorrectly()
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(5);
        item.UnitPrice = 100m;
        item.ApplyDiscount(); // 10% discount

        // When
        item.CalculateTotal();

        // Then
        item.TotalAmount.Should().Be(450m); // 5 * 100 * 0.90
    }

    [Fact(DisplayName = "Given item When cancelling Then item is marked as cancelled")]
    public void Cancel_Item_MarksAsCancelled()
    {
        // Given
        var item = SaleTestData.GenerateSaleItemWithQuantity(2);

        // When
        item.Cancel();

        // Then
        item.IsCancelled.Should().BeTrue();
    }
}
