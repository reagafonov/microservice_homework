using System.Text.Json;
using Common.Abstractions;
using Confluent.Kafka;

namespace Common.Infrastructure.Queue;

public class KafkaMessageSerializer<T>:ISerializer<T> where T: IKafkaMessage
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}