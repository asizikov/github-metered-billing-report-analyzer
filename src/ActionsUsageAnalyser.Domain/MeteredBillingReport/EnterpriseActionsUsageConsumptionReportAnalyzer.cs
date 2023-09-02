using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer : IReportAnalyzer
{
    private readonly IReportReader<MeteredBillingReportItem> reportReader;
    private readonly IOutputProvider outputProvider;
    private readonly ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger;

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
            var pricePerSku = new Dictionary<string, (decimal multiplier, decimal price)>();
            var enterprise = new Enterprise();
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (reportItem.Product != "Actions") continue;
                enterprise.ActionsConsumptionPerOwner.TryAdd(reportItem.Owner, new ActionsConsumption());
                enterprise.ActionsConsumptionPerOwner[reportItem.Owner].MinutesPerSku.TryAdd(reportItem.SKU, 0);
                enterprise.ActionsConsumptionPerOwner[reportItem.Owner].MinutesPerSku[reportItem.SKU] += reportItem.Quantity;
                enterprise.ActionsConsumptionPerOwner[reportItem.Owner].PricePerRepository.TryAdd(reportItem.RepositorySlug, 0);
                enterprise.ActionsConsumptionPerOwner[reportItem.Owner].PricePerRepository[reportItem.RepositorySlug] += reportItem.Quantity * reportItem.Multiplier * reportItem.PricePerUnit;

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