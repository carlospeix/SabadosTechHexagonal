using Model;
using Model.Ports.Driven;
using System.Linq.Expressions;

namespace Persistence;

public class Registrar(ApplicationContext applicationContext) : IRegistrar
{
    private readonly ApplicationContext applicationContext = applicationContext;

    public async Task<Teacher?> TeacherById(int teacherId) =>
        await applicationContext.Teachers.FirstOrDefaultAsync(s => s.Id == teacherId);

    public IAsyncEnumerable<Grade> AllGrades =>
        applicationContext.Grades.Include(g => g.StudentRecords)
                                 .Include(g => g.Subjects)
                                 .ThenInclude(s => s.Teacher)
                                 .AsAsyncEnumerable();
    public async Task<Grade?> GradeById(int gradeId) =>
        await applicationContext.Grades.Include(g => g.StudentRecords).ThenInclude(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
                                       .Include(g => g.Subjects).ThenInclude(s => s.Teacher)
                                       .FirstOrDefaultAsync(g => g.Id == gradeId);

    public IAsyncEnumerable<Parent> AllParents =>
        applicationContext.Parents.AsAsyncEnumerable();

    public async Task<StudentRecord?> StudentRecordById(int studentRecordId) =>
        await applicationContext.StudentRecords.Include(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
                                               .FirstOrDefaultAsync(s => s.Id == studentRecordId);

    public async Task<Configuration?> ConfigurationByName(string name) =>
        await applicationContext.Configurations.FirstOrDefaultAsync(c => c.Name == name);

    public async Task<Configuration> RequiredConfigurationByName(string name, string errorMessageIfNotConfigured)
    {
        var config = await applicationContext.Configurations.FirstOrDefaultAsync(c => c.Name == name);

        if (config is null || string.IsNullOrWhiteSpace(config.Value))
            throw new InvalidOperationException(errorMessageIfNotConfigured);
        
        return config;
    }

    public IAsyncEnumerable<Notification> FilteredNotifications(Expression<Func<Notification, bool>> filter) =>
        applicationContext.Notifications.Include(n => n.Recipients)
                                        .Where(filter).AsAsyncEnumerable();

    public void AddNotification(Notification notification)
    {
        applicationContext.Notifications.Add(notification);
    }
}
