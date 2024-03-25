using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_Project.Models;
using System.Diagnostics;

namespace PRN221_Project.Pages.Classes
{
    public class ManagementModel : PageModel
    {
        private readonly PRN221_ProjectContext context;

        public ManagementModel(PRN221_ProjectContext context)
        {
            this.context = context;
        }

        public List<Calendar> Calendars { get; set; }


        public IActionResult OnPost(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a file.");
                return Page();
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Please select a CSV file.");
                return Page();
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        if (values.Length != 5)
                        {
                            continue;
                        }

                        string clast = values[0];
                        string subject = values[1];
                        string teacher = values[2];
                        string room = values[3];
                        string timeSlot = values[4].Trim().ToUpper();

                        if (!IsValidTimeSlot(timeSlot) || IsDuplicateEntry(clast, subject, teacher, room, timeSlot) || !CheckConflict(clast, subject, teacher, room, timeSlot))
                        {
                            continue;
                        }

                        context.Calendars.Add(new Calendar
                        {
                            Class = clast,
                            Subject = subject,
                            Teacher = teacher,
                            Room = room,
                            TimeSlot = timeSlot
                        });
                    }

                    context.SaveChanges();
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error importing CSV file: {ex.Message}");
                return Page();
            }
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

        public IActionResult OnGet()
        {
            Calendars = context.Calendars.ToList();
            return Page();
        }

        public IActionResult OnPostAdd(string txtClass, string txtSubject, string txtTeacher, string txtRoom, string txtTime)
        {
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

            var newCalendar = new Calendar
            {
                Class = txtClass,
                Subject = txtSubject,
                Teacher = txtTeacher,
                Room = txtRoom,
                TimeSlot = txtTime
            };

            context.Calendars.Add(newCalendar);
            context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendar = context.Calendars.Find(id);

            if (calendar == null)
            {
                return NotFound();
            }

            context.Calendars.Remove(calendar);
            context.SaveChanges();

            return RedirectToPage();
        }

    }
}

