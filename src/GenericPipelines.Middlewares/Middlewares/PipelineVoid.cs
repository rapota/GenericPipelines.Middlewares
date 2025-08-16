namespace GenericPipelines.Middlewares;

public class Pipeline<TRequest> : IGenericPipeline
{
    private readonly List<IMiddleware<TRequest>> _middlewares;

    public Pipeline(params IMiddleware<TRequest>[] middlewares)
    {
        _middlewares = middlewares.ToList();
        _middlewares.Reverse();
    }

    public IRequestHandler<TRequest> DecorateHandler(IRequestHandler<TRequest> requestHandler)
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
        foreach (IMiddleware<TRequest> middleware in _middlewares)
        {
            target = new MiddlewareLink(middleware, target);
        }

        return target;
    }

    private interface IChainLink : IRequestHandler<TRequest>;

    private sealed class TargetLink(IRequestHandler<TRequest> requestHandler) : IChainLink
    {
        public Task HandleAsync(TRequest request, CancellationToken token) =>
            requestHandler.HandleAsync(request, token);

        public override string? ToString() => requestHandler.ToString();
    }

    private sealed class MiddlewareLink(IMiddleware<TRequest> middleware, IChainLink nextLink) : IChainLink
    {
        public Task HandleAsync(TRequest request, CancellationToken token) =>
            middleware.InvokeAsync(request, nextLink.HandleAsync, token);

        public override string? ToString() => middleware.ToString();
    }
}
