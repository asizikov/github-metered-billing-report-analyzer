using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using Moq;
using Shouldly;

namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class MeteredBillingReportReaderTests
{
    [Fact]
    public async Task ReadFromSourceAsync_Respects_SeparatorSettings()
    {
        var mockCsvContentStreamer = new MockCsvContentStreamer(new []
        {
            "sep=%",
            "One%Two%Three",
            "1%2%3",
        });
        var reportItemParserMock = new Mock<IReportItemParser<MeteredBillingReportItem>>();
        reportItemParserMock.Setup(x => x.Parse(It.Is<string[]>(y => y.Length == 3))).Returns(new MeteredBillingReportItem());
        
        var reader = new MeteredBillingReportReader(mockCsvContentStreamer, reportItemParserMock.Object);
        var enumerator = reader.ReadFromSourceAsync("filename.csv").GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        enumerator.Current.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReadFromSourceAsync_FallbacksToDefaultSeparator()
    {
        var mockCsvContentStreamer = new MockCsvContentStreamer(new []
        {
            "One,Two,Three",
            "1,2,3",
        });
        
        var reportItemParserMock = new Mock<IReportItemParser<MeteredBillingReportItem>>();
        reportItemParserMock.Setup(x => x.Parse(It.Is<string[]>(y => y.Length == 3))).Returns(new MeteredBillingReportItem());
        
        var reader = new MeteredBillingReportReader(mockCsvContentStreamer, reportItemParserMock.Object);
        var enumerator = reader.ReadFromSourceAsync("filename.csv").GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        enumerator.Current.ShouldNotBeNull();
    }
}