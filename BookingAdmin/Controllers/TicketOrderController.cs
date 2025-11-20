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

        // ==========================
        // 🔹 Danh sách đơn hàng
        // ==========================
        public async Task<IActionResult> Index()
        {
            var orders = await _firestore.GetAllAsync<TicketOrder>("Tickets");
            return View(orders);
        }

        // ==========================
        // 🔹 Xem chi tiết đơn
        // ==========================
        public async Task<IActionResult> Details(string id)
        {
            var order = await _firestore.GetByIdAsync<TicketOrder>("Tickets", id);
            if (order == null) return NotFound();
            return View(order);
        }

        // ==========================
        // 🔹 Update trạng thái THEO BOOKING ID
        // ==========================
        [HttpPost]
        public async Task<IActionResult> UpdateStatusByBooking(string bookingId, string status)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                TempData["Error"] = "❌ bookingId trống!";
                return RedirectToAction(nameof(Index));
            }

            await _firestore.UpdateStatusByBookingAsync("Tickets", bookingId, status);

            TempData["Success"] = $"✅ Đã cập nhật {status} cho tất cả vé thuộc booking {bookingId}!";
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // 🔹 Xóa TẤT CẢ vé theo bookingId
        // ==========================
        public async Task<IActionResult> DeleteByBooking(string bookingId)
        {
            var tickets = await _firestore.GetAllAsync<TicketOrder>("Tickets");

            foreach (var t in tickets)
            {
                if (t.BookingId == bookingId)
                    await _firestore.DeleteAsync("Tickets", t.Id);
            }

            TempData["Success"] = "🗑️ Đã xóa toàn bộ vé trong cùng booking!";
            return RedirectToAction(nameof(Index));
        }
    }
}
