using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using Shouldly;

namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class MeteredBillingReportItemParserTests
{
    [Fact]
    public void FromCsv_WhenCalledWithValidCsv_ReturnsMeteredBillingReportItem()
    {
        // Arrange
        var csvLine = "2023-08-01,Actions,Compute - UBUNTU,4719,minute,0.008,1.0,org-name,servicename,username,.github/workflows/api_monitoring.yml,";
        var expectedItem = new MeteredBillingReportItem
        {
            Date = new DateTime(2023, 8, 1),
            Product = Product.Actions,
            SKU = "Compute - UBUNTU",
            Quantity = 4719,
            UnitType = "minute",
            PricePerUnit = 0.008m,
            Multiplier = 1.0m,
            Owner = "org-name",
            RepositorySlug = "servicename",
            Username = "username",
            ActionsWorkflow = ".github/workflows/api_monitoring.yml",
            Notes = string.Empty
        };

        var parser = new MeteredBillingReportItemParser();
        // Act
        var actualItem = parser.Parse(csvLine.Split(","));

        // Assert
        actualItem.ShouldSatisfyAllConditions(
            () => actualItem.Date.ShouldBe(expectedItem.Date),
            () => actualItem.Product.ShouldBe(expectedItem.Product),
            () => actualItem.SKU.ShouldBe(expectedItem.SKU),
            () => actualItem.Quantity.ShouldBe(expectedItem.Quantity),
            () => actualItem.UnitType.ShouldBe(expectedItem.UnitType),
            () => actualItem.PricePerUnit.ShouldBe(expectedItem.PricePerUnit),
            () => actualItem.Multiplier.ShouldBe(expectedItem.Multiplier),
            () => actualItem.Owner.ShouldBe(expectedItem.Owner),
            () => actualItem.RepositorySlug.ShouldBe(expectedItem.RepositorySlug),
            () => actualItem.Username.ShouldBe(expectedItem.Username),
            () => actualItem.ActionsWorkflow.ShouldBe(expectedItem.ActionsWorkflow),
            () => actualItem.Notes.ShouldBe(expectedItem.Notes)
        );
    }

    [Fact]
    public void FromCsv_WhenCalledWith_CopilotProductConsumption_Returns_MeteredBillingReportItem()
    {
        var csvLine = "2023-08-02,Copilot,Copilot for Business,0.1613,user-month,19.0,1.0,org-name-three,,,,";
        var expectedItem = new MeteredBillingReportItem
        {
            Date = new DateTime(2023, 8, 2),
            Product = Product.Copilot,
            SKU = "Copilot for Business",
            Quantity = 0.1613m,
            UnitType = "user-month",
            PricePerUnit = 19.0m,
            Multiplier = 1.0m,
            Owner = "org-name-three",
            RepositorySlug = string.Empty,
            Username = string.Empty,
            ActionsWorkflow = string.Empty,
            Notes = string.Empty
        };
        
        var parser = new MeteredBillingReportItemParser();
        
        var actualItem = parser.Parse(csvLine.Split(","));
        
        actualItem.ShouldSatisfyAllConditions(
            () => actualItem.Date.ShouldBe(expectedItem.Date),
            () => actualItem.Product.ShouldBe(expectedItem.Product),
            () => actualItem.SKU.ShouldBe(expectedItem.SKU),
            () => actualItem.Quantity.ShouldBe(expectedItem.Quantity),
            () => actualItem.UnitType.ShouldBe(expectedItem.UnitType),
            () => actualItem.PricePerUnit.ShouldBe(expectedItem.PricePerUnit),
            () => actualItem.Multiplier.ShouldBe(expectedItem.Multiplier),
            () => actualItem.Owner.ShouldBe(expectedItem.Owner),
            () => actualItem.RepositorySlug.ShouldBe(expectedItem.RepositorySlug),
            () => actualItem.Username.ShouldBe(expectedItem.Username),
            () => actualItem.ActionsWorkflow.ShouldBe(expectedItem.ActionsWorkflow),
            () => actualItem.Notes.ShouldBe(expectedItem.Notes)
        );
    }

    [Fact]
    public void FromCsv_WhenCalledWith_StorageProductConsumption_Returns_MeteredBillingReportItem()
    {
        var csvLine = "2024-09-15,Shared Storage,Shared Storage,0.0,gb-day,0.008,1.0,enterprise-name,repo-name-two,,,";
        var expectedItem = new MeteredBillingReportItem
        {
            Date = new DateTime(2024, 09, 15),
            Product = Product.SharedStorage,
            SKU = "Shared Storage",
            Quantity = 0.0m,
            UnitType = "gb-day",
            PricePerUnit = 0.008m,
            Multiplier = 1.0m,
            Owner = "enterprise-name",
            RepositorySlug = "repo-name-two",
        };
        
        var parser = new MeteredBillingReportItemParser();
        
        var actualItem = parser.Parse(csvLine.Split(","));
        actualItem.ShouldSatisfyAllConditions(
            () => actualItem.Date.ShouldBe(expectedItem.Date),
            () => actualItem.Product.ShouldBe(expectedItem.Product),
            () => actualItem.SKU.ShouldBe(expectedItem.SKU),
            () => actualItem.Quantity.ShouldBe(expectedItem.Quantity),
            () => actualItem.UnitType.ShouldBe(expectedItem.UnitType),
            () => actualItem.PricePerUnit.ShouldBe(expectedItem.PricePerUnit),
            () => actualItem.Multiplier.ShouldBe(expectedItem.Multiplier),
            () => actualItem.Owner.ShouldBe(expectedItem.Owner),
            () => actualItem.RepositorySlug.ShouldBe(expectedItem.RepositorySlug),
            () => actualItem.Username.ShouldBe(expectedItem.Username),
            () => actualItem.ActionsWorkflow.ShouldBe(expectedItem.ActionsWorkflow),
            () => actualItem.Notes.ShouldBe(expectedItem.Notes)
        );
    }

    [Theory]
    [InlineData("Prebuild storage", "gb-month", "Create Codespaces Prebuilds")]
    [InlineData("Storage", "gb-month", "")]
    [InlineData("Compute - 2 core", "hour","" )]
    public void FromCsv_WhenCalledWith_CodespacesProductConsumption_Returns_MeteredBillingReportItem(string sku, string unit, string workflow)
    {
        var csvLine = $"2024-09-25,Codespaces - Linux,{sku},5.2512,{unit},0.07,1.0,enterprise-name,repo-name-two,,{workflow},";
        var expectedItem = new MeteredBillingReportItem
        {
            Date = new DateTime(2024, 09, 25),
            Product = Product.CodespacesLinux,
            SKU = sku,
            Quantity = 5.2512m,
            UnitType = unit,
            PricePerUnit = 0.07m,
            Multiplier = 1.0m,
            Owner = "enterprise-name",
            RepositorySlug = "repo-name-two",
            ActionsWorkflow = workflow
        };
        
        var parser = new MeteredBillingReportItemParser();
        
        var actualItem = parser.Parse(csvLine.Split(","));
        actualItem.ShouldSatisfyAllConditions(
            () => actualItem.Date.ShouldBe(expectedItem.Date),
            () => actualItem.Product.ShouldBe(expectedItem.Product),
            () => actualItem.SKU.ShouldBe(expectedItem.SKU),
            () => actualItem.Quantity.ShouldBe(expectedItem.Quantity),
            () => actualItem.UnitType.ShouldBe(expectedItem.UnitType),
            () => actualItem.PricePerUnit.ShouldBe(expectedItem.PricePerUnit),
            () => actualItem.Multiplier.ShouldBe(expectedItem.Multiplier),
            () => actualItem.Owner.ShouldBe(expectedItem.Owner),
            () => actualItem.RepositorySlug.ShouldBe(expectedItem.RepositorySlug),
            () => actualItem.Username.ShouldBe(expectedItem.Username),
            () => actualItem.ActionsWorkflow.ShouldBe(expectedItem.ActionsWorkflow),
            () => actualItem.Notes.ShouldBe(expectedItem.Notes)
        );
        
    }
    
    [Theory]
    [InlineData("Packages", Product.Packages)]
    [InlineData("Actions", Product.Actions)]
    [InlineData("Copilot", Product.Copilot)]
    [InlineData("Shared Storage", Product.SharedStorage)]
    [InlineData("Codespaces - Linux", Product.CodespacesLinux)]
    [InlineData("zzzz", Product.Unknown)]
    public void ProductType_Converted_As_Expected(string productType, Product expectedProduct)
    {
        var csvLine = $"2023-08-02,{productType},Copilot for Business,0.1613,user-month,19.0,1.0,org-name-three,,,,";
        var parser = new MeteredBillingReportItemParser();
        
        var actualItem = parser.Parse(csvLine.Split(","));
        
        actualItem.Product.ShouldBe(expectedProduct);
    }
}