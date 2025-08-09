namespace Services.Abstractions;

public interface IQueueAsyncMessage<TModel>
{
    Task<bool> ProduceAsync(TModel notification);
}