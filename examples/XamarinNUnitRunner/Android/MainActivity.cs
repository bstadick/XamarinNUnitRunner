using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.Droid
{
    [Activity(Label = "XamarinNUnitRunner", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // ReSharper disable once UnusedType.Global
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // TODO - Initialize test runner and add test assembly references
            NUnitRunner runner = new NUnitRunner(GetType().Namespace);
            runner.AddTestAssembly(typeof(Test.Stub.TestFixtureStubOne).Assembly);

            // Add a test listener to output results as tests are ran
            NUnitTestListener listener = new NUnitTestListener();
            listener.WriteOutput += Console.WriteLine;
            runner.TestListener = listener;

            // Load the Xamarin.Forms application with the test runner
            LoadApplication(new App(runner));
        }

        // ReSharper disable once RedundantNameQualifier
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}