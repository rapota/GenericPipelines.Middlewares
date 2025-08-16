namespace GenericPipelines.Middlewares.Tests;

public class PipelineTests
{
    class FooHandler : IRequestHandler<int, string>
    {
        public Task<string> HandleAsync(int request, CancellationToken ct = default) => Task.FromResult(request.ToString());
    }

    class Middleware1 : IMiddleware<int, string>
    {
        public async Task<string> InvokeAsync(int request, NextMiddlewareDelegate<int, string> next, CancellationToken ct)
        {
            request++;
            string result = await next(request, ct);
            return result + "+M1";
        }
    }

    class Middleware2 : IMiddleware<int, string>
    {
        public async Task<string> InvokeAsync(int request, NextMiddlewareDelegate<int, string> next, CancellationToken ct)
        {
            request += 2;
            string result = await next(request, ct);
            return result + "+M2";
        }
    }

    [Fact]
    public void EmptyPipelineTest()
    {
        FooHandler handler = new();

        Pipeline<int, string> pipeline = new();
        IRequestHandler<int, string> decorateHandler = pipeline.DecorateHandler(handler);

        Assert.Same(handler, decorateHandler);
    }

    [Fact]
    public async Task PipelineDecorationTest()
    {
        FooHandler handler = new();

        Pipeline<int, string> pipeline = new(new Middleware1());

        IRequestHandler<int, string> decorateHandler = pipeline.DecorateHandler(handler);
        string result = await decorateHandler.HandleAsync(1);

        Assert.Equal("2+M1", result);
    }

    [Fact]
    public async Task PipelineNestingTest()
    {
        FooHandler handler = new();

        Pipeline<int, string> pipeline = new(
            new Middleware1(),
            new Middleware2());

        IRequestHandler<int, string> decorateHandler = pipeline.DecorateHandler(handler);
        string result = await decorateHandler.HandleAsync(1);

        Assert.Equal("4+M2+M1", result);
    }
}