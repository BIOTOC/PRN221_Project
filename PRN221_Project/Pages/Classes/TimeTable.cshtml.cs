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
        }

        public List<Calendar> Calendars { get; set; }
        public void OnGet()
        {
            Calendars = _context.Calendars.ToList();
        }
    }
}
