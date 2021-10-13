using System.Text.Json;
using Paramore.Brighter;

namespace WebApplication.Events
{
    public abstract class JsonMessageMapper<T> : IAmAMessageMapper<T> where T : Event
    {
        public abstract string Topic { get; }

        public Message MapToMessage(T request)
        {
            var key = request.GetType().Name.ToLower();
            var header = new MessageHeader(request.Id, key, MessageType.MT_EVENT);
            var body = new MessageBody(JsonSerializer.Serialize(request));
            var message = new Message(header, body);
            return message;
        }

        public T MapToRequest(Message message)
        {
            var greetingCommand = JsonSerializer.Deserialize<T>(message.Body.Value);
            return greetingCommand;
        }
    }
}