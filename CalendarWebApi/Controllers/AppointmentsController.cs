using CalendarWebApi.Models;
using CalendarWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AppointmentsController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int? user)
        {
            if (user == null)
            {
                return await _context.Appointments.Where(e => !((e.End <= start) || (e.Start >= end))).Include(e => e.User).ToListAsync();
            }
            else
            {
                return await _context.Appointments.Where(e => e.User.Id == user && !((e.End <= start) || (e.Start >= end))).Include(e => e.User).ToListAsync();
            }

            //return await _context.Appointments.ToListAsync();
        }

        [HttpGet("free")]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string partner)
        {
            return await _context.Appointments.Where(e => (e.Status == "free" || (e.Status != "free" && e.PartnerId == partner)) && !((e.End <= start) || (e.Start >= end))).Include(e => e.User).ToListAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentSlot>> GetAppointmentSlot(int id)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);

            if (appointmentSlot == null)
            {
                return NotFound();
            }

            return appointmentSlot;
        }

        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentSlot(int id, AppointmentSlotUpdate update)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }

            appointmentSlot.Start = update.Start;
            appointmentSlot.End = update.End;

            if (update.Name != null)
            {
                appointmentSlot.PartnerName = update.Name;
            }
            if (update.Status != null)
            {
                appointmentSlot.Status = update.Status;
            }

            _context.Appointments.Update(appointmentSlot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AppointmentSlotUpdate
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string? Name { get; set; }
            public string? Status { get; set; }

        }


        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/request")]
        public async Task<IActionResult> PutAppointmentSlotRequest(int id, AppointmentSlotRequest slotRequest)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }

            appointmentSlot.PartnerName = slotRequest.Name;
            appointmentSlot.PartnerId = slotRequest.Partner;
            appointmentSlot.Status = "waiting";

            _context.Appointments.Update(appointmentSlot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AppointmentSlotRequest
        {
            public string Partner { get; set; }
            public string Name { get; set; }

        }

        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentSlot(AppointmentSlot appointmentSlot)
        {
            _context.Appointments.Add(appointmentSlot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointmentSlot", new { id = appointmentSlot.Id }, appointmentSlot);
        }

        [HttpPost("create")]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentSlots(AppointmentSlotRange range)
        {

            var user = await _context.Users.FindAsync(range.Resource);

            if (user == null)
            {
                return BadRequest();
            }

            var slots = Timeline.GenerateSlots(range.Start, range.End, range.Scale);
            slots.ForEach(slot => {
                slot.User = user;
                _context.Appointments.Add(slot);
            });

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("clear")]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentClear(ClearRange range)
        {
            var start = range.Start;
            var end = range.End;

            _context.Appointments.RemoveRange(_context.Appointments.Where(e => e.Status == "free" && !((e.End <= start) || (e.Start >= end))));
            await _context.SaveChangesAsync();

            return NoContent();
        }


        public class ClearRange
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        public class AppointmentSlotRange
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Resource { get; set; }

            public string Scale { get; set; }
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentSlot(int id)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointmentSlot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentSlotExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
