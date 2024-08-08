using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Contact;
using Microsoft.AspNetCore.Authorization;
using App.Data;


#nullable disable

namespace App.Areas.Contact.Controllers
{
    [Area("Contact")]
    [Route("/contact/{action}")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage {set; get;}

        [TempData]
        public string TypeStatusMessage {set; get;}
        
        [Authorize(Roles = RoleName.Administrator)]
        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }

        [Authorize(Roles = RoleName.Administrator)]
        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactModel = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }
        
        [HttpPost] [AllowAnonymous]
        public async Task<ActionResult> SendContact([FromBody] ContactModel model)
        {
            if (string.IsNullOrEmpty(model.FullName)
                || string.IsNullOrEmpty(model.Email)
                || string.IsNullOrEmpty(model.Message))
                return Json(new {
                    success = 0,
                    alert = "Error! Please complete all information !"
                });

            Console.WriteLine(model.FullName);
            Console.WriteLine(model.Message);
            model.DateSent = DateTime.Now;
            _context.Add(model);
            await _context.SaveChangesAsync();

            return Json(new {
                success = 1,
                alert = "Success! Your contact has been sent!"
            });
        }

        [HttpPost]
        public JsonResult Post(int id)
        {
            return Json("Response from Get");
        }

        [HttpGet("/admin/contact/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactModel = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }

        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactModel = await _context.Contacts.FindAsync(id);
            if (contactModel != null)
            {
                _context.Contacts.Remove(contactModel);
                StatusMessage = $"Ban da xoa lien he \"{contactModel.Email}\"";
                TypeStatusMessage = TypeMessage.Warning;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
