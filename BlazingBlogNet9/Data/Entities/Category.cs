using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlazingBlog.Data.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string Name { get; set; }

        [Required, MaxLength(125), Unicode(false)]
        public string Slug { get; set; }

        public Category Clone() => (Category)this.MemberwiseClone();
    }
}
