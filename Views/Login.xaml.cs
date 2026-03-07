using Week3_L1.ViewModels;

namespace Week3_L1.Views;

public partial class LoginPage : ContentPage
{
	 public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;  // <-- connects ViewModel to page
    }
}