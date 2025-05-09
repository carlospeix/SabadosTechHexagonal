﻿namespace Model;

public class Subject
{
    protected int? Id;

    public Grade Grade { get; private set; }
    public string Name { get; private set; }
    public Teacher Teacher { get; private set; }

    private Subject() {}

    public Subject(Grade grade, string name, Teacher teacher)
    {
        Grade = grade;
        Name = name;
        Teacher = teacher;
    }
}
