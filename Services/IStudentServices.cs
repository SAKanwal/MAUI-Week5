using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week3_L1.Models;

namespace Week3_L1.Services
{
    public interface IStudentServices
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<List<Student>> SearchStudentsAsync(string searchTerm);

        Task<Student> CreateStudentAsync(string studentName);
     
        Task<bool> UpdateStudentAsync(int StudentId, string student);
       // Task<bool> DeleteStudentAsync(int id);
        Task<int> GetTotalCountAsync();

        event EventHandler<StudentChangedEventArgs>? StudentChanged;
    }

    public class StudentChangedEventArgs : EventArgs
    {
        public required Student Student { get; set; }
        public ChangeType ChangeType { get; set; }
    }

    public enum ChangeType
    {
        Added,
        Updated,
        Deleted
    }
}
