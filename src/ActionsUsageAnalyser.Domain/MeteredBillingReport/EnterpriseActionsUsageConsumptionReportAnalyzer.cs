using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer : IReportAnalyzer
{
    private readonly IReportReader<MeteredBillingReportItem> reportReader;
    private readonly IOutputProvider outputProvider;
    private readonly ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger;
    private readonly Configuration configuration;
    
    private readonly Dictionary<Product, IReportEntryDataProcessor> dataProcessors = new()
    {
        { Product.Actions, new ActionsEntryDataProcessor() },
        { Product.Copilot, new CopilotEntryDataProcessor() }
    };

    public EnterpriseActionsUsageConsumptionReportAnalyzer(IReportReader<MeteredBillingReportItem> reportReader, IOutputProvider outputProvider, ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger, Configuration configuration)
    {
        this.reportReader = reportReader ?? throw new ArgumentNullException(nameof(reportReader));
        this.outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task AnalyzeAsync(string dataFilePath)
    {
        try
        {
            var pricePerSku = new Dictionary<string, (decimal multiplier, decimal price)>();
            var enterprise = new Enterprise();
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (!dataProcessors.ContainsKey(reportItem.Product)) continue;
                dataProcessors[reportItem.Product].ProcessForEnterprise(enterprise, reportItem);
                
                if (configuration.ShouldAddCopilotDataToReport && reportItem.Product == Product.Copilot)
                {
                    continue;
                }
                pricePerSku.TryAdd(reportItem.SKU, (reportItem.Multiplier, reportItem.PricePerUnit));
            }

            using var outputWriter = outputProvider.GetOutputWriter();

            outputWriter.WriteTitle(2, "Actions SKUs for this enterprise");

            outputWriter.BeginTable("SKU", "Price per minute", "Multiplier");
            foreach (var sku in pricePerSku)
            {
                outputWriter.WriteTableRow(sku.Key, sku.Value.price.ToString("C", CultureInfo.GetCultureInfo("en-US")), sku.Value.multiplier.ToString("N1"));
            }
            outputWriter.EndTable();

            outputWriter.WriteLine($"Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count}");

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
                    var priceForThisSku = sku.Value * pricePerSku[sku.Key].price * pricePerSku[sku.Key].multiplier;
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
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process file {dataFilePath}");
        }
    }
}

public class Configuration
{
    public bool ShouldAddCopilotDataToReport = false;
}