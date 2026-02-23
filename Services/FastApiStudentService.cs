using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Week3_L1.Models;
using Week3_L1.Services;

public class FastApiStudentService : IStudentServices
{
    private readonly HttpClient _http;

    // Dictionary<StudentId, Student> — fast lookup by id
    private readonly Dictionary<int, Student> _studentCache = new();

    public event EventHandler<StudentChangedEventArgs>? StudentChanged;

    public FastApiStudentService(HttpClient http)
    {
        _http = http;
    }

    // ---- GET ALL — fetch once, cache in dictionary ----
    public async Task<List<Student>> GetAllStudentsAsync()
    {
        // If dictionary already has data — return local copy instantly
        if (_studentCache.Any())
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Using cache ({_studentCache.Count} students)");
            return _studentCache.Values.ToList();
        }

        try
        {
            System.Diagnostics.Debug.WriteLine("[SERVICE] First load - fetching from API...");

            var students = await _http.GetFromJsonAsync<List<Student>>("api/v1/students/");

            _studentCache.Clear();
            foreach (var student in students ?? new List<Student>())
            {
                _studentCache[student.StudentId] = student;
            }

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Loaded {_studentCache.Count} students");
            return _studentCache.Values.ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] GetAllStudentsAsync: {ex.Message}");
            return new List<Student>();
        }
    }

    // ---- SEARCH — local, instant, no network call ----
    public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
    {
        System.Diagnostics.Debug.WriteLine($"[SERVICE] Searching: '{searchTerm}'");

        var allStudents = await GetAllStudentsAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
            return allStudents;

        var query = searchTerm.Trim().ToLower();

        var results = allStudents
            .Where(s => s.StudentName.ToLower().Contains(query))
            .OrderBy(s => s.StudentName)
            .ToList();

        System.Diagnostics.Debug.WriteLine($"[SERVICE] Found {results.Count} students");
        return results;
    }

    // ---- CREATE — POST to API, add to cache ----
    public async Task<Student> CreateStudentAsync(string studentName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty");

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Creating: {studentName}");

            var payload = new { name = studentName.Trim() };
            var response = await _http.PostAsJsonAsync("api/v1/students/", payload);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<Student>();

            if (created != null)
            {
                // Add to dictionary cache
                _studentCache[created.StudentId] = created;

                StudentChanged?.Invoke(this, new StudentChangedEventArgs
                {
                    Student = created,
                    ChangeType = ChangeType.Added
                });

                System.Diagnostics.Debug.WriteLine($"[SERVICE] Created student {created.StudentId}");
            }

            return created!;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] CreateStudentAsync: {ex.Message}");
            throw;
        }
    }

    // ---- UPDATE — PUT to API, update cache ----
    public async Task<bool> UpdateStudentAsync(int studentId, string studentName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty");

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Updating student {studentId}");

            // Check cache first — if not there load from API
            if (!_studentCache.ContainsKey(studentId))
            {
                await GetAllStudentsAsync();
                if (!_studentCache.ContainsKey(studentId))
                    return false;
            }

            var payload = new { name = studentName.Trim() };
            var response = await _http.PutAsJsonAsync($"api/v1/students/{studentId}", payload);
            response.EnsureSuccessStatusCode();

            // Update in cache directly — no need to re-fetch
            _studentCache[studentId].StudentName = studentName.Trim();

            StudentChanged?.Invoke(this, new StudentChangedEventArgs
            {
                Student = _studentCache[studentId],
                ChangeType = ChangeType.Updated
            });

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Updated student {studentId}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] UpdateStudentAsync: {ex.Message}");
            return false;
        }
    }

    // ---- COUNT — local, instant ----
    public async Task<int> GetTotalCountAsync()
    {
        var students = await GetAllStudentsAsync();
        return students.Count;
    }
}