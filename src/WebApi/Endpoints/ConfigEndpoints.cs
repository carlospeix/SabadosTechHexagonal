using Model.Ports.Driving;
using Model.UseCases;

namespace WebApi.Endpoints;

public static class ConfigEndpoints
{
    public static void MapNotificationsEndPoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");

        apiV1.MapPost("/notifications/global", (NotificationRequest request , INotifications notificationsUseCase) =>
        {
            try
            {
                notificationsUseCase.SendGlobal(Sanitize(request.Message));

                var response = new NotificationResponse(Guid.NewGuid(), request.Message, 1);

                return Results.Created($"/api/comunications/global/{response.Id}", response);
            }
            catch (UseCaseException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        })
        .Produces<NotificationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithName("SubmitGlobalNotification")
        .WithOpenApi();
    }

    // Sanitizes the parameter. Just a placeholder for now.
    public static T Sanitize<T>(T parameter)
    {
        return parameter;
    }
}

internal record NotificationRequest(string Message);
internal record NotificationResponse(Guid Id, string Message, int RecipientsAddressed);
