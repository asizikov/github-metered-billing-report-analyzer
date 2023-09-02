namespace ActionsUsageAnalyser.Domain;

public interface IOutputProvider
{
    IOutputWriter GetOutputWriter();
}