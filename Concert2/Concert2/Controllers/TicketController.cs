using Concert.Data;
using Concert.Data.Entities;
using Concert.Helpers;
using Concert.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Concert.Controllers
{
    public class TicketController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public TicketController(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.
                ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                ModelState.AddModelError(string.Empty, "Ticket no existe.");
            }
            if (ticket.WasUsed != false)
            {
                ModelState.AddModelError(string.Empty, "Ticket ya es usado.");
                return RedirectToAction(nameof(DetailsTicket), new { Id = ticket.Id });
            }
            return RedirectToAction(nameof(EditTicket), new { Id = ticket.Id });
        }

        public async Task<IActionResult> DetailsTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public async Task<IActionResult> EditTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un ticket con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(ticket);
        }


    }
}

//NOS FALTA

//Vista de index