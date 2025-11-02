using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookingAdmin.Models
//{
//    [FirestoreData]
//    public class Ticket
//    {
//        [FirestoreDocumentId]
//        [BindNever]
//        public string Id { get; set; }

//        [FirestoreProperty("airline")]
//        public string Airline { get; set; }

//        [FirestoreProperty("arrivalAirport")]
//        public string ArrivalAirport { get; set; }

//        [FirestoreProperty("departureAirport")]
//        public string DepartureAirport { get; set; }

//        // 🔹 Thống nhất với Android
//        [FirestoreProperty("departureDate")]
//        public string DepartureDate { get; set; }

//        [FirestoreProperty("departureTime")]
//        public string DepartureTime { get; set; }

//        [FirestoreProperty("arrivalTime")]
//        public string ArrivalTime { get; set; }

//        [FirestoreProperty("price")]
//        public long Price { get; set; }

//        [FirestoreProperty("returnDate")]
//        public string ReturnDate { get; set; }

//        [FirestoreProperty("seatClass")]
//        public string SeatClass { get; set; }
//        [FirestoreProperty("returnTime")]
//        public string ReturnTime { get; set; }

//        [FirestoreProperty("flightType")]
//        public string FlightType { get; set; }

//    }
//}
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty("airline")]
        public string? Airline { get; set; }

        [FirestoreProperty("arrivalAirport")]
        public string? ArrivalAirport { get; set; }

        [FirestoreProperty("departureAirport")]
        public string? DepartureAirport { get; set; }

        [FirestoreProperty("departureDate")]
        public string? DepartureDate { get; set; }

        [FirestoreProperty("departureTime")]
        public string? DepartureTime { get; set; }

        [FirestoreProperty("arrivalTime")]
        public string? ArrivalTime { get; set; }  // 🔸 Cho phép null

        [FirestoreProperty("price")]
        public long Price { get; set; }

        [FirestoreProperty("returnDate")]
        public string? ReturnDate { get; set; }   // 🔸 Cho phép null

        [FirestoreProperty("seatClass")]
        public string? SeatClass { get; set; }

        [FirestoreProperty("returnTime")]
        public string? ReturnTime { get; set; }   // 🔸 Cho phép null

        [FirestoreProperty("flightType")]
        public string? FlightType { get; set; }
    }
}
