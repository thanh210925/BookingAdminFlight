using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class AirportController : Controller
    {
        private readonly FirestoreService _firestore;

        public AirportController()
        {
            _firestore = new FirestoreService();
        }

        public async Task<IActionResult> Index()
        {
            var airports = await _firestore.GetAllAsync<Airport>("Airports");
            return View(airports);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Airport airport)
        {
            await _firestore.AddAsync("Airports", airport);
            TempData["Success"] = "✅ Đã thêm sân bay mới!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _firestore.GetByIdAsync<Airport>("Airports", id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Airport airport)
        {
            await _firestore.UpdateAsync("Airports", airport.Id, airport);
            TempData["Success"] = "✏️ Cập nhật sân bay thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _firestore.GetByIdAsync<Airport>("Airports", id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestore.DeleteAsync("Airports", id);
            TempData["Success"] = "🗑️ Đã xóa sân bay!";
            return RedirectToAction(nameof(Index));
        }
    }
}
