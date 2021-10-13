namespace WebApplication.Events
{
    public class StupidEventMessageMapper : JsonMessageMapper<StupidEvent>
    {
        public const string TOPIC = "stupidevent";
        public override string Topic => TOPIC;
    }
}