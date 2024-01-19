using ActionsUsageAnalyser.Domain;
using System.IO;
using System.Text;

namespace ActionsUsageAnalyzer.Infrastructure;

public class HtmlDocumentOutputWriter : IOutputWriter
{
    private readonly TextWriter writer;
    private readonly StringBuilder tableBuilder;

    public HtmlDocumentOutputWriter(string filePath)
    {
        writer = File.CreateText(filePath);
        tableBuilder = new StringBuilder();
    }

    public void WriteLine(string line)
    {
        writer.WriteLine($"<p>{line}</p>");
    }

    public void WriteTitle(int level, string line)
    {
        writer.WriteLine($"<h{level}>{line}</h{level}>");
    }

    public void BeginTable(params string[] headers)
    {
        tableBuilder.Clear();
        tableBuilder.AppendLine("<table>");
        tableBuilder.AppendLine("<thead>");
        tableBuilder.AppendLine("<tr>");
        foreach (var header in headers)
        {
            tableBuilder.AppendLine($"<th>{header}</th>");
        }
        tableBuilder.AppendLine("</tr>");
        tableBuilder.AppendLine("</thead>");
        tableBuilder.AppendLine("<tbody>");
    }

    public void WriteTableRow(params string[] columns)
    {
        tableBuilder.AppendLine("<tr>");
        foreach (var column in columns)
        {
            tableBuilder.AppendLine($"<td>{column}</td>");
        }
        tableBuilder.AppendLine("</tr>");
    }

    public void EndTable()
    {
        tableBuilder.AppendLine("</tbody>");
        tableBuilder.AppendLine("</table>");
        writer.WriteLine(tableBuilder.ToString());
    }

    public void Dispose()
    {
        // write the HTML template with the report summary as the content
        var template = File.ReadAllText("HtmlTemplate.html");
        var content = writer.ToString();
        var html = template.Replace("{{content}}", content);
        writer.Write(html);
        writer.Close();
        writer.Dispose();
    }
}
