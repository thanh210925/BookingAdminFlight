using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Airline
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Country { get; set; }

        [FirestoreProperty]
        public string Code { get; set; } // Ví dụ: VN, VJ, QH
    }
}
