namespace WareLogix;


[AttributeUsage(
    AttributeTargets.Class 
    | AttributeTargets.Struct
    | AttributeTargets.Method)
]
public class WorkInProgressAttribute : Attribute
{
    private string Name;
    public double Version;


    public WorkInProgressAttribute(string name)
    {
        Name = name;
        Version = 1.0;
    }
}