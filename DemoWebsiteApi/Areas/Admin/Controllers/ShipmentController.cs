using DemoWebsite.DataAccess.Repository;
using DemoWebsite.DataAccess.Repository.IRepository;
using DemoWebsite.Models.ViewModels;
using DemoWebsiteApi.Models;
using DemoWebsiteApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoWebsiteApi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ShipmentController : Controller
    {
        private readonly IUnitOfWork _db;

        public ShipmentController(IUnitOfWork db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Shipment> shipments = _db.ShipmentRepository.GetAll(includeProperties: "ApplicationUser").ToList();
            return View(shipments);
        }

        public IActionResult Upsert(int? id)
        {
            ShipmentVM shipmentVM = new ShipmentVM()
            {
                UserList = _db.ApplicationUserRepository.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                ).ToList(),
                Shipment = new Shipment()
            };
            if (id == null || id == 0)
            {
                //create
                return View(shipmentVM);
            }
            else
            {
                //update
                shipmentVM.Shipment = _db.ShipmentRepository.Get(u => u.ShipmentId == id);
                return View(shipmentVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ShipmentVM p)
        {
            if (ModelState.IsValid)
            {

                if (p.Shipment.ShipmentId == 0)
                {
                    //p.Shipment.CreatedDate = DateTime.Now;
                    _db.ShipmentRepository.Add(p.Shipment);
                }
                else
                {


                    _db.ShipmentRepository.Update(p.Shipment);

                }
                _db.save();
                return RedirectToAction("Index");
            }
            else
            {
                p.UserList = _db.ShipmentRepository.GetAll().Select(
               u => new SelectListItem
               {
                   Text = u.ApplicationUser.Name,
                   Value = u.ApplicationUserId.ToString()
               }
               ).ToList();
                return View(p);
            }

        }

        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                var objFromDb = _db.ShipmentRepository.Get(u => u.ShipmentId == id);
                return View(objFromDb);
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.ShipmentRepository.Get(u => u.ShipmentId == id);
            _db.ShipmentRepository.Remove(obj);
            _db.save();
            return RedirectToAction("Index");

        }

    }
}
