using Microsoft.AspNetCore.Mvc;
using BookingAdmin.Services;
using System.Threading.Tasks;

namespace BookingAdmin.Controllers
{
    public class BookingController : Controller
    {
        private readonly FirestoreService _firestore;

        public BookingController()
        {
            _firestore = new FirestoreService();
        }

        public async Task<IActionResult> Index()
        {
            var summary = await _firestore.GetFlightSummaryAsync();
            return View(summary);
        }
    }
}
