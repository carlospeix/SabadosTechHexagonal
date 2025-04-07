

namespace Model;

public class Student
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public Grade? Grade { get; private set; }

    private Student() {}

    public Student(string name)
    {
        Name = name;
    }
}
