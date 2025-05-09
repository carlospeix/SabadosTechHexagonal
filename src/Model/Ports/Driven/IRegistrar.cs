﻿using System.Linq.Expressions;

namespace Model.Ports.Driven;

public interface IRegistrar
{
    public Task<Teacher?> TeacherById(int teacherId);

    public IAsyncEnumerable<Grade> AllGrades { get; }
    public Task<Grade?> GradeById(int gradeId);

    public IAsyncEnumerable<Parent> AllParents { get; }

    public Task<StudentRecord?> StudentRecordById(int studentRecordId);

    public Task<Configuration?> ConfigurationByName(string name);
    public Task<Configuration> RequiredConfigurationByName(string name, string errorMessageIfNotConfigured);

    public IAsyncEnumerable<Notification> FilteredNotifications(Expression<Func<Notification, bool>> filter);
    public void AddNotification(Notification notification);
}
