using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            // TODO - Initialize test runner and add test assembly references
            NUnitRunner runner = new NUnitRunner(GetType().Namespace);
            runner.AddTestAssembly(typeof(Test.Stub.TestFixtureStubOne).Assembly);

            XamarinNUnitRunner.App app = new XamarinNUnitRunner.App(runner);
            LoadApplication(app);
        }
    }
}
