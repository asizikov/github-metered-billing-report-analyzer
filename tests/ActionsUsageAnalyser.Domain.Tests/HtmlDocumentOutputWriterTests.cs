using ActionsUsageAnalyser.Domain;
using ActionsUsageAnalyzer.Infrastructure;
using Xunit;

namespace ActionsUsageAnalyser.Domain.Tests;

public class HtmlDocumentOutputWriterTests
{
    [Fact]
    public void WriteReportSummary_ShouldGenerateHtmlDocument()
    {
        // arrange
        var filePath = "test.html";
        var expectedHtml = File.ReadAllText("HtmlTemplate.html");
        var outputWriter = new HtmlDocumentOutputWriter(filePath);

        // act
        WriteReportSummary(outputWriter);
        outputWriter.Dispose();

        // assert
        var actualHtml = File.ReadAllText(filePath);
        Assert.Equal(expectedHtml, actualHtml);
    }

    private void WriteReportSummary(IOutputWriter outputWriter)
    {
        // write some sample report summary
        outputWriter.WriteTitle(1, "Report Summary");
        outputWriter.WriteLine("This is a sample report summary.");
        outputWriter.WriteTitle(2, "Actions SKUs for this enterprise");
        outputWriter.BeginTable("SKU", "Price per minute", "Multiplier");
        outputWriter.WriteTableRow("actions-minutes-1000", "$0.08", "1.0");
        outputWriter.WriteTableRow("actions-minutes-5000", "$0.08", "0.8");
        outputWriter.WriteTableRow("actions-minutes-10000", "$0.08", "0.7");
        outputWriter.EndTable();
        outputWriter.WriteLine("Total number of organizations: 3");
        outputWriter.WriteTitle(2, "Actions consumption per organization");
        outputWriter.WriteTitle(3, "owner-1");
        outputWriter.WriteTitle(4, "Consumption per SKU");
        outputWriter.BeginTable("SKU", "Minutes", "Total price");
        outputWriter.WriteTableRow("actions-minutes-1000", "500", "$40.00");
        outputWriter.WriteTableRow("actions-minutes-5000", "1000", "$64.00");
        outputWriter.EndTable();
        outputWriter.WriteLine("Total cost for this organization: $104.00");
        outputWriter.WriteTitle(4, "Top 3 repositories by consumption");
        outputWriter.BeginTable("Repository", "Total price");
        outputWriter.WriteTableRow("repo-1", "$50.00");
        outputWriter.WriteTableRow("repo-2", "$30.00");
        outputWriter.WriteTableRow("repo-3", "$24.00");
        outputWriter.EndTable();
        // write more report summary as needed
    }
}
