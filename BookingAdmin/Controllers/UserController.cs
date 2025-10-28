using BookingAdmin.Models;
using BookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class UserController : Controller
    {
        private readonly FirestoreService _firestore;

        public UserController()
        {
            _firestore = new FirestoreService();
        }

        // 🔹 Danh sách user
        public async Task<IActionResult> Index()
        {
            var users = await _firestore.GetAllAsync<User>("users");
            return View(users);
        }

        // 🔹 Chi tiết user
        public async Task<IActionResult> Details(string id)
        {
            var user = await _firestore.GetByIdAsync<User>("users", id);
            if (user == null)
            {
                TempData["Error"] = "❌ Không tìm thấy người dùng!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // 🔹 Xóa user
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _firestore.GetByIdAsync<User>("users", id);
            if (user == null)
            {
                TempData["Error"] = "❌ Không tìm thấy người dùng cần xóa!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _firestore.DeleteAsync("users", id);
                TempData["Success"] = "🗑️ Đã xóa người dùng khỏi Firestore!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"⚠️ Lỗi khi xóa: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
