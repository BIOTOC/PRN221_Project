using System;
using System.Collections.Generic;

namespace PRN221_Project.Models
{
    public partial class Calendar
    {
        public int Id { get; set; }
        public string? Class { get; set; }
        public string? Subject { get; set; }
        public string? Teacher { get; set; }
        public string? Room { get; set; }
        public string? TimeSlot { get; set; }
    }
}
