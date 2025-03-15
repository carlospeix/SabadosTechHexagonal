namespace Model;

public class Grade
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    private Grade() {}

    public Grade(string name)
    {
        Name = name;
    }
}
