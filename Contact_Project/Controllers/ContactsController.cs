using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contact_Project.Data;
using Contact_Project.Models;

namespace Contact_Project.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ContactRepository _repository = new ContactRepository();

        public ActionResult Index(string searchTerm)
        {
            var contacts = _repository.GetAllContacts();

            // If a search term is provided, filter contacts
            if (!string.IsNullOrEmpty(searchTerm))
            {
                contacts = contacts.Where(c =>
                                            (c.FirstName + " " + c.LastName).ToLower().Contains(searchTerm.ToLower()) ||
                                            c.Email.ToLower().Contains(searchTerm.ToLower()) ||
                                            c.Organization.ToLower().Contains(searchTerm.ToLower()) ||
                                            c.Phone.ToLower().Contains(searchTerm.ToLower()))
                                   .ToList();
            }

            return View(contacts);
        }

        public ActionResult Create()
        {
            var contactTypes = _repository.GetAllContactTypes(); // Fetch contact types from the repository

            // Convert to SelectListItem for dropdown
            ViewBag.ContactTypes = contactTypes.Select(ct => new SelectListItem
            {
                Value = ct.Name,
                Text = ct.Name
            }).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Contact contact, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    string imageDirectory = Server.MapPath("~/Images/");
                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    string imageFileName = Path.GetFileName(ImageUpload.FileName);
                    string imagePath = Path.Combine(imageDirectory, imageFileName);

                    // Save the file
                    ImageUpload.SaveAs(imagePath);

                    // Save the image path to the contact model
                    contact.ImagePath = "/Images/" + imageFileName;
                }

                // Add contact
                _repository.AddContact(contact);
                return RedirectToAction("Index");
            }

            // Reload contact types in case of validation failure
            ViewBag.ContactTypes = _repository.GetAllContactTypes()
                .Select(ct => new SelectListItem
                {
                    Value = ct.Name,
                    Text = ct.Name
                }).ToList();

            return View(contact);
        }

        public ActionResult Edit(int id)
        {
            var contact = _repository.GetContactById(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            // Populate dropdown list
            ViewBag.ContactTypes = _repository.GetAllContactTypes()
                .Select(ct => new SelectListItem
                {
                    Value = ct.Name,
                    Text = ct.Name
                }).ToList();

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contact contact, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageUpload.FileName);
                    string uploadsPath = Server.MapPath("~/Uploads");

                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    string filePath = Path.Combine(uploadsPath, fileName);
                    ImageUpload.SaveAs(filePath);

                    contact.ImagePath = "/Uploads/" + fileName;
                }
                else if (string.IsNullOrEmpty(contact.ImagePath))
                {
                    var existingContact = _repository.GetContactById(contact.Id);
                    contact.ImagePath = existingContact.ImagePath;
                }

                _repository.UpdateContact(contact);
                return RedirectToAction("Index");
            }

            ViewBag.ContactTypes = _repository.GetAllContactTypes()
           .Select(ct => new SelectListItem
           {
               Value = ct.Id.ToString(),
               Text = ct.Name
           }).ToList();

            return View(contact);
        }

        public ActionResult Delete(int id)
        {
            _repository.DeleteContact(id);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var contact = _repository.GetContactById(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }
        [HttpPost]
        public JsonResult BulkDelete(List<int> contactIds)
        {
            if (contactIds == null || contactIds.Count == 0)
            {
                return Json(new { success = false, message = "No contacts selected for deletion." });
            }

            foreach (var id in contactIds)
            {
                var contact = _repository.GetContactById(id);
                if (contact != null)
                {
                    _repository.DeleteContact(id);
                }
            }

            return Json(new { success = true, message = $"{contactIds.Count} contacts deleted successfully." });
        }
    
    }
}
