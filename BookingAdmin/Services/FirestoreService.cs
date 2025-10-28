using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BookingAdmin.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _db;

        // ============================================================
        // 🔹 KHỞI TẠO KẾT NỐI FIRESTORE
        // ============================================================
        public FirestoreService()
        {
            // 🔹 Tên file key Firebase (bé giữ nguyên file này)
            string keyFileName = "device-streaming-3412c6be-firebase-adminsdk-gwxlc-439805a46b.json";

            // 🔹 Đường dẫn tuyệt đối đến file key (trong bin/Debug/net8.0)
            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, keyFileName);

            // 🔹 Kiểm tra file có tồn tại không
            if (!File.Exists(credentialPath))
                throw new FileNotFoundException("❌ Không tìm thấy file Firebase key tại: " + credentialPath);

            // 🔹 Gán biến môi trường cho SDK Firebase
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            // 🔹 Kết nối đến Firestore (Project ID)
            _db = FirestoreDb.Create("device-streaming-3412c6be");

            Console.WriteLine("✅ Kết nối Firestore thành công!");
        }

        // ============================================================
        // 🟢 THÊM DOCUMENT MỚI (có ID tự động) - TRẢ VỀ ID
        // ============================================================
        public async Task<string> AddAsync<T>(string collectionName, T data)
        {
            // 🔹 Tạo document mới (Firestore tự sinh ID)
            var docRef = _db.Collection(collectionName).Document();

            // 🔹 Nếu model có property Id → gán ID đó vào model
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
                idProp.SetValue(data, docRef.Id);

            // 🔹 Lưu dữ liệu vào Firestore
            await docRef.SetAsync(data);
            Console.WriteLine($"✅ Đã thêm document vào '{collectionName}' với ID: {docRef.Id}");

            // 🔹 Trả về ID document vừa tạo
            return docRef.Id;
        }

        // ============================================================
        // 🟢 LẤY TOÀN BỘ DOCUMENT
        // ============================================================
        public async Task<List<T>> GetAllAsync<T>(string collectionName)
        {
            var snapshot = await _db.Collection(collectionName).GetSnapshotAsync();
            var list = new List<T>();

            foreach (var doc in snapshot.Documents)
            {
                var obj = doc.ConvertTo<T>();
                var idProp = typeof(T).GetProperty("Id");

                if (idProp != null)
                    idProp.SetValue(obj, doc.Id);

                list.Add(obj);
            }

            return list;
        }

        // ============================================================
        // 🟢 LẤY DOCUMENT THEO ID
        // ============================================================
        public async Task<T> GetByIdAsync<T>(string collectionName, string id)
        {
            var docRef = _db.Collection(collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return default;

            var obj = snapshot.ConvertTo<T>();
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
                idProp.SetValue(obj, snapshot.Id);

            return obj;
        }

        // ============================================================
        // 🟢 CẬP NHẬT DOCUMENT
        // ============================================================
        public async Task UpdateAsync<T>(string collectionName, string id, T data)
        {
            await _db.Collection(collectionName).Document(id).SetAsync(data, SetOptions.Overwrite);
            Console.WriteLine($"✏️ Đã cập nhật document '{collectionName}' có ID: {id}");
        }

        // ============================================================
        // 🟢 XÓA DOCUMENT
        // ============================================================
        public async Task DeleteAsync(string collectionName, string id)
        {
            await _db.Collection(collectionName).Document(id).DeleteAsync();
            Console.WriteLine($"🗑️ Đã xóa document '{collectionName}' có ID: {id}");
        }

        // ============================================================
        // 🟢 LẤY THỐNG KÊ CHUYẾN BAY (tùy chọn)
        // ============================================================
        public async Task<Dictionary<string, object>> GetFlightSummaryAsync()
        {
            var docRef = _db.Collection("FlightStatistics").Document("Summary");
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                throw new Exception("❌ Không tìm thấy document 'Summary' trong Firestore!");

            return snapshot.ToDictionary();
        }
    }
}
