using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class TicketOrder
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("fullName")]
        public string FullName { get; set; }

        [FirestoreProperty("email")]
        public string Email { get; set; }

        [FirestoreProperty("phone")]
        public string Phone { get; set; }

        [FirestoreProperty("cmnd")]
        public string Cmnd { get; set; }

        [FirestoreProperty("airline")]
        public string Airline { get; set; }

        [FirestoreProperty("departure")]
        public string Departure { get; set; }

        [FirestoreProperty("destination")]
        public string Destination { get; set; }

        [FirestoreProperty("departure date")]
        public string DepartureDate { get; set; }

        [FirestoreProperty("returnDate")]
        public string ReturnDate { get; set; }

        [FirestoreProperty("departureSeats")]
        public List<string> DepartureSeats { get; set; }

        [FirestoreProperty("returnSeats")]
        public List<string> ReturnSeats { get; set; }

        [FirestoreProperty("seatClass")]
        public string SeatClass { get; set; }

        [FirestoreProperty("ticketPrice")]
        public long TicketPrice { get; set; }

        [FirestoreProperty("totalPrice")]
        public long TotalPrice { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("paymentImageUrl")]
        public string PaymentImageUrl { get; set; }

        [FirestoreProperty("createdAt")]
        public Timestamp CreatedAt { get; set; }
    }
}
