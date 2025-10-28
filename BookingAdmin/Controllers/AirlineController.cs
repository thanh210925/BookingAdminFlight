using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BookingAdmin.Controllers
{
    public class AirlineController : Controller
    {
        private readonly FirestoreService _firestore;

        public AirlineController()
        {
            _firestore = new FirestoreService();
        }

        // ✅ DANH SÁCH HÃNG BAY
        public async Task<IActionResult> Index()
        {
            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            return View(airlines);
        }

        // 🟢 FORM THÊM HÃNG BAY
        [HttpGet]
        public IActionResult Create() => View();

        // 🟢 XỬ LÝ THÊM HÃNG BAY + TỰ SINH GHẾ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Airline airline)
        {
            // B1️⃣: Thêm hãng bay vào Firestore
            var newAirlineId = await _firestore.AddAsync("Airlines", airline);

            // B2️⃣: Sinh ghế dựa theo cấu hình (SeatRows × SeatCols)
            await GenerateSeatsAsync(newAirlineId, airline.SeatRows, airline.SeatCols);

            TempData["Success"] = $"✈️ Đã thêm hãng {airline.Name} và tạo {airline.SeatRows * airline.SeatCols} ghế!";
            return RedirectToAction(nameof(Index));
        }

        // 🔹 HÀM SINH GHẾ TỰ ĐỘNG (theo hạng ghế)
        // 🔹 Sinh ghế chính xác theo hạng: Business (1–10), First Class (11–15), Economy (16+)
        private async Task GenerateSeatsAsync(string airlineId, int rows, int cols)
        {
            var seats = new List<Seat>();

            for (int r = 1; r <= rows; r++)
            {
                string seatClass;

                // Xác định hạng ghế chính xác
                if (r >= 1 && r <= 10)
                    seatClass = "Business";
                else if (r >= 11 && r <= 15)
                    seatClass = "First Class";
                else
                    seatClass = "Economy";

                for (int c = 1; c <= cols; c++)
                {
                    // Mã ghế: 1A, 1B, ...
                    string seatCode = $"{r}{(char)('A' + c - 1)}";

                    var seat = new Seat
                    {
                        AirlineId = airlineId,
                        SeatCode = seatCode,
                        Class = seatClass,
                        Status = "Trống"
                    };

                    seats.Add(seat);
                }
            }

            // ✅ Lưu toàn bộ ghế vào Firestore theo thứ tự
            foreach (var seat in seats)
            {
                await _firestore.AddAsync("Seats", seat);
            }

            Console.WriteLine($"✅ Đã sinh {seats.Count} ghế cho hãng {airlineId}");
        }


        // 🟡 FORM SỬA HÃNG BAY
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

        // 🔴 FORM XÓA HÃNG BAY
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _firestore.GetByIdAsync<Airline>("Airlines", id);
            if (item == null) return NotFound();
            return View(item);
        }

        // 🔴 XỬ LÝ XÓA HÃNG BAY + GHẾ
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Xóa tất cả ghế thuộc hãng bay này
            var seats = await _firestore.GetAllAsync<Seat>("Seats");
            var relatedSeats = seats.Where(s => s.AirlineId == id).ToList();

            foreach (var seat in relatedSeats)
            {
                await _firestore.DeleteAsync("Seats", seat.Id);
            }

            // Sau đó xóa hãng bay
            await _firestore.DeleteAsync("Airlines", id);

            TempData["Success"] = $"🗑️ Đã xóa hãng bay và {relatedSeats.Count} ghế liên quan!";
            return RedirectToAction(nameof(Index));
        }
    }
}
