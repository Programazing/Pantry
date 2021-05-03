using System;

namespace Pantry.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Expiration { get; set; }
        public int Weight { get; set; }
        public int Count { get; set; }
    }
}
