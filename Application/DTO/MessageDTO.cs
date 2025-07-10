namespace Application.DTO
{
    public class MessageDTO
    {
        public Guid MessageId { get; set; }
        public string Sender { get; set; }
        public string MessageText { get; set; }
        public Guid ChatId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
