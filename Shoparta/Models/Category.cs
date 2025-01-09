using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoparta.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; }

        public List<Item> Items { get; set; }

        public Category()
        {
            Items = new List<Item>();
        }
    }
}
