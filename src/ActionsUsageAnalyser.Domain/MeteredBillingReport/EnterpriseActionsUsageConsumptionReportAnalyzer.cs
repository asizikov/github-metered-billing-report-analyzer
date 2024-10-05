using ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Codespaces;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Copilot;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Packages;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;
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

    private readonly Dictionary<Product, (IReportEntryDataProcessor dataProcessor, IConsumptionReportSectionBuilder reportSectionBuilder)> dataProcessors = new()
    {
        { Product.Actions, (new ActionsEntryDataProcessor(), new ActionsConsumptionReportSectionBuilder()) },
        { Product.Copilot, (new CopilotEntryDataProcessor(), new CopilotConsumptionReportSectionBuilder()) },
        { Product.SharedStorage, (new SharedStorageEntryDataProcessor(), new SharedStorageConsumptionSectionBuilder()) },
        { Product.Packages, (new PackagesEntryDataProcessor(), new PackagesConsumptionReportSectionBuilder()) },
        { Product.CodespacesLinux, (new CodespacesEntryDataProcessor(), new CodespacesConsumptionReportSectionBuilder()) },
    };

    public async Task AnalyzeAsync(string dataFilePath)
    {
        try
        {
            var (start, end) = (DateTime.MaxValue, DateTime.MinValue);
            var pricePerSku = new Dictionary<string, (Product product, decimal multiplier, string unit, decimal price)>();
            var enterprise = new Enterprise();

            var seenProducts = new HashSet<Product>();
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (!dataProcessors.TryGetValue(reportItem.Product, out var dataProcessor)) continue;
                
                seenProducts.Add(reportItem.Product);
                start = reportItem.Date < start ? reportItem.Date : start;
                end = reportItem.Date > end ? reportItem.Date : end;

                dataProcessor.dataProcessor.ProcessForEnterprise(enterprise, reportItem);
                pricePerSku.TryAdd(reportItem.SKU, (reportItem.Product, reportItem.Multiplier, reportItem.UnitType, reportItem.PricePerUnit));
            }

            using var outputWriter = outputProvider.GetOutputWriter();

            outputWriter.WriteTitle(2, "Metered SKUs for this enterprise");

            outputWriter.WriteLine($"Metered data for period: **{start:yyyy-MM-dd}** to **{end:yyyy-MM-dd}**");

            outputWriter.BeginTable("Product", "SKU", "Unit", "Price per unit");
            foreach (var sku in pricePerSku
                         .OrderBy(kv => kv.Value.product.ToString())
                         .ThenBy(kv => kv.Value.price))
            {
                outputWriter.WriteTableRow(sku.Value.product.ToString(), sku.Key, sku.Value.unit, sku.Value.price.ToUsString());
            }

            outputWriter.EndTable();

            outputWriter.WriteLine($"Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count}");

            var summary = new List<(string product, decimal cost)>();
            foreach (var seenProduct in seenProducts)
            {
                if (!dataProcessors.TryGetValue(seenProduct, out var dataProcessor)) continue;
                var productCost = dataProcessor.reportSectionBuilder.Build(outputWriter, enterprise);
                summary.Add((seenProduct.ToString(), productCost));
            }

            
            outputWriter.WriteTitle(1, "Summary for this enterprise");

            outputWriter.BeginTable("Metered Cost", "Total price");
            foreach (var summaryItem in summary)
            {
                outputWriter.WriteTableRow(summaryItem.product, summaryItem.cost.ToUsString());
            }
            outputWriter.EndTable();

            outputWriter.WriteLine("Grand total: " + summary.Sum(x => x.cost).ToUsString());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process file {dataFilePath}");
        }
    }
}