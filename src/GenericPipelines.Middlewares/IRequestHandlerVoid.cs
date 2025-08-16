namespace GenericPipelines;

public interface IRequestHandler<in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken ct = default);
}
