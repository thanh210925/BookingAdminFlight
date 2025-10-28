using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Airport
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Code { get; set; } // Ví dụ: SGN, HAN, DAD

        [FirestoreProperty]
        public string City { get; set; }

        [FirestoreProperty]
        public string Country { get; set; }
    }
}
