namespace GenericPipelines.Middlewares;

public class Pipeline<TRequest, TResponse> : IGenericPipeline
{
    private readonly List<IMiddleware<TRequest, TResponse>> _middlewares;

    public Pipeline(params IMiddleware<TRequest, TResponse>[] middlewares)
    {
        _middlewares = middlewares.ToList();
        _middlewares.Reverse();
    }

    public IRequestHandler<TRequest, TResponse> DecorateHandler(IRequestHandler<TRequest, TResponse> requestHandler)
    {
        if (requestHandler == null)
        {
            throw new ArgumentNullException(nameof(requestHandler));
        }

        if (_middlewares.Count == 0)
        {
            return requestHandler;
        }

        IChainLink target = new TargetLink(requestHandler);
        foreach (IMiddleware<TRequest, TResponse> middleware in _middlewares)
        {
            target = new MiddlewareLink(middleware, target);
        }

        return target;
    }

    private interface IChainLink : IRequestHandler<TRequest, TResponse>;

    private sealed class TargetLink(IRequestHandler<TRequest, TResponse> requestHandler) : IChainLink
    {
        public Task<TResponse> HandleAsync(TRequest request, CancellationToken token) =>
            requestHandler.HandleAsync(request, token);

        public override string? ToString() => requestHandler.ToString();
    }

    private sealed class MiddlewareLink(IMiddleware<TRequest, TResponse> middleware, IChainLink nextLink) : IChainLink
    {
        public Task<TResponse> HandleAsync(TRequest request, CancellationToken token) =>
            middleware.InvokeAsync(request, nextLink.HandleAsync, token);

        public override string? ToString() => middleware.ToString();
    }
}