using Google.Cloud.Firestore;

namespace BookingAdmin.Models
{
    [FirestoreData]
    public class Seat
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string SeatCode { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Class { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Status { get; set; } = "Trống";

        [FirestoreProperty]
        public string AirlineId { get; set; } = string.Empty;

        // 🔹 Không lưu Firestore, chỉ hiển thị
        public string AirlineName { get; set; } = string.Empty;
    }
}
