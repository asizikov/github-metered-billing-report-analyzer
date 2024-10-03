using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer : IReportAnalyzer
{
    private readonly IReportReader<MeteredBillingReportItem> reportReader;
    private readonly IOutputProvider outputProvider;
    private readonly ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger;
    
    private readonly Dictionary<Product, IReportEntryDataProcessor> dataProcessors = new()
    {
        { Product.Actions, new ActionsEntryDataProcessor() },
        { Product.Copilot, new CopilotEntryDataProcessor() },
        { Product.SharedStorage, new SharedStorageEntryDataProcessor()},
        { Product.Packages, new PackagesEntryDataProcessor()}
    };

    public EnterpriseActionsUsageConsumptionReportAnalyzer(IReportReader<MeteredBillingReportItem> reportReader, IOutputProvider outputProvider, ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger)
    {
        this.reportReader = reportReader ?? throw new ArgumentNullException(nameof(reportReader));
        this.outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AnalyzeAsync(string dataFilePath)
    {
        try
        {
            var pricePerSku = new Dictionary<string, (decimal multiplier, string unit, decimal price)>();
            var enterprise = new Enterprise();
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (!dataProcessors.TryGetValue(reportItem.Product, out var dataProcessor)) continue;
                dataProcessor.ProcessForEnterprise(enterprise, reportItem);
                pricePerSku.TryAdd(reportItem.SKU, (reportItem.Multiplier, reportItem.UnitType, reportItem.PricePerUnit));
            }

            using var outputWriter = outputProvider.GetOutputWriter();

            outputWriter.WriteTitle(2, "Metered SKUs for this enterprise");

            outputWriter.BeginTable("SKU","Unit", "Price per unit", "Multiplier");
            foreach (var sku in pricePerSku.OrderBy(kv => kv.Key))
            {
                outputWriter.WriteTableRow(sku.Key, sku.Value.unit, sku.Value.price.ToString("C", CultureInfo.GetCultureInfo("en-US")), sku.Value.multiplier.ToString("N1"));
            }
            outputWriter.EndTable();

            outputWriter.WriteLine($"Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count}");

            BuildActionsConsumptionSection(outputWriter, enterprise, pricePerSku);
            BuildSharedStorageConsumptionSecction(outputWriter, enterprise);
            BuildCopilotConsumptionSecction(outputWriter, enterprise);
            BuildPackagesConsumptionSecction(outputWriter, enterprise);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process file {dataFilePath}");
        }
    }

    private static void BuildActionsConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise, Dictionary<string, (decimal multiplier, string unit, decimal price)> pricePerSku)
    {
        outputWriter.WriteTitle(2, "Actions consumption per organization");

        var totalConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.ActionsConsumptionPerOwner)
        {
            var totalPriceForThisOwner = 0M;
            outputWriter.WriteTitle(3, owner.Key);
            outputWriter.WriteTitle(4, "Consumption per SKU");

            outputWriter.BeginTable("SKU", "Minutes", "Total price");
            foreach (var sku in owner.Value.MinutesPerSku)
            {
                var priceForThisSku = sku.Value * pricePerSku[sku.Key].price;
                outputWriter.WriteTableRow(sku.Key, sku.Value.ToString("N1"), priceForThisSku.ToString("C", CultureInfo.GetCultureInfo("en-US")));
                totalPriceForThisOwner += priceForThisSku;
            }
            outputWriter.EndTable();


            outputWriter.WriteLine($"Total cost for this organization: {totalPriceForThisOwner.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            totalConsumptionForEnterprise += totalPriceForThisOwner;

            outputWriter.WriteTitle(4, "Top 3 repositories by consumption");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToString("C", CultureInfo.GetCultureInfo("en-US")));
            }
            outputWriter.EndTable();
        }

        outputWriter.WriteLine($"Total consumption for the enterprise: {totalConsumptionForEnterprise.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
    }

    private static void BuildSharedStorageConsumptionSecction(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Shared storage consumption per organization");
            
        var totalStorageConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.StorageConsumptionPerOwner)
        {
            var totalStoragePriceForThisOwner = owner.Value.AccumulatedPrise;
            outputWriter.WriteTitle(3, owner.Key);
                
            outputWriter.WriteLine($"Total storage cost for this organization: {totalStoragePriceForThisOwner.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            totalStorageConsumptionForEnterprise += totalStoragePriceForThisOwner;
        }
        outputWriter.WriteLine($"Total storage consumption for the enterprise: {totalStorageConsumptionForEnterprise.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
    }

    private static void BuildCopilotConsumptionSecction(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Copilot consumption per organization");
            
        var totalCopilotConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.CopilotConsumptionPerOwner)
        {
            var totalCopilotPriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);
                
            outputWriter.WriteLine($"Total copilot cost for this organization: {totalCopilotPriceForThisOwner.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            totalCopilotConsumptionForEnterprise += totalCopilotPriceForThisOwner;
        }
        outputWriter.WriteLine($"Total copilot consumption for the enterprise: {totalCopilotConsumptionForEnterprise.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
    }

    private static void BuildPackagesConsumptionSecction(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Packages consumption per organization");
            
        var totalPackagesConsumptionForEnterprise = 0M;

        foreach (var owner in enterprise.PackagesConsumptionPerOwner)
        {
            var totalPackagesPriceForThisOwner = owner.Value.AccumulatedCost;
                
            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteTitle(4, "Top 3 sources by packages cost");
            outputWriter.BeginTable("Source", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToString("C", CultureInfo.GetCultureInfo("en-US")));
            }
            outputWriter.EndTable();
                
            outputWriter.WriteLine($"Total packages cost for this organization: {totalPackagesPriceForThisOwner.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            totalPackagesConsumptionForEnterprise += totalPackagesPriceForThisOwner;
        }
        outputWriter.WriteLine($"Total packages consumption for the enterprise: {totalPackagesConsumptionForEnterprise.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
    }
}