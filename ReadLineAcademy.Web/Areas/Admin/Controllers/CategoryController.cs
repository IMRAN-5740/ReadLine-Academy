using ReadLineAcademy.Repositories;
using ReadLineAcademy.Repositories.Absractions;
using ReadLineAcademy.Repositories.Absractions.Base;
using ReadLineAcademy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadLineAcademy.Models.EntityModels;

namespace ReadLineAcademy.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {

        IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category createModel)
        {
            if (createModel.Name == createModel.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CategoryError", "The DisplayOrder cannot exactly match the Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(createModel);
                _unitOfWork.Save();
                TempData["create"] = "Category Created Successfully";
                return RedirectToAction("Index");

            }
            return View(createModel);
        }
        public IActionResult Edit(int id)
        {

            var existingCategory = _unitOfWork.Category.GetFirstOrDefault(data => data.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            return View(existingCategory);
        }
        [HttpPost]
        public IActionResult Edit(Category editModel)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = _unitOfWork.Category.GetFirstOrDefault(data => data.Id == editModel.Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                existingCategory.Name = editModel.Name;
                existingCategory.DisplayOrder = editModel.DisplayOrder;
                _unitOfWork.Category.Update(existingCategory);

                _unitOfWork.Save();
                TempData["edit"] = "Category Updated Successfully";

                return RedirectToAction("Index");
            }

            return View();
        }
        public IActionResult Details(int id)
        {
            var existingCategory = _unitOfWork.Category.GetFirstOrDefault(data => data.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            return View(existingCategory);
        }
        public IActionResult Delete(int? id)
        {
            var existingCategory = _unitOfWork.Category.GetFirstOrDefault(data => data.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            return View(existingCategory);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public IActionResult DeletePost(int? id)
        {

            var existingCategory = _unitOfWork.Category.GetFirstOrDefault(data => data.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(existingCategory);

            _unitOfWork.Save();
            TempData["delete"] = "Category Deleted Successfully";
            return RedirectToAction("Index");

        }


    }
}
