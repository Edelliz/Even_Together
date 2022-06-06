namespace Backend3.Storage
{
    public class Review
    {
        public Guid EventId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public User Owner { get; set; }
    }
}
