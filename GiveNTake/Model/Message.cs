using System;
namespace GiveNTake.Model
{
    public class Message
    {
        public string Body { get; set; }
        public int MessageId { get; set; }
        public Product Product { get; set; }
        public string Title { get; set; }

        public User FromUser { get; set; }
        public User ToUser { get; set; }
    }
}
