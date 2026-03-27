namespace IncidentManager.API.Endpoints.Webhooks;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        // TODO MVP-2: POST /api/webhooks/prometheus, POST /api/webhooks/gitea
        app.MapGroup("/api/webhooks").WithTags("Webhooks");
        return app;
    }
}
