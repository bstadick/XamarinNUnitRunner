using System.Diagnostics;
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

            // Add a test listener to output results as tests are ran
            NUnitTestListener listener = new NUnitTestListener();
            listener.WriteOutput += message => Debug.WriteLine(message);
            runner.TestListener = listener;

            // Load the Xamarin.Forms application with the test runner
            XamarinNUnitRunner.App app = new XamarinNUnitRunner.App(runner);
            LoadApplication(app);
        }
    }
}
