namespace ActionsUsageAnalyser.Domain.Meta;

[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute : Attribute
{
    public SensitiveDataAttribute(DataType dataType)
    {
        DataType = dataType;
    }

    public DataType DataType { get; }
}