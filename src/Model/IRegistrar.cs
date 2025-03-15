namespace Model;

public interface IRegistrar
{
    public IQueryable<Teacher> Teachers { get; }
    public IQueryable<Grade> Grades { get; }
}
