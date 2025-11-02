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
            var tickets = await _firestore.GetAllAsync<Ticket>("tickets");
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
            //var monthlyStats = validTickets
            //    .GroupBy(t =>
            //    {
            //        try
            //        {
            //            string raw = t.DepartureDate?.Replace("/", "-").Trim();
            //            DateTime parsed;

            //            // ✅ Dùng TryParseExact một lần với nhiều định dạng
            //            if (DateTime.TryParseExact(
            //                raw,
            //                new[] { "yyyy-MM-dd", "dd-MM-yyyy" },
            //                CultureInfo.InvariantCulture,
            //                DateTimeStyles.None,
            //                out parsed))
            //            {
            //                return parsed.Month;
            //            }
            //            else
            //            {
            //                Console.WriteLine($"⚠️ Không đọc được ngày: {raw}");
            //                return 0;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine($"❌ Lỗi parse ngày: {ex.Message}");
            //            return 0;
            //        }

            //    })
            //    .Where(g => g.Key > 0)
            //    .Select(g => new
            //    {
            //        Month = g.Key,
            //        TicketCount = g.Count(),
            //        TotalRevenue = g.Sum(t => t.Price)
            //    })
            //    .OrderBy(x => x.Month)
            //    .ToList();

            // 🔹 Gom theo tháng như cũ
            var groupedByMonth = validTickets
                .GroupBy(t =>
                {
                    try
                    {
                        string raw = t.DepartureDate?.Replace("/", "-").Trim();
                        DateTime parsed;

                        if (DateTime.TryParseExact(
                            raw,
                            new[] { "yyyy-MM-dd", "dd-MM-yyyy" },
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out parsed))
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
                .ToDictionary(g => g.Key, g => new
                {
                    TicketCount = g.Count(),
                    TotalRevenue = g.Sum(t => t.Price)
                });

            // ✅ Tạo danh sách đủ 12 tháng
            var monthlyStats = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = m,
                    TicketCount = groupedByMonth.ContainsKey(m) ? groupedByMonth[m].TicketCount : 0,
                    TotalRevenue = groupedByMonth.ContainsKey(m) ? groupedByMonth[m].TotalRevenue : 0
                })
                .ToList();

            // 🔹 Gom theo hãng bay
            //var airlineStats = validTickets
            //    .GroupBy(t => t.Airline)
            //    .Select(g => new
            //    {
            //        Airline = g.Key,
            //        TicketCount = g.Count(),
            //        TotalRevenue = g.Sum(t => t.Price)
            //    })
            //    .OrderByDescending(x => x.TicketCount)
            //    .ToList();
            var airlineStats = validTickets
            .GroupBy(t => t.Airline)
            .Select(g => new
            {
                Airline = g.Key,
                TicketCount = g.Count(),
                TotalRevenue = g.Sum(t => Convert.ToDecimal(t.Price)) // ✅ tránh tràn số
            })
            .OrderByDescending(x => x.TicketCount)
            .ToList();


            // 🔹 Gửi sang View
            ViewBag.Months = monthlyStats.Select(x => $"Tháng {x.Month}").ToList();
            ViewBag.TicketCounts = monthlyStats.Select(x => x.TicketCount).ToList();
            ViewBag.Revenues = monthlyStats
                .Select(x => Math.Round((double)x.TotalRevenue / 1_000_000, 2)) // triệu ₫
                .ToList();

            ViewBag.AirlineNames = airlineStats.Select(x => x.Airline).ToList();
            ViewBag.AirlineTickets = airlineStats.Select(x => x.TicketCount).ToList();
            ViewBag.AirlineRevenue = airlineStats
                .Select(x => Math.Round((double)x.TotalRevenue / 1_000_000, 2)) // triệu ₫
                .ToList();

            // 🧠 Log xác nhận
            Console.WriteLine("🎯 Dữ liệu thống kê:");
            foreach (var m in monthlyStats)
                Console.WriteLine($"   ➜ Tháng {m.Month}: {m.TicketCount} vé, {m.TotalRevenue}₫");

            return View();
        }
    }
}
