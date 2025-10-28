using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Airline
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Code { get; set; } = string.Empty;

        [FirestoreProperty]
        public int SeatRows { get; set; }

        [FirestoreProperty]
        public int SeatCols { get; set; }

        [FirestoreProperty]
        public string Country { get; set; } = string.Empty;

    }
}
