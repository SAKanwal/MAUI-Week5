using System.Windows.Input;
using Week3_L1.Views;

namespace Week3_L1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            Routing.RegisterRoute("studentDetailPage", typeof(StudentDetailPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));  // <-- ADD

        }
        public ICommand LogoutCommand =>
       new Command(async () =>
       {
        // Clear token
            SecureStorage.Remove("auth_token");
           await Shell.Current.GoToAsync("//startpage");
       });

    }
}
