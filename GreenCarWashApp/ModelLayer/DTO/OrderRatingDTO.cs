using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.DTO
{
    public  class OrderRatingDTO
    {
        public int OrderId { get; set; }
        public int Rating { get; set; } 
        public string? Comment { get; set; }
        public string? Review { get; set; }
    }
}
