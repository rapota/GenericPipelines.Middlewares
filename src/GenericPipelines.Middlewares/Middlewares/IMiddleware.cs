namespace GenericPipelines.Middlewares;

public delegate Task<TResponse> NextMiddlewareDelegate<in TRequest, TResponse>(TRequest request, CancellationToken ct);

public interface IMiddleware<TRequest, TResponse>
{
    Task<TResponse> InvokeAsync(TRequest request, NextMiddlewareDelegate<TRequest, TResponse> next, CancellationToken ct);
}