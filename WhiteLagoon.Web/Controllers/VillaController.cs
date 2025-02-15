using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Web.Controllers
{   
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ;
        }

        // GET: VillaController
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
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
                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["success"]="The villa has been created successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be created.";

            return View();
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
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
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["success"]="The villa has been updated successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be updated.";

            return View();
        }
        public IActionResult Delete (int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
            if(obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFormDb = _unitOfWork.Villa.Get(u => u.Id == obj.Id);
            if(objFormDb is not null)
            {
                _unitOfWork.Villa.Remove(objFormDb);
                _unitOfWork.Save();
                TempData["success"]="The villa has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"]="The villa could not be deleted.";

            return View();
        }
    }
}
