using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("uid")]
        public string Uid { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("e-mail")]
        public string Email { get; set; }

        [FirestoreProperty("phone")]
        public string Phone { get; set; }

        [FirestoreProperty("birthday")]
        public string Birthday { get; set; }

        [FirestoreProperty("gender")]
        public string Gender { get; set; }

        [FirestoreProperty("role")]
        public string Role { get; set; }
    }
}
