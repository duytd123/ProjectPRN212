using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;

namespace BusinessObjects
{
    public class ReportObjects
    {
        private readonly IReportRepository _reportRepository;
        private readonly ProjectPrn212Context _context;

        public ReportObjects()
        {
            _context = new ProjectPrn212Context();
            _reportRepository = new ReportRepository(_context);
        }

        public Task<bool> AddReport(Report report)
        {
            return _reportRepository.AddReport(report);
        }

        public IEnumerable<Report> GetReportsByUserIdAndFilters(int userId, DateOnly? fromDate, DateOnly? toDate, string? status, string? violationType, string? plateNumber)
        {
            return _reportRepository.GetReportsByUserIdAndFilters(userId, fromDate, toDate, status, violationType, plateNumber);
        }

        public string UploadFile(string filePath, string folder)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(filePath)}";
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", folder);
                Directory.CreateDirectory(destinationFolder);
                string destinationPath = Path.Combine(destinationFolder, fileName);

                File.Copy(filePath, destinationPath);
                return $"/Storage/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}");
            }
        }

        public bool ValidateFile(string filePath, string fileType, long maxSizeInBytes = 10485760) // Default 10MB
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            var fileInfo = new FileInfo(filePath);

            // Kiểm tra kích thước file
            if (fileInfo.Length > maxSizeInBytes)
                throw new Exception("File size exceeds maximum limit");

            // Kiểm tra loại file
            string extension = Path.GetExtension(filePath).ToLower();
            if (fileType.ToLower() == "image")
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                if (!Array.Exists(allowedExtensions, x => x == extension))
                    throw new Exception("Invalid image file type");
            }
            else if (fileType.ToLower() == "video")
            {
                string[] allowedExtensions = { ".mp4", ".avi", ".mov" };
                if (!Array.Exists(allowedExtensions, x => x == extension))
                    throw new Exception("Invalid video file type");
            }

            return true;
        }
    }
}
