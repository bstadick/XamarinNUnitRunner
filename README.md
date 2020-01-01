# XamarinNUnitRunner

NUnit test runner for Xamarin based projects.

This project looks to revive the [NUnit](https://github.com/nunit/nunit.xamarin) and [Xamarin NUnitLite](https://github.com/xamarin) unit test runner projects and combine the granular interface provided by the Xamarin runner with the full functionality provided by the NUnit framework.

Features include:

- Exploring loaded tests by namespace, class, and method
- Running tests and test cases individually or by namespace, class, or method
- Viewing overall results and individual results with details
- Running tests on a background thread, leaving the GUI thread available to perform other work

## Usage

1. Include a reference to the XamarinNUnitRunner DLL or Nuget package in a Xamarin.Forms project for your target platform.
2. Include references to the project with your NUnit tests in the Xamarin.Forms project.
3. In the launch method for the Xamarin.Forms project, create and load an instance of the XamarinNUnitRunner.App. Where this is done will vary based on the platform being targeted.
    - For Android: place in MainActivity.OnCreate()
    - For iOS: place in AppDelegate.FinishedLaunching()
    - For UWP: place in MainPage.MainPage()
4. The XamarinNUnitRunner.App takes in an NUnitRunner as a parameter to the constructor. Add assembly references that contain NUnit tests to this NUnitRunner.
5. Build the Xamarin.Forms project in Debug. NUnit will not be able to work correctly in Release builds.

```csharp
NUnitRunner runner = new NUnitRunner(GetType().Namespace);
runner.AddTestAssembly(typeof(Test.Stub.TestFixtureStubOne).Assembly);

LoadApplication(new App(runner));
```

## Build

Use the provided solution and Visual Studio 2019 to build the project.

The XamarinNUnitRunner project can be built and referenced using the DLL or preferably using the pre-built Nuget package.

## Examples

Example test runner projects can be found in the examples folder.

## Future Enhancements

- User interface improvements and beautification
- Running tests by category
- Exporting results to a file
- Searching and filtering tests
- Configuring test settings
- Live updates/console as tests are progressing
- Remote test management

## Contributing

Pull requests for new features or fixes and reporting issues are welcome. When developing, follow the MVVM pattern as best as possible. Follow patterns in existing code when grouping code elements within classes. Additional resources should be included as Embedded Resources and strings included in a resx file where applicable. Consider testability and use TDD to develop new code and make changes. Unit tests should maintain at least 95% code coverage.
