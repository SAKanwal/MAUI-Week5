// File: Services/FirebaseStudentService.cs

using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Week3_L1.Models;

namespace Week3_L1.Services
{
    public class FirebaseStudentService : IStudentServices
    {
        private readonly FirebaseClient _firebaseClient;

        //  Use existing dictionary - no extra bool needed!
        private readonly Dictionary<int, string> _firebaseKeyAndId;

        //  Local list cache
        private readonly List<Student> _localStudents;

        private const string STUDENTS_NODE = "Students";

        public event EventHandler<StudentChangedEventArgs>? StudentChanged;

        public FirebaseStudentService(FirebaseClient firebaseClient)
        {
            _firebaseClient = firebaseClient ?? throw new ArgumentNullException(nameof(firebaseClient));
            _firebaseKeyAndId = new Dictionary<int, string>();
            _localStudents = new List<Student>();
        }

        // ==========================================
        // GET ALL STUDENTS
        // ==========================================

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            //  If dictionary has data = already loaded!
            if (_firebaseKeyAndId.Any())
            {
                System.Diagnostics.Debug.WriteLine($"[SERVICE] Using local list ({_localStudents.Count} students)");
                return _localStudents.ToList();
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[SERVICE] First load - Fetching from Firebase...");

                var firebaseStudents = await _firebaseClient
                    .Child(STUDENTS_NODE)
                    .OnceAsync<Student>();

                _localStudents.Clear();
                _firebaseKeyAndId.Clear();

                foreach (var item in firebaseStudents)
                {
                    if (item.Object != null)
                    {
                        var student = item.Object;

                        // Store both: Firebase key AND student data
                        _firebaseKeyAndId[student.StudentId] = item.Key;
                        _localStudents.Add(student);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[SERVICE] ✅ Loaded {_localStudents.Count} students");

                return _localStudents.OrderBy(s => s.StudentId).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // SEARCH (Local only - INSTANT!)
        // ==========================================

        public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE] 🔍 Searching: '{searchTerm}'");

            // Get from local list (fetches from Firebase only if empty)
            var allStudents = await GetAllStudentsAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return allStudents;
            }

            var query = searchTerm.Trim().ToLower();

            var results = allStudents
                .Where(s => s.StudentName.ToLower().Contains(query))
                .OrderBy(s => s.StudentName)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Found {results.Count} students");

            return results;
        }

        // ==========================================
        // CREATE STUDENT
        // ==========================================

        public async Task<Student> CreateStudentAsync(string studentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentName))
                {
                    throw new ArgumentException("Student name cannot be empty");
                }

                System.Diagnostics.Debug.WriteLine($"[SERVICE] Creating: {studentName}");

                // Get max ID
                int maxId = 0;
                if (_firebaseKeyAndId.Any())
                {
                    maxId = _firebaseKeyAndId.Keys.Max();
                }

                int newId = maxId + 1;

                var student = new Student
                {
                    StudentId = newId,
                    StudentName = studentName.Trim()
                };

                //convert max id to string
                string idStr = "Std" + newId;
                // Add to Firebase
                await _firebaseClient.Child(STUDENTS_NODE).Child(idStr).PutAsync(student);


                // Update BOTH dictionary and local list
                _firebaseKeyAndId[newId] = idStr;
                _localStudents.Add(student);

                StudentChanged?.Invoke(this, new StudentChangedEventArgs
                {
                    Student = student,
                    ChangeType = ChangeType.Added
                });

                System.Diagnostics.Debug.WriteLine($"[SERVICE] Created student {newId}");

                return student;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // UPDATE STUDENT
        // ==========================================

        public async Task<bool> UpdateStudentAsync(int studentId, string studentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentName))
                {
                    throw new ArgumentException("Student name cannot be empty");
                }

                System.Diagnostics.Debug.WriteLine($"[SERVICE] Updating student {studentId}");

                // Check if we have the Firebase key
                if (!_firebaseKeyAndId.ContainsKey(studentId))
                {
                    // Load from Firebase first
                    await GetAllStudentsAsync();

                    if (!_firebaseKeyAndId.ContainsKey(studentId))
                    {
                        return false;
                    }
                }

                string firebaseKey = _firebaseKeyAndId[studentId];

                var updatedStudent = new Student
                {
                    StudentId = studentId,
                    StudentName = studentName.Trim()
                };

                // Update in Firebase
                await _firebaseClient
                    .Child(STUDENTS_NODE)
                    .Child(firebaseKey)
                    .PutAsync(updatedStudent);

                //  Update in local list
                var localStudent = _localStudents.FirstOrDefault(s => s.StudentId == studentId);
                if (localStudent != null)
                {
                    localStudent.StudentName = studentName.Trim();
                }

                StudentChanged?.Invoke(this, new StudentChangedEventArgs
                {
                    Student = updatedStudent,
                    ChangeType = ChangeType.Updated
                });

                System.Diagnostics.Debug.WriteLine($"[SERVICE] Updated student {studentId}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SERVICE ERROR] {ex.Message}");
                return false;
            }
        }
        // ==========================================
        // GET TOTAL COUNT
        // ==========================================

        public async Task<int> GetTotalCountAsync()
        {
            var students = await GetAllStudentsAsync();
            return students.Count;
        }

      
    }
}