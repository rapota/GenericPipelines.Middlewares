namespace GenericPipelines.Middlewares.Tests;

public class VoidPipelineTests
{
    class FooHandler : IRequestHandler<bool>
    {
        public Task HandleAsync(bool request, CancellationToken ct = default) => Task.CompletedTask;
    }

    class EmptyMiddleware : IMiddleware<bool>
    {
        public Task InvokeAsync(bool request, NextVoidMiddlewareDelegate<bool> next, CancellationToken ct) => next(request, ct);
    }
    
    class FooMiddleware : IMiddleware<bool>
    {
        public Task InvokeAsync(bool request, NextVoidMiddlewareDelegate<bool> next, CancellationToken ct) => throw new NotSupportedException();
    }

    [Fact]
    public void EmptyPipelineTest()
    {
        FooHandler handler = new();

        Pipeline<bool> pipeline = new();
        IRequestHandler<bool> decorateHandler = pipeline.DecorateHandler(handler);

        Assert.Same(handler, decorateHandler);
    }
    
    [Fact]
    public async Task PipelineDecorationTest()
    {
        FooHandler handler = new();

        Pipeline<bool> pipeline = new(new FooMiddleware());
        IRequestHandler<bool> decorateHandler = pipeline.DecorateHandler(handler);

        async Task InvokeHandler()
        {
            await decorateHandler.HandleAsync(false);
        }

        await Assert.ThrowsAsync<NotSupportedException>(InvokeHandler);
    }
    
    [Fact]
    public async Task PipelineNestingTest()
    {
        FooHandler handler = new();

        Pipeline<bool> pipeline = new(
            new EmptyMiddleware(),
            new FooMiddleware());
        IRequestHandler<bool> decorateHandler = pipeline.DecorateHandler(handler);

        async Task InvokeHandler()
        {
            await decorateHandler.HandleAsync(false);
        }

        await Assert.ThrowsAsync<NotSupportedException>(InvokeHandler);
    }
}