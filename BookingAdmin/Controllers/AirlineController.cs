using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class AirlineController : Controller
    {
        private readonly FirestoreService _firestore;

        public AirlineController()
        {
            _firestore = new FirestoreService();
        }

        public async Task<IActionResult> Index()
        {
            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            return View(airlines);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Airline airline)
        {
            await _firestore.AddAsync("Airlines", airline);
            TempData["Success"] = "✅ Đã thêm hãng bay mới!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _firestore.GetByIdAsync<Airline>("Airlines", id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Airline airline)
        {
            await _firestore.UpdateAsync("Airlines", airline.Id, airline);
            TempData["Success"] = "✏️ Cập nhật hãng bay thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _firestore.GetByIdAsync<Airline>("Airlines", id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestore.DeleteAsync("Airlines", id);
            TempData["Success"] = "🗑️ Đã xóa hãng bay!";
            return RedirectToAction(nameof(Index));
        }
    }
}
