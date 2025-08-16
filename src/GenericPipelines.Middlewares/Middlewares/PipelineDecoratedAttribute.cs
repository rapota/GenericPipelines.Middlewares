namespace GenericPipelines.Middlewares;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PipelineDecoratedAttribute<TPipelineType>  : Attribute
    where TPipelineType : IGenericPipeline;
