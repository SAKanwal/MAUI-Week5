using Week3_L1.Models;
using Week3_L1.ViewModels;
namespace Week3_L1.Views;


public partial class StudentDetailPage : ContentPage,IQueryAttributable
{
	StudentDetailViewModel viewModel;
	public StudentDetailPage()
	{
		InitializeComponent();
        viewModel = new StudentDetailViewModel();
		BindingContext = viewModel;

	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
		Student selectedStudent = query["student"] as Student;
		viewModel.LoadStudent(selectedStudent);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("[PAGE] StudentDetailPage Appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        System.Diagnostics.Debug.WriteLine("[PAGE] StudentDetailPage Disappearing");
    }
}