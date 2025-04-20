using Model.Ports.Driving;

namespace WebApi.Endpoints;

public static class NotificationEndpointsExtensions
{
    public static void MapNotificationsEndPoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");

        apiV1.MapPost("/notifications/general", (NotificationRequest request , INotifications notifications) =>
        {
            return CommonHandler(request, async () =>
            {
                await notifications.SendGeneral(Sanitize(request.Message), request.ScheduleAt);
                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
                return Results.Created($"/api/comunications/general/{response.Id}", response);
            });
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitGeneralNotification")
        .WithOpenApi();

        apiV1.MapPost("/notifications/grade", (NotificationRequest request, INotifications notifications) =>
        {
            return CommonHandler(request, async () =>
            {
                await notifications.SendToGrade(request.GradeId, Sanitize(request.Message));
                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
                return Results.Created($"/api/comunications/grade/{response.Id}", response);
            });
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitGradeNotification")
        .WithOpenApi();

        apiV1.MapPost("/notifications/student", (NotificationRequest request, INotifications notifications) =>
        {
            return CommonHandler(request, async () =>
            {
                await notifications.SendStudent(request.StudentId, Sanitize(request.Message));
                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
                return Results.Created($"/api/comunications/student/{response.Id}", response);
            });
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitStudentNotification")
        .WithOpenApi();

        apiV1.MapPost("/notifications/disciplinary", (NotificationRequest request, INotifications notifications) =>
        {
            return CommonHandler(request, async () =>
            {
                await notifications.SendDisciplinary(request.StudentId, request.TeacherId, Sanitize(request.Message));
                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);
                return Results.Created($"/api/comunications/disciplinary/{response.Id}", response);
            });
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitDisciplinaryNotification")
        .WithOpenApi();
    }

    // Sanitizes the parameter. Just a placeholder for now.
    public static T Sanitize<T>(T parameter)
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
            return TypedResults.BadRequest(e.Message);
        }
        catch (Exception)
        {
            return TypedResults.InternalServerError();
        }
    }
}

internal record NotificationRequest(int GradeId, int StudentId, int TeacherId, string Message, DateTime ScheduleAt);
internal record NotificationResponse(Guid Id, string Message, int RecipientsAddressed);
