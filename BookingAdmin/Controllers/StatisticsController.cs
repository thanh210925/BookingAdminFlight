using Microsoft.AspNetCore.Mvc;
using BookingAdmin.Services;
using BookingAdmin.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace BookingAdmin.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly FirestoreService _firestore;

        public StatisticsController()
        {
            _firestore = new FirestoreService();
        }

        public async Task<IActionResult> Index()
        {
            // 🔹 Lấy dữ liệu vé thật từ Firestore
            var tickets = await _firestore.GetAllAsync<Ticket>("Tickets");
            Console.WriteLine($"📦 Tổng vé lấy được: {tickets.Count}");

            foreach (var t in tickets)
            {
                Console.WriteLine($"🛫 Vé: Airline={t.Airline}, DepartureDate={t.DepartureDate}, Price={t.Price}");
            }

            // 🔹 Lọc vé hợp lệ
            var validTickets = tickets
                .Where(t => !string.IsNullOrEmpty(t.DepartureDate) && t.Price > 0)
                .ToList();

            Console.WriteLine($"✅ Vé hợp lệ: {validTickets.Count}");

            // 🔹 Gom theo tháng (tự động nhận dạng DD-MM-YYYY hoặc YYYY-MM-DD)
            var monthlyStats = validTickets
                .GroupBy(t =>
                {
                    try
                    {
                        string raw = t.DepartureDate.Replace("/", "-").Trim();
                        DateTime parsed;

                        // Thử 2 định dạng phổ biến
                        if (DateTime.TryParseExact(raw, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed) ||
                            DateTime.TryParseExact(raw, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                        {
                            return parsed.Month;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Không đọc được ngày: {raw}");
                            return 0;
                        }
                    }
                    catch
                    {
                        return 0;
                    }
                })
                .Where(g => g.Key > 0)
                .Select(g => new
                {
                    Month = g.Key,
                    TicketCount = g.Count(),
                    TotalRevenue = g.Sum(t => t.Price)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // 🔹 Gom theo hãng bay
            var airlineStats = validTickets
                .GroupBy(t => t.Airline)
                .Select(g => new
                {
                    Airline = g.Key,
                    TicketCount = g.Count(),
                    TotalRevenue = g.Sum(t => t.Price)
                })
                .OrderByDescending(x => x.TicketCount)
                .ToList();

            // 🔹 Gửi sang View
            ViewBag.Months = monthlyStats.Select(x => $"Tháng {x.Month}").ToList();
            ViewBag.TicketCounts = monthlyStats.Select(x => x.TicketCount).ToList();
            ViewBag.Revenues = monthlyStats.Select(x => (double)x.TotalRevenue / 1_000_000).ToList();

            ViewBag.AirlineNames = airlineStats.Select(x => x.Airline).ToList();
            ViewBag.AirlineTickets = airlineStats.Select(x => x.TicketCount).ToList();
            ViewBag.AirlineRevenue = airlineStats.Select(x => (double)x.TotalRevenue / 1_000_000).ToList();

            // 🧠 Log xác nhận
            Console.WriteLine("🎯 Dữ liệu thống kê:");
            foreach (var m in monthlyStats)
                Console.WriteLine($"   ➜ Tháng {m.Month}: {m.TicketCount} vé, {m.TotalRevenue}₫");

            return View();
        }
    }
}
