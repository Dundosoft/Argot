using Foundation;

namespace Argot;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => Argot.CreateMauiApp();
}
