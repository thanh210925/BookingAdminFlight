using Microsoft.AspNetCore.Mvc;
using BookingAdmin.Services;
using BookingAdmin.Models;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class TicketOrderController : Controller
    {
        private readonly FirestoreService _firestore;

        public TicketOrderController()
        {
            _firestore = new FirestoreService();
        }

        // 🔹 Danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _firestore.GetAllAsync<TicketOrder>("Tickets");
            return View(orders);
        }

        // 🔹 Xem chi tiết
        public async Task<IActionResult> Details(string id)
        {
            var order = await _firestore.GetByIdAsync<TicketOrder>("Tickets", id);
            if (order == null) return NotFound();
            return View(order);
        }

        // 🔹 Cập nhật trạng thái
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            await _firestore.UpdateFieldAsync("Tickets", id, "status", status);
            TempData["Success"] = "✅ Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Xóa đơn hàng
        public async Task<IActionResult> Delete(string id)
        {
            await _firestore.DeleteAsync("Tickets", id);
            TempData["Success"] = "🗑️ Đã xóa đơn hàng!";
            return RedirectToAction(nameof(Index));
        }
    }
}
