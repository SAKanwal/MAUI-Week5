namespace Week3_L1.Views;

using Week3_L1.ViewModels;

public partial class StudentView : ContentPage
{

    StudentViewModel vm;
	public StudentView(StudentViewModel viewModel)
	{
		InitializeComponent();
        vm = viewModel;
        this.BindingContext = vm;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("[PAGE] StudentListPage Appearing");
        var _ = vm.LoadServiceDataAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        System.Diagnostics.Debug.WriteLine("[PAGE] StudentListPage Disappearing");
    }
}