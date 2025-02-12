using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{   
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VillaController(ApplicationDbContext db)
        {
            _db = db ;
        }

        // GET: VillaController
        public IActionResult Index()
        {
            var villas = _db.Villas.ToList();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if( obj.Description == obj.Name)
            {
                ModelState.AddModelError("", "The description cannot exactly match the name");
            }
            if(ModelState.IsValid)
            {
                _db.Villas.Add(obj);
                _db.SaveChanges();
                TempData["success"]="The villa has been created successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be created.";

            return View();
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
            if(obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if(ModelState.IsValid)
            {
                _db.Villas.Update(obj);
                _db.SaveChanges();
                TempData["success"]="The villa has been updated successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be updated.";

            return View();
        }
        public IActionResult Delete (int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
            if(obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFormDb = _db.Villas.FirstOrDefault(u => u.Id == obj.Id);
            if(objFormDb is not null)
            {
                _db.Villas.Remove(objFormDb);
                _db.SaveChanges();
                TempData["success"]="The villa has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be deleted.";

            return View();
        }
    }
}
