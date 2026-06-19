using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker faker = new();

    public static SaleItem GenerateSaleItemWithQuantity(int quantity)
    {
        return new SaleItem
        {
            Id = faker.Random.Guid(),
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = faker.Finance.Amount(10, 500)
        };
    }

    public static Sale GenerateValidSale()
    {
        var sale = new Sale
        {
            Id = faker.Random.Guid(),
            SaleNumber = $"SALE-{faker.Random.Number(1000, 9999)}",
            SaleDate = faker.Date.Recent(),
            CustomerId = faker.Random.Guid(),
            CustomerName = faker.Name.FullName(),
            BranchId = faker.Random.Guid(),
            BranchName = faker.Company.CompanyName()
        };

        var item = GenerateSaleItemWithQuantity(2);
        item.ApplyDiscount();
        item.CalculateTotal();
        sale.Items.Add(item);
        sale.RecalculateTotals();

        return sale;
    }
}
