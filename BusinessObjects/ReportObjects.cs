    using DataAccess.Models;
    using DataAccess.Repository;
    using DataAccess.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

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

            public User GetVehicleOwnerByPlateNumber(string plateNumber)
            {
                var vehicle = _context.Vehicles.Include(v => v.Owner).FirstOrDefault(v => v.PlateNumber == plateNumber);
                return vehicle?.Owner;
            }

        public string UploadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(filePath)}";
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                string policeFolder = Path.Combine(Path.GetDirectoryName(projectRoot), "Police");

                Directory.CreateDirectory(policeFolder);

                string destinationPath = Path.Combine(policeFolder, fileName);

                File.Copy(filePath, destinationPath);

                return $"Police/{fileName}";
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
                    string executablePath = AppDomain.CurrentDomain.BaseDirectory;
                    string projectDirectory = Path.GetFullPath(Path.Combine(executablePath, @"..\..\.."));
                    string relativePath = fileUrl.TrimStart('/');
                    string filePath = Path.Combine(projectDirectory, relativePath);

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
                if (string.IsNullOrEmpty(filePath))
                    return false;

                string executablePath = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Path.GetFullPath(Path.Combine(executablePath, @"..\..\.."));
                string relativePath = filePath.TrimStart('/');
                string absoluteFilePath = Path.Combine(projectDirectory, relativePath);

                if (!File.Exists(absoluteFilePath))
                    return false;

                var fileInfo = new FileInfo(absoluteFilePath);
                if (fileInfo.Length > maxSizeInBytes)
                    throw new Exception("File size exceeds maximum limit");

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
