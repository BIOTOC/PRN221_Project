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

        public Calender2 Calendar { get; private set; }
        public string ErrorMessage { get; set; }


        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Calendar = context.Calender2s.FirstOrDefault(x => x.Id == id); 

            if (Calendar == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int id, string txtClass, string txtSubject, string txtTeacher, string txtRoom, string txtTime)
        {
            var calendarToUpdate = context.Calender2s.Find(id);

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
                ErrorMessage =  "Invalid time slot.";
                Calendar = context.Calender2s.Find(id);

                return Page();
            }

            if (IsDuplicateEntry(txtClass, txtSubject, txtTeacher, txtRoom, txtTime))
            {
                ErrorMessage = "Duplicate entry.";
                Calendar = context.Calender2s.Find(id);
                return Page();
            }

            if (!CheckConflict(txtClass, txtSubject, txtTeacher, txtRoom, txtTime))
            {
               ErrorMessage = "Conflict detected.";
                Calendar = context.Calender2s.Find(id);
               return Page();
            }

            calendarToUpdate.Class = txtClass;
            calendarToUpdate.Subject = txtSubject;
            calendarToUpdate.Teacher = txtTeacher;
            calendarToUpdate.Room = txtRoom;
            calendarToUpdate.Session = txtTime.Substring(0, 1);
            calendarToUpdate.Slot1 = int.Parse(txtTime.Substring(1, 1));
            calendarToUpdate.Slot2 = int.Parse(txtTime.Substring(2, 1));

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
            return context.Calender2s.Any(e => e.Id == id);
        }

        private bool IsValidTimeSlot(string timeSlot)
        {
            const string ValidTimeSlots = "A24,A35,A46,A52,A63,A42,A53,A64,A25,A36,P24,P35,P46,P52,P63,P42,P53,P64,P25,P36";
            return ValidTimeSlots.Split(',').Contains(timeSlot);
        }

        private bool IsDuplicateEntry(string clast, string subject, string teacher, string room, string timeSlot)
        {
            return context.Calender2s.Any(item =>
                item.Class == clast &&
                item.Subject == subject &&
                item.Teacher == teacher &&
                item.Room == room &&
                item.Session == timeSlot.Substring(0, 1) &&
                item.Slot1 == int.Parse(timeSlot.Substring(1, 1)) &&
                item.Slot2 == int.Parse(timeSlot.Substring(2, 1))
            );
        }

        private bool CheckConflict(string clast, string subject, string teacher, string room, string time)
        {
            return !context.Calender2s.Any(item =>
                (item.Room == room && (item.Session + item.Slot1 + item.Slot2) == time && (item.Class != clast || item.Teacher != teacher || item.Subject != subject)) ||
                (item.Class == clast && item.Session == time.Substring(0, 1) && item.Slot1 == int.Parse(time.Substring(1, 1)) && item.Slot2 == int.Parse(time.Substring(2, 1)) && (item.Room != room || item.Teacher != teacher || item.Subject != subject)) ||
                ((item.Session + item.Slot1 + item.Slot2) == time && item.Teacher == teacher && (item.Room != room || item.Class != clast || item.Subject != subject)));
        }
    }
}
