namespace Week3_L1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        protected override void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine("[APP] OnStart");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            System.Diagnostics.Debug.WriteLine("[APP] OnSleep");
        }

        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("[APP] OnResume");
        }
    }
}