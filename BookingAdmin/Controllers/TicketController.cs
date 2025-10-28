using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class TicketController : Controller
    {
        private readonly FirestoreService _firestore;

        public TicketController()
        {
            _firestore = new FirestoreService();
        }

        // ✅ Lấy dữ liệu trong collection "tickets"
        public async Task<IActionResult> Index()
        {
            var items = await _firestore.GetAllAsync<Ticket>("tickets");
            return View(items);
        }

        // 🟢 FORM THÊM
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            await _firestore.AddAsync("tickets", ticket); // ✅ Lưu vào "tickets"
            TempData["Success"] = "✅ Đã thêm vé mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🟡 FORM SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _firestore.GetByIdAsync<Ticket>("tickets", id);
            if (item == null)
            {
                TempData["Error"] = "❌ Không tìm thấy vé cần sửa!";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (!ModelState.IsValid)
                return View(ticket);

            await _firestore.UpdateAsync("tickets", ticket.Id, ticket); // ✅ Ghi vào "tickets"
            TempData["Success"] = "✏️ Cập nhật vé thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🔴 FORM XÓA
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _firestore.GetByIdAsync<Ticket>("tickets", id);
            if (item == null)
            {
                TempData["Error"] = "❌ Không tìm thấy vé cần xóa!";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // 🔴 XÁC NHẬN XÓA (XÓA LUÔN KHỎI FIREBASE)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _firestore.DeleteAsync("tickets", id); // ✅ Xóa khỏi "tickets"
                TempData["Success"] = "🗑️ Đã xóa vé khỏi Firestore!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"⚠️ Lỗi khi xóa: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
