using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Week3_L1.Models;
using Week3_L1.Services;
namespace Week3_L1.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        bool is_loading = false;

        // =======================
        // INotifyPropertyChanged
        // =======================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // =======================
        // Search
        // =======================
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                _ = ApplyFilter();
            }
        }

        // =======================
        // Selection
        // =======================
        private Student selectedStudent;
        public Student SelectedStudent
        {
            get => selectedStudent;
            set
            {
                //Show the details of the Student in a new page

                selectedStudent = value;

                selectedStudent = value;
                if (selectedStudent != null)
                {
                    //Shell.Current.DisplayAlert(
                    //    "Student Clicked",
                    //    selectedStudent.StudentName,
                    //    "OK"
                    //);

                }
            }
        }

        // =======================
        // Collections
        // =======================
        public ObservableCollection<Student> Students { get; set; }


        // =======================
        // Commands
        // =======================
        public ICommand AddStudentCommand { get; }
        public ICommand EditStudentCommand { get; }
        public ICommand StudentSelectedCommand { get; }
        private readonly IStudentServices _studentService;

        // =======================
        // Constructor
        // =======================
        public StudentViewModel(IStudentServices studentService, ILogger<StudentViewModel> logger)
        {
            logger.LogInformation("Testing");
            _studentService = studentService;
            _studentService.StudentChanged += _studentService_StudentChanged;
            
            // Create an empty observable collection
            Students = new ObservableCollection<Student>();

            AddStudentCommand = new Command(AddStudent);
            EditStudentCommand = new Command<Student>(EditStudent);
            StudentSelectedCommand = new Command<Student>(
       async student =>
       {
           if (student == null) return;

           await Shell.Current.GoToAsync(
               "/studentDetailPage",
               new Dictionary<string, object>
               {
                   ["student"] = student
               });
       });
            _studentService = studentService;
        }

        private void _studentService_StudentChanged(object? sender, StudentChangedEventArgs e)
        {

            switch (e.ChangeType)
            {
                case ChangeType.Updated:
                    var existing = Students.FirstOrDefault(s => s.StudentId == e.Student.StudentId);
                    if (existing != null)
                    {
                        var index = Students.IndexOf(existing);
                        Students[index] = e.Student;

                    }
                    break;

                case ChangeType.Added:
                    if (!Students.Any(s => s.StudentId == e.Student.StudentId))
                    {
                        Students.Add(e.Student);
                    }
                    break;
            }
        }



        public async Task LoadServiceDataAsync()
        {
            try
            {
                await ApplyFilter();
            }
            finally
            {
            }
        }

        // =======================
        // Add Student
        // =======================
        async void AddStudent()
        {
            var name = await Shell.Current.DisplayPromptAsync(
                "New Student",
                "Enter name:");

            if (string.IsNullOrWhiteSpace(name))
                return;

            Student student = await _studentService.CreateStudentAsync(name);

        }

        // =======================
        // Edit Student
        // =======================
        async void EditStudent(Student student)
        {
            if (student == null)
                return;

            string updatedName = await Shell.Current.DisplayPromptAsync(
                "Edit Student",
                "Update student name",
                initialValue: student.StudentName);

            if (!string.IsNullOrWhiteSpace(updatedName))
            {
                student.StudentName = updatedName;
            }
            await _studentService.UpdateStudentAsync(student.StudentId, updatedName);

        }

        // =======================
        // Filtering Logic
        // =======================
        async Task ApplyFilter()
        {
            if (is_loading)
            {
                return;
            }
            try
            {
                is_loading = true;
                Students.Clear();

                var query = (SearchText ?? string.Empty).Trim().ToLower();

                var students = await _studentService.SearchStudentsAsync(query);
                foreach (var student in students)
                {
                    {
                        Students.Add(student);
                    }
                }
            }
            finally
            {
                is_loading = false;
            }
        }
    }
}
