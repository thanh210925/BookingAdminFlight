using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("airline")]
        public string Airline { get; set; }

        [FirestoreProperty("arrivalAirport")]
        public string ArrivalAirport { get; set; }

        [FirestoreProperty("departureAirport")]
        public string DepartureAirport { get; set; }

        [FirestoreProperty("departure date")]
        public string DepartureDate { get; set; }

        [FirestoreProperty("departure time")]
        public string DepartureTime { get; set; }

        [FirestoreProperty("arrival time")]
        public string ArrivalTime { get; set; }

        [FirestoreProperty("price")]
        public long Price { get; set; }

        [FirestoreProperty("returnDate")]
        public string ReturnDate { get; set; }

        [FirestoreProperty("seatClass")]
        public string SeatClass { get; set; }
    }
}
