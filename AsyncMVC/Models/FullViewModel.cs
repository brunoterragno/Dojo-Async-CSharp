using System.Collections.Generic;

namespace AsyncMVC.Models
{
    public class FullViewModel
    {
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Title> Titles { get; set; }
    }
}