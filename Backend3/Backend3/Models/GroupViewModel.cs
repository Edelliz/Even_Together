namespace Backend3.Models
{
    public class GroupViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Owner { get; set; }
        public List<ShortUserViewModel> Users { get; set; }
        public List<ShortUserViewModel> Requests { get; set; }
        public int Size { get; set; }
    }

    public class CreateGroupViewModel
    {
        public  string Title { get; set; }
        public string? Description { get; set; }
        public int Size { get; set; }
    }
}
