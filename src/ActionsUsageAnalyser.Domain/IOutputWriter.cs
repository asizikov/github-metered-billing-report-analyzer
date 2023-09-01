namespace ActionsUsageAnalyser.Domain;

public interface IOutputWriter: IDisposable
{
    void WriteLine(string line);
    void WriteTitle(int level, string line);
    void BeginTable(params string [] headers);
    void WriteTableRow(params string[] columns);
    void EndTable();
}