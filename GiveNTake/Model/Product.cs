using System;
using System.Collections.Generic;
namespace GiveNTake.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public string Title { get; set; }
        public User Owner { get; set; }
        public City City { get; set; }
        public Category Category { get; set; }
        public IList<ProductMedia> ProductMedias { get; set; } 
    }
}
