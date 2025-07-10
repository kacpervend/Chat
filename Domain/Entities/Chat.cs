namespace Domain.Entities
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
