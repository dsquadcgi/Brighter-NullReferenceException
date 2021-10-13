using System;
using Paramore.Brighter;

namespace WebApplication.Events
{
    public class StupidEvent : Event
    {
        public DateTime CreationDate { get; set; }

        public StupidEvent() : base(Guid.NewGuid())
        {
        }

        public StupidEvent(Guid id) : base(id)
        {
        }
    }
}