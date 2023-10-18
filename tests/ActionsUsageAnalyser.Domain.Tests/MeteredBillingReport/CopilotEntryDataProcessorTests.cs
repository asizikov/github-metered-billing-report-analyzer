using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using Shouldly;

namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class CopilotEntryDataProcessorTests
{
    private readonly CopilotEntryDataProcessor dataProcessor = new();
    
    [Fact]
    public void ProcessForEnterprise_WhenItemIsNotCopilotProduct_ShouldNotAddToEnterprise()
    {
        var enterprise = new Enterprise();
        var item = new MeteredBillingReportItem
        {
            Product = Product.Actions
        };
        
        dataProcessor.ProcessForEnterprise(enterprise, item);
        

        enterprise.CopilotConsumptionPerOwner.ShouldBeEmpty();
    }
    
    [Fact]
    public void ProcessForEnterprise_WhenItemIsCopilotProduct_ShouldAddToEnterprise()
    {
        var enterprise = new Enterprise();
        var item = new MeteredBillingReportItem
        {
            Product = Product.Copilot,
            Owner = "owner",
            SKU = "Copilot for Business",
            Quantity = 1,
            Multiplier = 1,
            PricePerUnit = 19.0m
        };
        
        dataProcessor.ProcessForEnterprise(enterprise, item);
        
        enterprise.ShouldSatisfyAllConditions(
            () => enterprise.CopilotConsumptionPerOwner.ShouldContainKey(item.Owner),
            () => enterprise.CopilotConsumptionPerOwner[item.Owner].AccumulatedPrise.ShouldBe(item.Quantity * item.Multiplier * item.PricePerUnit)
        );
    }

    [Fact]
    public void ProcessForEnterprise_WhenCalledForMultipleEntries_Should_AggregateInformation()
    {
        var enterprise = new Enterprise();

        var items = new List<MeteredBillingReportItem>
        {
            new()
            {
                Product = Product.Copilot,
                Owner = "owner",
                SKU = "Copilot for Business",
                Quantity = 1,
                Multiplier = 1,
                PricePerUnit = 19.0m
            },
            new()
            {
                Product = Product.Copilot,
                Owner = "owner",
                SKU = "Copilot for Business",
                Quantity = 2.1m,
                Multiplier = 1,
                PricePerUnit = 19.0m
            },
            new()
            {
                Product = Product.Copilot,
                Owner = "owner",
                SKU = "Copilot for Business",
                Quantity = 0.23m,
                Multiplier = 2,
                PricePerUnit = 19.0m
            }
        };
        
        foreach (var item in items)
        {
            dataProcessor.ProcessForEnterprise(enterprise, item);
        }
        
        enterprise.ShouldSatisfyAllConditions(
            () => enterprise.CopilotConsumptionPerOwner.ShouldContainKey(items[0].Owner),
            () => enterprise.CopilotConsumptionPerOwner[items[0].Owner].AccumulatedPrise.ShouldBe(items.Sum(i => i.Quantity * i.PricePerUnit))
        );
    }
}