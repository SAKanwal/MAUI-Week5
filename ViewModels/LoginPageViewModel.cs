using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Week3_L1.Services;

namespace Week3_L1.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        private readonly IStudentServices _studentService;

        // ---- INotifyPropertyChanged ----
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // ---- Username ----
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        // ---- Password ----
        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        // ---- Command ----
        public ICommand LoginCommand { get; }

        public LoginPageViewModel(IStudentServices studentService)
        {
            _studentService = studentService;

            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
             try
    {
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter username and password.", "OK");
            return;
        }

        var success = await _studentService.LoginAsync(Username, Password);

        if (success)
        {
            await Shell.Current.GoToAsync("//StudentView");
        }
        else
        {
            await Shell.Current.DisplayAlert("Login Failed", "Invalid username or password.", "OK");
        }
    }
    catch (Exception ex)
    {
        // This will show you exactly what is crashing
        await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
    }
        }
        }
}