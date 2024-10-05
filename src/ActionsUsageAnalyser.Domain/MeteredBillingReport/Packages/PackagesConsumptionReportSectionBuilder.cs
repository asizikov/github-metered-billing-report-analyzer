using ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Packages;

public class PackagesConsumptionReportSectionBuilder: IConsumptionReportSectionBuilder
{
    public decimal Build(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Packages consumption per organization");

        var totalPackagesConsumptionForEnterprise = 0M;

        foreach (var owner in enterprise.PackagesConsumptionPerOwner)
        {
            var totalPackagesPriceForThisOwner = owner.Value.AccumulatedCost;

            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteTitle(4, "Top 3 sources by packages cost");
            outputWriter.BeginTable("Source", "Total cost");
            foreach (var repository in owner.Value.CostPerRepository.OrderByDescending(x => x.Value).Take(3))
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