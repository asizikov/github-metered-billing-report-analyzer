namespace ActionsUsageAnalyser.Domain.Meta;

[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute(DataType dataType) : Attribute
{
    public DataType DataType { get; } = dataType;
}