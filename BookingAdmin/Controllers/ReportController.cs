using BookingAdmin.Models;
using BookingAdmin.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookingAdmin.Controllers
{
    public class ReportController : Controller
    {
        private readonly FirestoreService _firestore;

        public ReportController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        // ================================================================
        // BUILD REPORT (TÍNH THEO BOOKING – KHÔNG DOUBLE DOANH THU)
        // ================================================================
        private async Task<ReportViewModel> BuildReportAsync()
        {
            // Lấy tất cả ticket
            var tickets = await _firestore.GetAllAsync<TicketOrder>("Tickets");

            // Lọc vé đã xác nhận
            tickets = tickets
                .Where(t => t.Status == "Đã xác nhận")
                .ToList();

            var vm = new ReportViewModel();

            // ===== GOM VÉ THEO BOOKING =====
            var bookings = tickets
                .GroupBy(t => t.BookingId)
                .Select(g => g.ToList())
                .ToList();

            // ===== TỔNG VÉ =====
            vm.TotalTickets = tickets.Count;

            // ===== DOANH THU THEO BOOKING =====
            vm.TotalRevenue = bookings.Sum(g => g.First().TotalPrice);

            // ===== HÃNG BAY BÁN NHIỀU NHẤT =====
            var topAirline = tickets
                .GroupBy(t => t.Airline)
                .Select(g => new { Airline = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            vm.TopAirline = topAirline?.Airline ?? "Không có";
            vm.TopAirlineCount = topAirline?.Count ?? 0;

            // ===== SÂN BAY =====
            vm.ActiveAirports = tickets
                .Select(t => t.Departure)
                .Distinct()
                .Count();

            // ===== HÀNH LÝ =====
            vm.TicketsHasLuggage = tickets.Count(t =>
                !string.IsNullOrWhiteSpace(t.Luggage) ||
                t.LuggagePrice > 0);

            vm.TicketsNoLuggage = tickets.Count - vm.TicketsHasLuggage;
            vm.LuggageRevenue = tickets.Sum(t => (long)t.LuggagePrice);

            // ===== 1 CHIỀU / KHỨ HỒI =====
            foreach (var g in bookings)
            {
                var routes = g
                    .Select(x => new { x.Departure, x.Destination })
                    .ToList();

                long revenue = g.First().TotalPrice;

                bool hasOut = routes.Any(r => routes.Any(
                    rr => rr.Departure == r.Destination && rr.Destination == r.Departure));

                if (hasOut && routes.Count >= 2)
                {
                    vm.RoundTripBookings++;
                    vm.RoundTripRevenue += revenue;
                }
                else
                {
                    vm.OneWayBookings++;
                    vm.OneWayRevenue += revenue;
                }
            }

            // ===== DOANH THU THEO SÂN BAY =====
            var airportRevenue = bookings
                .Select(b => b.First())
                .GroupBy(t => t.Departure)
                .Select(g => new
                {
                    Airport = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            vm.AirportLabels = airportRevenue.Select(x => x.Airport).ToList();
            vm.AirportRevenue = airportRevenue.Select(x => x.Revenue).ToList();

            // ===== DOANH THU THEO CHẶNG BAY =====
            var routeRevenue = bookings
                .Select(b => b.First())
                .GroupBy(t => $"{t.Departure} → {t.Destination}")
                .Select(g => new
                {
                    Route = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            vm.RouteLabels = routeRevenue.Select(x => x.Route).ToList();
            vm.RouteRevenue = routeRevenue.Select(x => x.Revenue).ToList();

            return vm;
        }

        // ================================================================
        // TRANG THỐNG KÊ
        // ================================================================
        public async Task<IActionResult> Index()
        {
            var model = await BuildReportAsync();
            return View(model);
        }

        // ================================================================
        // EXPORT PDF
        // ================================================================
        public async Task<IActionResult> ExportPdf()
        {
            var model = await BuildReportAsync();

            using var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 36, 36, 48, 48);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            var culture = CultureInfo.GetCultureInfo("vi-VN");

            // ===== Title =====
            var title = new Paragraph("BÁO CÁO THỐNG KÊ VÉ MÁY BAY\n\n", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // ===== Tổng quan =====
            doc.Add(new Paragraph("1. THÔNG TIN TỔNG QUAN", headerFont));
            doc.Add(new Paragraph($"- Tổng vé bán: {model.TotalTickets}", normalFont));
            doc.Add(new Paragraph($"- Tổng doanh thu: {model.TotalRevenue.ToString("c0", culture)}", normalFont));
            doc.Add(new Paragraph($"- Hãng bay bán nhiều nhất: {model.TopAirline} ({model.TopAirlineCount} vé)", normalFont));
            doc.Add(new Paragraph($"- Sân bay đang hoạt động: {model.ActiveAirports}\n", normalFont));

            // ===== Hành lý =====
            doc.Add(new Paragraph("2. THỐNG KÊ HÀNH LÝ", headerFont));
            doc.Add(new Paragraph($"- Vé có hành lý: {model.TicketsHasLuggage}", normalFont));
            doc.Add(new Paragraph($"- Vé không hành lý: {model.TicketsNoLuggage}", normalFont));
            doc.Add(new Paragraph($"- Doanh thu hành lý: {model.LuggageRevenue.ToString("c0", culture)}\n", normalFont));

            // ===== Một chiều / Khứ hồi =====
            doc.Add(new Paragraph("3. THỐNG KÊ 1 CHIỀU / KHỨ HỒI", headerFont));
            doc.Add(new Paragraph($"- Đơn một chiều: {model.OneWayBookings} – Doanh thu: {model.OneWayRevenue.ToString("c0", culture)}", normalFont));
            doc.Add(new Paragraph($"- Đơn khứ hồi: {model.RoundTripBookings} – Doanh thu: {model.RoundTripRevenue.ToString("c0", culture)}\n", normalFont));

            // ===== Sân bay =====
            doc.Add(new Paragraph("4. DOANH THU THEO SÂN BAY", headerFont));
            var tb1 = new PdfPTable(2) { WidthPercentage = 100 };
            tb1.AddCell("Sân bay");
            tb1.AddCell("Doanh thu");
            for (int i = 0; i < model.AirportLabels.Count; i++)
            {
                tb1.AddCell(model.AirportLabels[i]);
                tb1.AddCell(model.AirportRevenue[i].ToString("c0", culture));
            }
            doc.Add(tb1);
            doc.Add(new Paragraph("\n"));

            // ===== Chặng bay =====
            doc.Add(new Paragraph("5. DOANH THU THEO CHẶNG BAY", headerFont));
            var tb2 = new PdfPTable(2) { WidthPercentage = 100 };
            tb2.AddCell("Chặng bay");
            tb2.AddCell("Doanh thu");
            for (int i = 0; i < model.RouteLabels.Count; i++)
            {
                tb2.AddCell(model.RouteLabels[i]);
                tb2.AddCell(model.RouteRevenue[i].ToString("c0", culture));
            }
            doc.Add(tb2);

            doc.Close();
            return File(ms.ToArray(), "application/pdf", "BaoCaoThongKe.pdf");
        }
    }
}
