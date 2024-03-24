using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_Project.Models;

namespace PRN221_Project.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly PRN221_ProjectContext context;

        public EditModel(PRN221_ProjectContext _context)
        {
            this.context = _context;
        }

        public Calendar Calendar { get; private set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Calendar = context.Calendars.FirstOrDefault(x => x.Id == id); 

            if (Calendar == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int id, string txtClass, string txtSubject, string txtTeacher, string txtRoom, string txtTime)
        {
            var calendarToUpdate = context.Calendars.Find(id);

            if (calendarToUpdate == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!IsValidTimeSlot(txtTime))
            {
                ModelState.AddModelError(string.Empty, "Invalid time slot.");
                return Page();
            }

            if (IsDuplicateEntry(txtClass, txtSubject, txtTeacher, txtRoom, txtTime))
            {
                ModelState.AddModelError(string.Empty, "Duplicate entry.");
                return Page();
            }

            if (!CheckConflict(txtClass, txtSubject, txtTeacher, txtRoom, txtTime))
            {
                ModelState.AddModelError(string.Empty, "Conflict detected.");
                return Page();
            }

            calendarToUpdate.Class = txtClass;
            calendarToUpdate.Subject = txtSubject;
            calendarToUpdate.Teacher = txtTeacher;
            calendarToUpdate.Room = txtRoom;
            calendarToUpdate.TimeSlot = txtTime;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalendarExists(calendarToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect("/Home");
        }


        private bool CalendarExists(int id)
        {
            return context.Calendars.Any(e => e.Id == id);
        }

        private bool IsValidTimeSlot(string timeSlot)
        {
            const string ValidTimeSlots = "A24,A35,A46,A52,A63,A42,A53,A64,A25,A36,P24,P35,P46,P52,P63,P42,P53,P64,P25,P36";
            return ValidTimeSlots.Split(',').Contains(timeSlot);
        }

        private bool IsDuplicateEntry(string clast, string subject, string teacher, string room, string timeSlot)
        {
            return context.Calendars.Any(item =>
                item.Class == clast &&
                item.Subject == subject &&
                item.Teacher == teacher &&
                item.Room == room &&
                item.TimeSlot == timeSlot
            );
        }

        private bool CheckConflict(string clast, string subject, string teacher, string room, string time)
        {
            return !context.Calendars.Any(item =>
                (item.Room == room && item.TimeSlot == time && (item.Class != clast || item.Teacher != teacher || item.Subject != subject)) ||
                (item.Class == clast && item.TimeSlot == time && (item.Room != room || item.Teacher != teacher || item.Subject != subject)) ||
                (item.TimeSlot == time && item.Teacher == teacher && (item.Room != room || item.Class != clast || item.Subject != subject)));
        }
    }
}
