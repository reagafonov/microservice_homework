namespace Consumer.Options
{    
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }

        public string? GroupID { get; set; }
    }
}