namespace Model;

public class Configuration
{
    protected int? id;

    private Configuration() { }

    public Configuration(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; init; }
    public string Value { get; private set; }

    public void ChangeValue(string newValue)
    {
        Value = newValue;
    }
}
