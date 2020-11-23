using CRUDapp.DAL;
using CRUDapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        /// <summary>
        /// Home page as a list of contacts added via modal window
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Trick: when request is sent via ajax by inspecting headers 
        /// then ajax requests send X-Requested-With header with value XMLHttpRequest.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            List<Contact> contacts = await _context.Contacts.AsNoTracking().ToListAsync();

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return PartialView("_ContactsPartial", contacts);
            }

            return View(contacts);
        }


        /// <summary>
        /// Add new contact
        /// </summary>
        /// <returns></returns>
        [HttpGet, ActionName("Contact")]
        public IActionResult Contact()
        {
            Contact model = new Contact();

            return PartialView("_ContactModalPartial", model);
        }


        [HttpPost, ActionName("Contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactAsync([Bind("Name,Email,Phone,Domain,Notes")] Contact model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.Contacts.AddAsync(model);
                    await _context.SaveChangesAsync();

                    CreateNotification($"The new contact ({model.Name}) added!");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ex.Message);
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return PartialView("_ContactModalPartial", model);
        }


        /// <summary>
        /// Modify existing contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return PartialView("_EditModalPartial", contact);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int id, [Bind("ID,Name,Email,Phone,Domain,Notes")] Contact contact)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    CreateNotification($"The contact (id={id}) has been updated!");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ex.Message);
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }

            return PartialView("_EditModalPartial", contact);
        }


        /// <summary>
        /// Remove data from database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                string message = $"Delete failed (id={id}). Try again, and if the problem persists " +
                                  "see your system administrator.";
                ViewData["ErrorMessage"] = message;
                _logger.LogWarning(message);
            }

            return PartialView("_DeleteModalPartial", contact);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            try
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync(); 

                CreateNotification($"The contact (id={id}) has been deleted!");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ex.Message);
                ModelState.AddModelError("", "Unable to delete.");

                return RedirectToAction(nameof(DeleteConfirmedAsync), new { id = id, saveChangesError = true });
            }

            return PartialView("_DeleteModalPartial", contact);
        }


        /// <summary>
        /// Show contact information
        /// </summary>
        /// <returns></returns>
        [HttpGet, ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsModalPartial", contact);
        }


        /// <summary>
        /// Documentation, project description
        /// </summary>
        /// <returns></returns>
        [HttpGet, ActionName("Doc")]
        public IActionResult Doc()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Notifications()
        {
            TempData.TryGetValue("Notifications", out object value);
            var notifications = value as IEnumerable<string> ?? Enumerable.Empty<string>();

            return PartialView("_NotificationsPartial", notifications);
        }


        [NonAction]
        private void CreateNotification(string message)
        {
            TempData.TryGetValue("Notifications", out object value);
            var notifications = value as List<string> ?? new List<string>();
            notifications.Add(message);
            TempData["Notifications"] = notifications;
        }
    }
}
