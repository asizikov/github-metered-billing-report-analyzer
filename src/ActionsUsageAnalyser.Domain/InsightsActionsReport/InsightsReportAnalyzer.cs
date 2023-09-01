using ActionsUsageAnalyzer.Domain;

namespace ActionsUsageAnalyser.Domain.InsightsActionsReport;

public class InsightsReportAnalyzer
{
    public async Task CompareReportsAsync(string dataFilePath, string insightsFilePath)
    {
        var actionsPerRepository = new Dictionary<string, List<ActionsReportItem>>();

        await foreach (var reportItem in BillingReportReader.ReadFromFileAsync(dataFilePath))
        {
            if (reportItem.Product != "Actions") continue;
            actionsPerRepository.TryAdd(reportItem.RepositorySlug, new List<ActionsReportItem>());
            actionsPerRepository[reportItem.RepositorySlug].Add(reportItem);
        }

        Console.WriteLine($"Total number of repositories: {actionsPerRepository.Count}");
       

        var repository = "xxx";
        
        Console.WriteLine($"Runs for '{repository}' repository: {actionsPerRepository[repository].Count}");
        
        var months = 4; //April

        var runs = actionsPerRepository[repository];
        var runsByMonth = runs.GroupBy(r => r.Date.Month);

        var aprilRuns = runsByMonth.First(g => g.Key == months).ToList();

        var workflows = new Dictionary<string, (double actual, double billed)>();
        foreach (var currentMonthReport in aprilRuns)
        {
            workflows.TryAdd(currentMonthReport.WorkflowName, (0, 0));
            workflows[currentMonthReport.WorkflowName] = (
                workflows[currentMonthReport.WorkflowName].actual + currentMonthReport.Quantity,
                workflows[currentMonthReport.WorkflowName].billed + currentMonthReport.Quantity * currentMonthReport.Multiplier
            );
        }


        Console.WriteLine("======================================");
        foreach (var workflow in workflows.OrderBy(x => x.Key))
        {
            var sanitizedWorkflowName = workflow.Key.ToLower().Replace(".github/workflows/", "");
            Console.WriteLine($"{sanitizedWorkflowName} - {workflow.Value.actual}");
        }

        Console.WriteLine($"Total in April billed {workflows.Sum(w => w.Value.billed)}");
        Console.WriteLine($"Total in April actual {workflows.Sum(w => w.Value.actual)}");
        Console.WriteLine("======================================");

        var internalActions = new Dictionary<string, List<InternalActionsReportItem>>();
        await foreach (var reportItem in InsightsReportReader.ReadFromFileAsync(insightsFilePath))
        {
            internalActions.TryAdd(reportItem.Repository, new List<InternalActionsReportItem>());
            internalActions[reportItem.Repository].Add(reportItem);
        }

        var internalActionsRuns = internalActions[repository];

        var runsByWorkflow = internalActionsRuns.GroupBy(r => r.Workflow);
        var runsByWorkflow2 = internalActionsRuns.GroupBy(r => r.Workflow).ToDictionary(x => x.Key, x => x.Sum(r => r.Minutes));
        foreach (var r in runsByWorkflow)
        {
            Console.WriteLine($"{r.Key} - {r.Sum(r => r.Minutes)}");
        }


        Console.WriteLine("======================================");
        Console.WriteLine("Workflow Name: Billing Report (minutes) vs Insights Report (minutes)");
        foreach (var workflow in workflows.OrderBy(x => x.Key))
        {
            var sanitizedWorkflowName = workflow.Key.ToLower().Replace(".github/workflows/", "");
            Console.WriteLine($"{sanitizedWorkflowName}: {workflow.Value.actual} vs {runsByWorkflow2[sanitizedWorkflowName]}");
        }

        Console.WriteLine("--------------------------------------");
    }
}