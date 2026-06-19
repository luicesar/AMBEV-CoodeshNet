using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class SaleHandlerTestData
{
    private static readonly Faker<CreateSaleItemCommand> itemCommandFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 9))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 1000));

    private static readonly Faker<CreateSaleCommand> createSaleFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, _ => itemCommandFaker.Generate(2));

    private static readonly Faker<UpdateSaleItemCommand> updateItemCommandFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 9))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 1000));

    private static readonly Faker<UpdateSaleCommand> updateSaleFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, _ => updateItemCommandFaker.Generate(2));

    public static CreateSaleCommand GenerateValidCreateCommand() => createSaleFaker.Generate();

    public static UpdateSaleCommand GenerateValidUpdateCommand() => updateSaleFaker.Generate();

    public static Sale GenerateValidSale()
    {
        var faker = new Faker();
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

        var item = new SaleItem
        {
            Id = faker.Random.Guid(),
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Int(1, 3),
            UnitPrice = faker.Finance.Amount(10, 100)
        };
        item.ApplyDiscount();
        item.CalculateTotal();
        sale.Items.Add(item);
        sale.RecalculateTotals();

        return sale;
    }

    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateValidSale();
        sale.Cancel();
        return sale;
    }
}
