using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure;

public class ConsoleOutputWriter : IOutputWriter
{
    private List<string[]> tableRows = new();

    public void WriteLine(string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
    }

    public void WriteTitle(int level, string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
        var ch = level switch
        {
            2 => '=',
            _ => '-'
        };
        Console.WriteLine(new string(ch, line.Length));
    }

    public void BeginTable(params string[] headers)
    {
        Console.WriteLine();
        tableRows = new() { headers };
    }

    public void WriteTableRow(params string[] columns)
    {
        tableRows.Add(columns);
    }

    public void EndTable()
    {
        // print table
        var columnWidths = tableRows.First().Select(_ => _.Length).ToArray();
        foreach (var row in tableRows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
            }
        }

        // print header
        Console.WriteLine(string.Join(" | ", tableRows.First().Select((_, i) => _.PadRight(columnWidths[i]))));
        Console.WriteLine(string.Join(" | ", columnWidths.Select(_ => new string('-', _))));
        // print rows
        foreach (var row in tableRows.Skip(1))
        {
            Console.WriteLine(string.Join(" | ", row.Select((_, i) => _.PadRight(columnWidths[i]))));
        }

        Console.WriteLine();
    }

    public void Dispose()
    {
    }
}