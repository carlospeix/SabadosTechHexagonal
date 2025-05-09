﻿using Model.Ports.Driving;

namespace WebApi.Endpoints;

public static class NotificationEndpointsExtensions
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");
        var notificationsGroup = apiV1.MapGroup("notifications");

        notificationsGroup.MapPost("general", CreateGeneralNotification)
            .Produces<NotificationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(nameof(CreateGeneralNotification))
            .WithOpenApi();

        notificationsGroup.MapPost("grade", CreateGradelNotification)
            .Produces<NotificationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(nameof(CreateGradelNotification))
            .WithOpenApi();

        notificationsGroup.MapPost("student", CreateStudentNotification)
            .Produces<NotificationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(nameof(CreateStudentNotification))
            .WithOpenApi();

        notificationsGroup.MapPost("disciplinary", CreateDisciplinaryNotification)
            .Produces<NotificationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(nameof(CreateDisciplinaryNotification))
            .WithOpenApi();
    }

    private static async Task<IResult> CreateGeneralNotification(NotificationRequest request, INotifications notifications)
    {
        return await CommonHandler(request, async () =>
        {
            await notifications.SendGeneral(Sanitize(request.Message), request.ScheduleAt);
            var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
            return Results.Created($"/api/comunications/general/{response.Id}", response);
        });
    }

    private static async Task<IResult> CreateGradelNotification(NotificationRequest request, INotifications notifications)
    {
        return await CommonHandler(request, async () =>
        {
            await notifications.SendGrade(request.GradeId, Sanitize(request.Message));
            var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
            return Results.Created($"/api/comunications/grade/{response.Id}", response);
        });
    }

    private static async Task<IResult> CreateStudentNotification(NotificationRequest request, INotifications notifications)
    {
        return await CommonHandler(request, async () =>
        {
            await notifications.SendStudent(request.StudentRecordId, Sanitize(request.Message));
            var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
            return Results.Created($"/api/comunications/student/{response.Id}", response);
        });
    }

    private static async Task<IResult> CreateDisciplinaryNotification(NotificationRequest request, INotifications notifications)
    {
        return await CommonHandler(request, async () =>
        {
            await notifications.SendDisciplinary(request.StudentRecordId, request.TeacherId, Sanitize(request.Message));
            var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
            return Results.Created($"/api/comunications/disciplinary/{response.Id}", response);
        });
    }

    // Sanitizes the parameter. Just a placeholder for now.
    private static T Sanitize<T>(T parameter)
    {
        return parameter;
    }

    private static async Task<IResult> CommonHandler(NotificationRequest request, Func<Task<IResult>> innerHandler)
    {
        try
        {
            if (request.Message == "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4")
            {
                throw new Exception();
            }

            return await innerHandler();
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(e.Message);
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }
    }
}

public record NotificationRequest(int GradeId, int StudentRecordId, int TeacherId, string Message, DateTime ScheduleAt);
public record NotificationResponse(Guid Id, string Message, int RecipientsAddressed);
