using ActionsUsageAnalyzer.Domain;
using Shouldly;

namespace ActionsUsageAnalyser.Domain.Tests
{
    public class MeteredBillingReportItemTests
    {
        [Fact]
        public void FromCsv_WhenCalledWithValidCsv_ReturnsMeteredBillingReportItem()
        {
            // Arrange
            var csvLine = "2023-08-01,Actions,Compute - UBUNTU,4719,minute,0.008,1.0,org-name,servicename,username,.github/workflows/api_monitoring.yml,";
            var expectedItem = new MeteredBillingReportItem
            {
                Date = new DateTime(2023, 8, 1),
                Product = "Actions",
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

            // Act
            var actualItem = MeteredBillingReportItem.FromCsv(csvLine.Split(","));

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
    }
}