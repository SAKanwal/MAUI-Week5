using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Week3_L1.Services;
namespace Week3_L1.ViewModels
{
    public class StartPageViewModel :INotifyPropertyChanged
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalStudentsText));
            }
        }
        private readonly IStudentServices _studentServices;

        int totalStudents = 0;
        public string TotalStudentsText
        {
            get
            {
                if (IsLoading)
                    return "Loading students...";  // Shows while loading

                return $"Total Students: {totalStudents}";  // Shows when done
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand GoToStudentViewCommand { get; }

        public StartPageViewModel(IStudentServices studentServices)
        {
            GoToStudentViewCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//StudentView");
            });
            _studentServices = studentServices;
            // Subscribe to changes
            _studentServices.StudentChanged += Students_CollectionChanged;
        }

        public async Task LoadServiceDataAsync()
        { 
            if(IsLoading)
            { return; }
            try
            {
                IsLoading = true;
                totalStudents = await _studentServices.GetTotalCountAsync();
                OnPropertyChanged(nameof(TotalStudentsText));
            }
            finally { IsLoading = false; }

        }

        private void Students_CollectionChanged(object? sender, StudentChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalStudentsText));

        }
    }

}
