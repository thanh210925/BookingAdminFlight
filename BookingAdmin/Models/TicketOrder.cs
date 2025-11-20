using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class TicketOrder
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "";

        [FirestoreProperty("airline")]
        public string Airline { get; set; } = "";

        [FirestoreProperty("bookingId")]
        public string BookingId { get; set; } = "";

        [FirestoreProperty("cmnd")]
        public string Cmnd { get; set; } = "";

        [FirestoreProperty("createdAt")]
        public DateTime CreatedAt { get; set; }




        [FirestoreProperty("date")]
        public string Date { get; set; } = "";

        [FirestoreProperty("departure")]
        public string Departure { get; set; } = "";

        [FirestoreProperty("destination")]
        public string Destination { get; set; } = "";

        [FirestoreProperty("dob")]
        public string Dob { get; set; } = "";

        [FirestoreProperty("e-mail")]
        public string Email { get; set; } = "";

        [FirestoreProperty("gender")]
        public string Gender { get; set; } = "";

        [FirestoreProperty("luggage")]
        public string Luggage { get; set; } = "";

        [FirestoreProperty("luggagePrice")]
        public int LuggagePrice { get; set; }

        [FirestoreProperty("nationality")]
        public string Nationality { get; set; } = "";

        [FirestoreProperty("passengerName")]
        public string PassengerName { get; set; } = "";

        [FirestoreProperty("paymentImageUrl")]
        public string PaymentImageUrl { get; set; } = "";

        [FirestoreProperty("phone")]
        public string Phone { get; set; } = "";

        [FirestoreProperty("price")]
        public long Price { get; set; }

        [FirestoreProperty("seat")]
        public string Seat { get; set; } = "";

        [FirestoreProperty("seatClass")]
        public string SeatClass { get; set; } = "";

        [FirestoreProperty("status")]
        public string Status { get; set; } = "";

        [FirestoreProperty("time")]
        public string Time { get; set; } = "";

        [FirestoreProperty("totalPrice")]
        public long TotalPrice { get; set; }

        [FirestoreProperty("kind")]
        public string Kind { get; set; } = "";

        [FirestoreProperty("userId")]
        public string UserId { get; set; } = "";
        [FirestoreProperty("address")]
        public string Address { get; set; } = "";

    }
}
