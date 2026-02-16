using Week3_L1.ViewModels;

namespace Week3_L1.Views;

public partial class StartPage : ContentPage
{
	StartPageViewModel vm;
	public StartPage(StartPageViewModel viewModel)
	{
		InitializeComponent();
		vm = viewModel;
        BindingContext = viewModel;

    }
	protected override void OnAppearing()
	{
		base.OnAppearing();
        _ = vm.LoadServiceDataAsync();
	}
}