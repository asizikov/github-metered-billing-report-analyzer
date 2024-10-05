using System.Globalization;

namespace ActionsUsageAnalyser.Domain;

public static class DecimalFormatExtensions
{
    public static string ToUsString(this decimal value) => 
        value.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
}