using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week3_L1.Models;

namespace Week3_L1.Services
{
    public class StudentService : IStudentServices
    {
        private readonly List<Student> _students;
        private int _nextId = 1;

        public event EventHandler<StudentChangedEventArgs>? StudentChanged;

        public StudentService()
        {
            _students = new List<Student>();
            LoadData();
        }
        // Add to StudentService.cs

        public async Task<Student> CreateStudentAsync(string studentName)
        {
            await Task.Delay(100);

            // Validate
            if (string.IsNullOrWhiteSpace(studentName))
            {
                throw new ArgumentException("Student name cannot be empty");
            }

            // 🎯 Service creates the ENTIRE Student object
            var newStudent = new Student
            {
                StudentId = _nextId++,      // Auto-assigned
                StudentName = studentName.Trim()
            };

            _students.Add(newStudent);

            // Fire event
            StudentChanged?.Invoke(this, new StudentChangedEventArgs
            {
                Student = newStudent,
                ChangeType = ChangeType.Added
            });

            return newStudent;
        }
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            await Task.Delay(5);
            return _students.Select(s => new Student
            {
                StudentName = s.StudentName,
                StudentId = s.StudentId

            }).ToList();
        }



        public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
        {
            await Task.Delay(1);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStudentsAsync();

            var query = searchTerm.Trim().ToLower();

            return _students
                .Where(s => s.StudentName.ToLower().Contains(query))

                .Select(s => new Student
                {
                    StudentName = s.StudentName,
                    StudentId = s.StudentId,

                })
                .ToList();
        }

 

        public async Task<bool> UpdateStudentAsync(int studentId, string studentName)
        {
            await Task.Delay(1);

            var existing = _students.FirstOrDefault(s => s.StudentId == studentId);
            if (existing == null)
                return false;

            existing.StudentName = studentName;


            StudentChanged?.Invoke(this, new StudentChangedEventArgs 
            {
                Student = existing,
                ChangeType = ChangeType.Updated
            });

            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            await Task.Delay(1);
            return _students.Count;
        }

        private void LoadData()
        {
            var sampleStudents = new List<Student>
        {
            new Student
            {
                StudentId = _nextId++,
                StudentName = "John",

            },
            new Student
            {
                StudentId = _nextId++,
                StudentName = "Thomas",

            },
            new Student
            {
                StudentId = _nextId++,
                StudentName = "Mark",

            },
            new Student
            {
                StudentId = _nextId++,
                StudentName = "Jame",

            }
        };

            _students.AddRange(sampleStudents);
        }
    }
}
