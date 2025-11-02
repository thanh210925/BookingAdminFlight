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

        // ✅ Danh sách vé
        public async Task<IActionResult> Index()
        {
            var items = await _firestore.GetAllAsync<Ticket>("tickets");
            return View(items);
        }

        // 🟢 Hiển thị form thêm
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            ViewBag.Airports = await _firestore.GetAllAsync<Airport>("Airports");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Ticket ticket)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    TempData["Error"] = "⚠️ Dữ liệu không hợp lệ!";
        //    //    ViewBag.Airlines = await _firestore.GetAllAsync<Airline>("Airlines");
        //    //    ViewBag.Airports = await _firestore.GetAllAsync<Airport>("Airports");
        //    //    return View(ticket);
        //    //}

        //    //await _firestore.AddAsync("tickets", ticket);
        //    //TempData["Success"] = "✅ Đã thêm vé mới thành công!";
        //    //return RedirectToAction(nameof(Index));
        //    if (!ModelState.IsValid)
        //    {
        //        // Log lỗi chi tiết để xem
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();
        //        Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));

        //        TempData["Error"] = "Dữ liệu không hợp lệ!";
        //        return View(ticket);
        //    }

        //    await _firestore.AddAsync("tickets", ticket);
        //    TempData["Success"] = "Thêm vé thành công!";
        //    return RedirectToAction("Index");
        //}
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi chi tiết để xem
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));

                TempData["Error"] = "⚠️ Dữ liệu không hợp lệ!";

                // 🟢 Gán lại ViewBag để tránh null khi render lại view
                ViewBag.Airlines = await _firestore.GetAllAsync<Airline>("Airlines");
                ViewBag.Airports = await _firestore.GetAllAsync<Airport>("Airports");

                return View(ticket);
            }
            // 🟢 TẠO ID NGẪU NHIÊN TRƯỚC KHI LƯU
            ticket.Id = Guid.NewGuid().ToString();

            // 🟢 LƯU DỮ LIỆU LÊN FIRESTORE
            await _firestore.AddAsync("tickets", ticket);
            TempData["Success"] = "✅ Đã thêm vé mới thành công!";
            return RedirectToAction("Index");
        }
        // 🟡 Hiển thị form sửa
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "⚠️ Không xác định được vé cần sửa!";
                return RedirectToAction(nameof(Index));
            }

            var ticket = await _firestore.GetByIdAsync<Ticket>("tickets", id);
            if (ticket == null)
            {
                TempData["Error"] = "❌ Không tìm thấy vé cần sửa!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            ViewBag.Airports = await _firestore.GetAllAsync<Airport>("Airports");

            return View(ticket);
        }

        // 🟡 Xử lý cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
            {
                TempData["Error"] = "⚠️ Không tìm thấy ID vé để cập nhật!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _firestore.UpdateAsync("tickets", ticket.Id, ticket);
                TempData["Success"] = "✅ Cập nhật vé thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi khi cập nhật vé: {ex.Message}";
                ViewBag.Airlines = await _firestore.GetAllAsync<Airline>("Airlines");
                ViewBag.Airports = await _firestore.GetAllAsync<Airport>("Airports");
                return View(ticket);
            }
        }

        // 🔴 XÓA TRỰC TIẾP KHỎI FIRESTORE
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "⚠️ Không tìm thấy ID vé cần xóa!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _firestore.DeleteAsync("tickets", id);
                TempData["Success"] = "🗑️ Đã xóa vé khỏi Firestore!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi khi xóa vé: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
