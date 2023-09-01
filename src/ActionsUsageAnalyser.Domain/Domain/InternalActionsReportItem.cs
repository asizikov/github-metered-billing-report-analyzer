namespace ActionsUsageAnalyzer.Domain;
//Workflow;Repository;Repo Visibility;Runtime;Runner Type;User Count;Runtime Minutes;#of Repo;;;;;;;;;;;;;;;
public class InternalActionsReportItem
{
    public string Workflow { get; set; }
    public string Repository { get; set; }
    public string RepoVisibility { get; set; }
    public string Runtime { get; set; }
    public string RunnerType { get; set; }
    public int UserCount { get; set; }
    public int Minutes { get; set; }
    public double FractionOfRepo { get; set; }
    
    public static InternalActionsReportItem? FromCsv(string[] values)
    {
        return new InternalActionsReportItem
        {
            Workflow = values[0],
            Repository = values[1],
            RepoVisibility = values[2],
            Runtime = values[3],
            RunnerType = values[4],
            UserCount = int.Parse(values[5]),
            Minutes = int.Parse(values[6]),
            FractionOfRepo = double.Parse(values[7])
        };
    }
}