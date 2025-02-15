using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModel;

namespace WhiteLagoon.Web.Controllers
{   
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ;
        }

        // GET: VillaController
        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            // VillaNumberVM villaNumberVM = new()
            // {
            //     VillaList =  _db.Villas.ToList().Select(u => new SelectListItem
            //     {
            //         Text = u.Name,
            //         Value = u.Id.ToString()
            //     })
            // };
            // return View(villaNumberVM);
            VillaNumberVM villaNumberVM = new VillaNumberVM();
            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            // ModelState.Remove("Villa");

            // bool roomNumberExists = _db.VillaNumbers.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
            // if(ModelState.IsValid && !roomNumberExists)
            // {
            //     _db.VillaNumbers.Add(obj.VillaNumber);
            //     _db.SaveChanges();
            //     TempData["success"]="The villa number has been created successfully.";
            //     return RedirectToAction("Index");
            // }
            // if(roomNumberExists)
            // {
            //     TempData["error"]="The villa number already exists.";
            // }
            // obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            // {
            //     Text = u.Name,
            //     Value = u.Id.ToString()
            // });
            // return View(obj);
            bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExists)
            {
                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"]="The villa number has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            if(roomNumberExists)
            {
                TempData["error"]="The villa number already exists.";
            }
            obj.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int VillaNumberId)
        {
            // VillaNumberVM villaNumberVM = new()
            // {
            //     VillaList =  _db.Villas.ToList().Select(u => new SelectListItem
            //     {
            //         Text = u.Name,
            //         Value = u.Id.ToString()
            //     }),
            //     VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == VillaNumberId)
            // };
            // if(villaNumberVM.VillaNumber == null)
            // {
            //     return RedirectToAction("Error", "Home");
            // }
            // return View(villaNumberVM);
            VillaNumberVM villaNumberVM = new VillaNumberVM();
            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            });
            villaNumberVM.VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == VillaNumberId);
            if(villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            // if(ModelState.IsValid)
            // {
            //     _db.VillaNumbers.Update(villaNumberVM.VillaNumber);
            //     _db.SaveChanges();
            //     TempData["success"] = "The villa number has been updated successfully.";
            //     return RedirectToAction("Index");
            // }
        
            // villaNumberVM.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            // {
            //     Text = u.Name,
            //     Value = u.Id.ToString()
            // });
            // return View(villaNumberVM);
            if(ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }      
            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaNumberVM);
        }
        public IActionResult Delete (int VillaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM();
            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            });
            villaNumberVM.VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == VillaNumberId);
            if(villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);

        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFormDb = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            if(objFormDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(objFormDb);
                _unitOfWork.Save();
                TempData["success"]="The villa number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"]="The villa number could not be deleted.";
            return View();
        }
    }
}
