using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Models.ViewModels;
using ReadLineAcademy.Repositories.Absractions.Base;
using ReadLineAcademy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReadLineAcademy.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new();
           
            
            if (id == null || id==0)
            {
                //create
                return View(company);
            }
           else
            {
                //Update
                company=_unitOfWork.Company.GetFirstOrDefault(data=>data.Id==id);
                return View(company);
            }
           
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
           if(ModelState.IsValid)
            {
               
                    
                if(obj.Id==0)
                {
                    _unitOfWork.Company.Add(obj);
                    _unitOfWork.Save();
                    TempData["create"] = "Company Created Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    _unitOfWork.Save();
                    TempData["create"] = "Company Updated Successfully";
                    return RedirectToAction("Index");

                }
            }
            return View();
        }




        [HttpGet]
        public IActionResult Details(int? id)
        {
            Company company = new();


            if (id == null || id == 0)
            {
                //create
                return View(company);
            }
            else
            {
                //Update
                company = _unitOfWork.Company.GetFirstOrDefault(data => data.Id == id);
                return View(company);
            }

        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList=_unitOfWork.Company.GetAll();
            return Json(new {data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(data => data.Id == id);
            if (obj == null)
            {
                return Json(new
                {
                    success = false,
                    message="Error while Deleting"
                });
            }
            
            _unitOfWork.Company.Remove(obj);

            _unitOfWork.Save();
            //TempData["delete"] = "Product Deleted Successfully";
            return Json(new
            {
                success = true,
                message = "Product Deleted Succesfully"
            });
        }

        #endregion


    }
}
