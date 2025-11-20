using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace BookingAdmin.Models
{
    public class ReportViewModel
    {
      

        // ===== Tổng quan =====
        public int TotalTickets { get; set; }
        public long TotalRevenue { get; set; }

        public string TopAirline { get; set; } = "";
        public int TopAirlineCount { get; set; }
        public int ActiveAirports { get; set; }

        // ===== Hành lý =====
        public int TicketsHasLuggage { get; set; }
        public int TicketsNoLuggage { get; set; }
        public long LuggageRevenue { get; set; }

        // ===== Một chiều / Khứ hồi (theo Booking) =====
        public int OneWayBookings { get; set; }
        public int RoundTripBookings { get; set; }
        public long OneWayRevenue { get; set; }
        public long RoundTripRevenue { get; set; }

        // ===== Chart: Doanh thu theo sân bay =====
        public List<string> AirportLabels { get; set; } = new();
        public List<long> AirportRevenue { get; set; } = new();

        // ===== Chart: Doanh thu theo chặng bay =====
        public List<string> RouteLabels { get; set; } = new();
        public List<long> RouteRevenue { get; set; } = new();
    }
}
