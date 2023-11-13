using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmonisTest
{
    class AdmonisTest
    {
        public class AdmonisProduct
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string DescriptionLong { get; set; }
            public string Brand { get; set; }
            public List<List<AdmonisProductOption>> Options { get; } = new List<List<AdmonisProductOption>>();

        }

        public class AdmonisProductOption
        {
            public string optionName { get; set; }
            public string Size { get; set; } 
            public string Color { get; set; }
        }
    }
}
