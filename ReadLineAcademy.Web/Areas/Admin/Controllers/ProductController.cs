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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _he;
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment he)
        {
            _unitOfWork = unitOfWork;
            _he = he;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product=new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(data =>
                                new SelectListItem
                                {
                                    Text = data.Name,
                                    Value = data.Id.ToString(),

                                }),
                CoverTypeList= _unitOfWork.CoverType.GetAll().Select(data =>
                                  new SelectListItem
                                  {
                                      Text = data.Name,
                                      Value = data.Id.ToString(),

                                  })
            };
            
            if (id == null || id==0)
            {
                //create
                return View(productVM);
            }
           else
            {
                //Update
                productVM.Product=_unitOfWork.Product.GetFirstOrDefault(data=>data.Id==id);
                return View(productVM);
            }
           
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
           if(ModelState.IsValid)
            {
                string wwwRootPath = _he.WebRootPath;
                if(file!=null)
                {
                    string fileName=Guid.NewGuid().ToString(); 
                    var uploads=Path.Combine(wwwRootPath,@"Images\ProductImages");
                    var extension=Path.GetExtension(file.FileName);

                    if(obj.Product.ImageURL!=null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using(var fileStreams=new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    };
                    obj.Product.ImageURL = @"\Images\ProductImages\" + fileName + extension;
                }
                if(obj.Product.Id==0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();
                    TempData["create"] = "Product Created Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                    TempData["create"] = "Product Updated Successfully";
                    return RedirectToAction("Index");

                }
            }
            return View();
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(data =>
                                new SelectListItem
                                {
                                    Text = data.Name,
                                    Value = data.Id.ToString(),

                                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(data =>
                                  new SelectListItem
                                  {
                                      Text = data.Name,
                                      Value = data.Id.ToString(),

                                  })
            };

            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(data => data.Id == id);
                return View(productVM);
            }

        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var peoductList=_unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new {data = peoductList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(data => data.Id == id);
            if (obj == null)
            {
                return Json(new
                {
                    success = false,
                    message="Error while Deleting"
                });
            }

            var oldImagePath = Path.Combine(_he.WebRootPath, obj.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(obj);

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
