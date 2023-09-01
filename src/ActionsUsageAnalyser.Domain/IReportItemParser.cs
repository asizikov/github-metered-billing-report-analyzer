namespace ActionsUsageAnalyser.Domain;

public interface IReportItemParser<out T> where T : class
{
    T Parse(string[] values);
}