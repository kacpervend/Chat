namespace Application.DTO
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public HashSet<MessageDTO>? Messages { get; set; }
    }
}
