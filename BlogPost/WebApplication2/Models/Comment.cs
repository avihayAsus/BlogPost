using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Post))]
        public Guid PostId { get; set; }

    }



}
