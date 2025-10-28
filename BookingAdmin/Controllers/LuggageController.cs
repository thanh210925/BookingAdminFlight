using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class LuggageController : Controller
    {
        private readonly FirestoreService _firestore;

        public LuggageController()
        {
            _firestore = new FirestoreService();
        }

        // ✅ Lấy dữ liệu trong collection "Luggage"
        public async Task<IActionResult> Index()
        {
            var items = await _firestore.GetAllAsync<Luggage>("Luggage");
            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Luggage luggage)
        {
            await _firestore.AddAsync("Luggage", luggage); // ✅ Đổi thành Luggage (viết hoa)
            TempData["Success"] = "✅ Đã thêm hành lý thành công!";
            return RedirectToAction(nameof(Index));
        }
        // 🟡 FORM SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _firestore.GetByIdAsync<Luggage>("Luggage", id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Luggage luggage)
        {
            if (!ModelState.IsValid)
                return View(luggage);

            await _firestore.UpdateAsync("Luggage", luggage.Id, luggage);
            TempData["Success"] = "✏️ Cập nhật hành lý thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🔴 XÓA
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _firestore.GetByIdAsync<Luggage>("Luggage", id);
            if (item == null)
            {
                TempData["Error"] = "❌ Không tìm thấy hành lý cần xóa!";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // 🗑️ XÓA TRỰC TIẾP TRONG FIREBASE FIRESTORE
                await _firestore.DeleteAsync("Luggage", id);

                TempData["Success"] = "🗑️ Đã xóa hành lý khỏi Firestore!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"⚠️ Lỗi khi xóa: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}