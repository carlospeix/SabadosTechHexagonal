﻿namespace Model;

public class Configuration : TenantEntity
{
    public const string DISCIPLINARY_INBOX = "DISCIPLINARY_INBOX";

    private Configuration() { }

    public Configuration(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public int Id { get; private set; }

    public string Name { get; init; }
    public string Value { get; private set; }

    public void ChangeValue(string newValue)
    {
        Value = newValue;
    }
}
