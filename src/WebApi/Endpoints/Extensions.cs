using Application;
using Model.Ports.Driving;

namespace WebApi.Endpoints;

public static class Extensions
{
    public static void MapNotificationsEndPoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");

        apiV1.MapPost("/notifications/general", (NotificationRequest request , INotifications notificationsUseCase) =>
        {
            try
            {
                if (request.Message == "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4")
                {
                    throw new Exception();
                }

                notificationsUseCase.SendGeneral(Sanitize(request.Message));

                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);

                return Results.Created($"/api/comunications/general/{response.Id}", response);
            }
            catch (UseCaseException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitGeneralNotification")
        .WithOpenApi();

        apiV1.MapPost("/notifications/grade", (NotificationRequest request, INotifications notificationsUseCase) =>
        {
            try
            {
                if (request.Message == "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4")
                {
                    throw new Exception();
                }

                notificationsUseCase.SendToGrade(request.GradeId, Sanitize(request.Message));

                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);

                return Results.Created($"/api/comunications/grade/{response.Id}", response);
            }
            catch (UseCaseException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitGradeNotification")
        .WithOpenApi();

        apiV1.MapPost("/notifications/student", (NotificationRequest request, INotifications notificationsUseCase) =>
        {
            try
            {
                if (request.Message == "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4")
                {
                    throw new Exception();
                }

                notificationsUseCase.SendStudent(request.StudentId, Sanitize(request.Message));

                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);

                return Results.Created($"/api/comunications/student/{response.Id}", response);
            }
            catch (UseCaseException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitStudentNotification")
        .WithOpenApi();
    }

    // Sanitizes the parameter. Just a placeholder for now.
    public static T Sanitize<T>(T parameter)
    {
        return parameter;
    }
}

internal record NotificationRequest(int GradeId, int StudentId, string Message);
internal record NotificationResponse(Guid Id, string Message, int RecipientsAddressed);
