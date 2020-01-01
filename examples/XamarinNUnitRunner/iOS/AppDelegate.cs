using Foundation;
using UIKit;
using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();

            // TODO - Initialize test runner and add test assembly references
            NUnitRunner runner = new NUnitRunner(GetType().Namespace);
            runner.AddTestAssembly(typeof(Test.Stub.TestFixtureStubOne).Assembly);

            LoadApplication(new App(runner));

            return base.FinishedLaunching(app, options);
        }
    }
}
