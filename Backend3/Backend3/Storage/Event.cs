using System.ComponentModel.DataAnnotations;

namespace Backend3.Storage
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }
        public string Organizer { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public string? Poster { get; set; }
    }

    public class Searching
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
