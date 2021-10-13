namespace WebApplication.Models
{
    public class Settings
    {
        public const int StatusCodeWhenReproduced = 123;
        public string AMQP { get; set; }
        public string AMQPExchange { get; set; }
    }
}