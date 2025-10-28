using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<IActionResult> Create()
        {
            // 🔹 Lấy dữ liệu sân bay & hãng bay từ Firestore
            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            var airports = await _firestore.GetAllAsync<Airport>("Airports");

            ViewBag.Airlines = airlines;
            ViewBag.Airports = airports;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            await _firestore.AddAsync("tickets", ticket);
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

            // 🔹 Lấy dữ liệu sân bay & hãng bay từ Firestore để hiển thị trong dropdown
            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            var airports = await _firestore.GetAllAsync<Airport>("Airports");

            ViewBag.Airlines = airlines;
            ViewBag.Airports = airports;

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (!ModelState.IsValid)
                return View(ticket);

            await _firestore.UpdateAsync("tickets", ticket.Id, ticket);
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

        // 🔴 XÁC NHẬN XÓA
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _firestore.DeleteAsync("tickets", id);
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
