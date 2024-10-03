using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using ActionsUsageAnalyzer.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

var inputDirectory = Environment.GetEnvironmentVariable("INPUT_DIRECTORY") ?? @"/input/";
var outputDirectory = Environment.GetEnvironmentVariable("OUTPUT_DIRECTORY") ?? @"/output/";

if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage: dotnet run AppName --input <input file name> [--output <output file name>]");
    return;
}

if (!args.Contains("--input") && args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run -- --input <input file name> [<output file>]");
    return;
}

var inputIndex = Array.IndexOf<string>(args, "--input");
var dataFileName = args[inputIndex + 1];
var dataFilePath = Path.Combine(inputDirectory, dataFileName);

if (!File.Exists(dataFilePath))
{
    Console.WriteLine("Input file does not exist");
    return;
}

var outputFilePath = args.Length > inputIndex + 2 ? Path.Combine(outputDirectory, args[inputIndex + 2]) : null;

Console.WriteLine($"Report {dataFileName}");

var reportAnalyzer = new EnterpriseActionsUsageConsumptionReportAnalyzer(new MeteredBillingReportReader(new FileContentStreamer(), new MeteredBillingReportItemParser()), 
    new OutputProvider(outputFilePath),
    NullLogger<EnterpriseActionsUsageConsumptionReportAnalyzer>.Instance
    );

await reportAnalyzer.AnalyzeAsync(dataFilePath);

Console.WriteLine($"Output created {outputFilePath}");