using Microsoft.Extensions.Logging;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer(
    IReportReader<MeteredBillingReportItem> reportReader,
    IOutputProvider outputProvider,
    ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger)
    : IReportAnalyzer
{
    private readonly IReportReader<MeteredBillingReportItem> reportReader = reportReader ?? throw new ArgumentNullException(nameof(reportReader));
    private readonly IOutputProvider outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
    private readonly ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly Dictionary<Product, IReportEntryDataProcessor> dataProcessors = new()
    {
        { Product.Actions, new ActionsEntryDataProcessor() },
        { Product.Copilot, new CopilotEntryDataProcessor() },
        { Product.SharedStorage, new SharedStorageEntryDataProcessor() },
        { Product.Packages, new PackagesEntryDataProcessor() },
        { Product.CodespacesLinux, new CodespacesEntryDataProcessor() },
    };

    public async Task AnalyzeAsync(string dataFilePath)
    {
        try
        {
            var (start, end) = (DateTime.MaxValue, DateTime.MinValue);
            var pricePerSku = new Dictionary<string, (Product product, decimal multiplier, string unit, decimal price)>();
            var enterprise = new Enterprise();
            
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (!dataProcessors.TryGetValue(reportItem.Product, out var dataProcessor)) continue;
                start = reportItem.Date < start ? reportItem.Date : start;
                end = reportItem.Date > end ? reportItem.Date : end;
                
                dataProcessor.ProcessForEnterprise(enterprise, reportItem);
                pricePerSku.TryAdd(reportItem.SKU, (reportItem.Product, reportItem.Multiplier, reportItem.UnitType, reportItem.PricePerUnit));
            }

            using var outputWriter = outputProvider.GetOutputWriter();

            outputWriter.WriteTitle(2, "Metered SKUs for this enterprise");
            
            outputWriter.WriteLine($"Metered data for period: **{start:yyyy-MM-dd}** to **{end:yyyy-MM-dd}**");

            outputWriter.BeginTable("Product", "SKU","Unit", "Price per unit");
            foreach (var sku in pricePerSku
                         .OrderBy(kv => kv.Value.product.ToString())
                         .ThenBy(kv => kv.Value.price))
            {
                outputWriter.WriteTableRow(sku.Value.product.ToString(), sku.Key, sku.Value.unit, sku.Value.price.ToUsString());
            }
            outputWriter.EndTable();

            outputWriter.WriteLine($"Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count}");

            var actionsCost = BuildActionsConsumptionSection(outputWriter, enterprise, pricePerSku);
            var storageCost = BuildSharedStorageConsumptionSection(outputWriter, enterprise);
            var packagesCost = BuildPackagesConsumptionSection(outputWriter, enterprise);
            var copilotCost =  BuildCopilotConsumptionSection(outputWriter, enterprise);
            var codespacesCost = BuildCodespacesConsumptionSection(outputWriter, enterprise);
            
            outputWriter.WriteTitle(1, "Summary for this enterprise");
            
            outputWriter.BeginTable("Metered Cost", "Total price");
            outputWriter.WriteTableRow("Actions", actionsCost.ToUsString());
            outputWriter.WriteTableRow("Shared storage", storageCost.ToUsString());
            outputWriter.WriteTableRow("Packages", packagesCost.ToUsString());
            outputWriter.WriteTableRow("Copilot", copilotCost.ToUsString());
            outputWriter.WriteTableRow("Codespaces", codespacesCost.ToUsString());
            outputWriter.EndTable();
            
            outputWriter.WriteLine("Grand total: " + (actionsCost + packagesCost + storageCost + copilotCost).ToUsString());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process file {dataFilePath}");
        }
    }

    private decimal BuildCodespacesConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Codespaces consumption per organization");
            
        var totalCodespacesConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.CodespacesConsumptionPerOwner)
        {
            var totalCodespacesPriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);
            
            outputWriter.WriteTitle(4, "Consumption per SKU");
            outputWriter.BeginTable("SKU", "Unit", "Total price");
            foreach (var sku in owner.Value.ConsumptionPerSku)
            {
                outputWriter.WriteTableRow(sku.Key, sku.Value.unit, sku.Value.cost.ToUsString());
            }
            outputWriter.EndTable();
            
            outputWriter.WriteTitle(4, "Top 3 repositories by codespaces cost");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
            
            outputWriter.WriteLine($"Total codespaces cost for this organization: {totalCodespacesPriceForThisOwner.ToUsString()}");
            totalCodespacesConsumptionForEnterprise += totalCodespacesPriceForThisOwner;
        }
        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total codespaces consumption for the enterprise: {totalCodespacesConsumptionForEnterprise.ToUsString()}");
        return totalCodespacesConsumptionForEnterprise;
        
    }

    private static decimal BuildActionsConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise, Dictionary<string, (Product product, decimal multiplier, string unit, decimal price)> pricePerSku)
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
                outputWriter.WriteTableRow(sku.Key, sku.Value.ToString("N1"), priceForThisSku.ToUsString());
                totalPriceForThisOwner += priceForThisSku;
            }
            outputWriter.EndTable();

            outputWriter.WriteLine($"Total cost for this organization: {totalPriceForThisOwner.ToUsString()}");
            totalConsumptionForEnterprise += totalPriceForThisOwner;

            outputWriter.WriteTitle(4, "Top 3 repositories by consumption");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
        }

        outputWriter.WriteLine($"Total consumption for the enterprise: {totalConsumptionForEnterprise.ToUsString()}");
        return totalConsumptionForEnterprise;
    }

    private static decimal BuildSharedStorageConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Shared storage consumption per organization");
            
        var totalStorageConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.StorageConsumptionPerOwner)
        {
            var totalStoragePriceForThisOwner = owner.Value.AccumulatedPrice;
            outputWriter.WriteTitle(3, owner.Key);
            
            outputWriter.WriteTitle(4, "Top 3 repositories by storage cost");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
            
            outputWriter.WriteLine($"Total storage cost for this organization: {totalStoragePriceForThisOwner.ToUsString()}");
            totalStorageConsumptionForEnterprise += totalStoragePriceForThisOwner;
        }
        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total storage consumption for the enterprise: {totalStorageConsumptionForEnterprise.ToUsString()}");
        return totalStorageConsumptionForEnterprise;
    }

    private static decimal BuildCopilotConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Copilot consumption per organization");
            
        var totalCopilotConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.CopilotConsumptionPerOwner)
        {
            var totalCopilotPriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);
                
            outputWriter.WriteLine($"Total copilot cost for this organization: {totalCopilotPriceForThisOwner.ToUsString()}");
            totalCopilotConsumptionForEnterprise += totalCopilotPriceForThisOwner;
        }
        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total copilot consumption for the enterprise: {totalCopilotConsumptionForEnterprise.ToUsString()}");
        return totalCopilotConsumptionForEnterprise;
    }

    private static decimal BuildPackagesConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise)
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
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
                
            outputWriter.WriteLine($"Total packages cost for this organization: {totalPackagesPriceForThisOwner.ToUsString()}");
            totalPackagesConsumptionForEnterprise += totalPackagesPriceForThisOwner;
        }
        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total packages consumption for the enterprise: {totalPackagesConsumptionForEnterprise.ToUsString()}");
        return totalPackagesConsumptionForEnterprise;
    }
}