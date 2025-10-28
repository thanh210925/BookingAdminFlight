using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class SeatController : Controller
    {
        private readonly FirestoreService _firestore;

        public SeatController()
        {
            _firestore = new FirestoreService();
        }

        // ✅ DANH SÁCH GHẾ
        public async Task<IActionResult> Index()
        {
            var seats = await _firestore.GetAllAsync<Seat>("Seats");
            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");

            // 🔹 Ghép tên hãng bay theo AirlineId
            foreach (var seat in seats)
            {
                var airline = airlines.FirstOrDefault(a => a.Id == seat.AirlineId);
                seat.AirlineName = airline != null ? airline.Name : "Không xác định";
            }

            return View(seats.OrderBy(s => s.SeatCode).ToList());
        }

        // 🟡 CHỈNH SỬA GHẾ
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var seat = await _firestore.GetByIdAsync<Seat>("Seats", id);
            if (seat == null) return NotFound();

            var airlines = await _firestore.GetAllAsync<Airline>("Airlines");
            ViewBag.Airlines = airlines;
            return View(seat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Seat seat)
        {
            await _firestore.UpdateAsync("Seats", seat.Id, seat);
            TempData["Success"] = "✏️ Đã cập nhật ghế thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🔴 XÓA GHẾ
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var seat = await _firestore.GetByIdAsync<Seat>("Seats", id);
            if (seat == null) return NotFound();
            return View(seat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestore.DeleteAsync("Seats", id);
            TempData["Success"] = "🗑️ Đã xóa ghế!";
            return RedirectToAction(nameof(Index));
        }
    }
}
