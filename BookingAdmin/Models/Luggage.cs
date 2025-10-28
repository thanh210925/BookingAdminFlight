using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Luggage
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("weight")]
        public int Weight { get; set; }

        [FirestoreProperty("price")]
        public double Price { get; set; }

        [FirestoreProperty("description")]
        public string? Description { get; set; }
    }
}
