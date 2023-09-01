namespace ActionsUsageAnalyser.Domain;

public interface IReportReader<out T> where T : class
{
    IAsyncEnumerable<T> ReadFromSourceAsync(string filename);
}