using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Week3_L1.Models;

namespace Week3_L1.ViewModels
{
    public class StudentDetailViewModel : INotifyPropertyChanged
    {
        private Student currentStudent;

        public void LoadStudent(Student student)
        {
            CurrentStudent = student;
        }
        public Student CurrentStudent
        {
            get => currentStudent;
            set { 
                currentStudent = value; 
                OnPropertyChanged(nameof(CurrentStudent));

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
            
    }
}
