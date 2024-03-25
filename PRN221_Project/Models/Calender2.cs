using System;
using System.Collections.Generic;

namespace PRN221_Project.Models
{
    public partial class Calender2
    {
        public int Id { get; set; }
        public string Class { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Teacher { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string Session { get; set; } = null!;
        public int Slot1 { get; set; }
        public int Slot2 { get; set; }
    }
}
