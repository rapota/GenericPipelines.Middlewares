namespace GenericPipelines.Middlewares;

public delegate Task NextVoidMiddlewareDelegate<in TRequest>(TRequest request, CancellationToken ct);

public interface IMiddleware<TRequest>
{
    Task InvokeAsync(TRequest request, NextVoidMiddlewareDelegate<TRequest> next, CancellationToken ct);
}
