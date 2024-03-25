using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN221_Project.Models;


namespace PRN221_Project.Pages.Classes
{
    public class TimeTableModel : PageModel
    {
        private readonly PRN221_ProjectContext _context;

        public TimeTableModel(PRN221_ProjectContext context)
        {
            _context = context;
            TimeSlot = new Dictionary<Tuple<int, int>, Calender2>();
        }

        public Dictionary<Tuple<int, int>, Calender2> TimeSlot;

        public List<Calender2> Calendars { get; set; }
        String ca = "KhangPQ";
        public void OnGet()
        {
            
            Calendars = _context.Calender2s.Where(x => x.Teacher.Equals(ca)).ToList();
            foreach (var c in Calendars)
            {
                int session = getSession(c.Session);
                Tuple<int, int> tuple = Tuple.Create(1 + session, c.Slot1);
                Tuple<int, int> tuple2 = Tuple.Create(2 + session, c.Slot2);

                
                TimeSlot.Add(tuple, c);
                TimeSlot.Add(tuple2, c);

            }
        }

        public int getSession(string session)
        {
            if (session.Equals("A"))
            {
                return 0;
            }
            if (session.Equals("P"))
            {
                return 2;
            }
            return 0;
        }
    }
}
