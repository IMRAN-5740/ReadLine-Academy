using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Repositories.Absractions;
using ReadLineAcademy.Repositories.Absractions.Base;
using ReadLineAcademy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReadLineAcademy.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;    
            
        }
        public IActionResult Index()
        {
            var coverTypes= _unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CoverType createModel)
        {
            var existingType=_unitOfWork.CoverType.GetFirstOrDefault(data=>data.Name==createModel.Name);
            if(existingType!=null)
            {
                ModelState.AddModelError("CoverTypeError", "This cover type already exists try another");

            }
            
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(createModel);
                _unitOfWork.Save();
                TempData["create"] = "CoverType Created Successfully";
                return RedirectToAction("Index");

            }
            return View(createModel);
        }
        public IActionResult Edit(int id) 
        {
            var existingCoverType = _unitOfWork.CoverType.GetFirstOrDefault(data => data.Id == id);
            if (existingCoverType == null)
            {
                return NotFound();
            }
            return View(existingCoverType);
        }
        [HttpPost]
        public IActionResult Edit(CoverType editModel) 
        {
            if (ModelState.IsValid)
            {
                var existingCoverType = _unitOfWork.CoverType.GetFirstOrDefault(data => data.Id == editModel.Id);
                if (existingCoverType == null)
                {
                    return NotFound();
                }
                existingCoverType.Name = editModel.Name;
                _unitOfWork.CoverType.Update(existingCoverType);

                _unitOfWork.Save();
                TempData["edit"] = "Cover Type Updated Successfully";

                return RedirectToAction("Index");
            }
            return View();
        }
        
        public IActionResult Details(int id)
        {
            var existingCoverType = _unitOfWork.CoverType.GetFirstOrDefault(data => data.Id == id);
            if (existingCoverType == null)
            {
                return NotFound();
            }
            return View(existingCoverType);
        }
        public IActionResult Delete(int id)
        {
            var existingCoverType = _unitOfWork.CoverType.GetFirstOrDefault(data => data.Id == id);
            if (existingCoverType == null)
            {
                return NotFound();
            }
            return View(existingCoverType);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            var existingCoverType = _unitOfWork.CoverType.GetFirstOrDefault(data => data.Id == id);
            if (existingCoverType == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(existingCoverType);

            _unitOfWork.Save();
            TempData["delete"] = "Cover Type Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
