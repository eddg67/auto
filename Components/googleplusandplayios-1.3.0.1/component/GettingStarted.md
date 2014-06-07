Showing a G+ Sign In Button
===========================

### Setup your project settings

Before you can use Google Plus SDK and run this code, you must create an APIs Console project at [https://developers.google.com/console](https://developers.google.com/console) make sure you take note of your Bundle ID you set on Google's portal.

Once you registered and obtained a Client Id, you must set the same Bundle ID you used on Google's Portal on your Bundle Identifier inside your Info.plist.

And one last step you need to setup an **URL Scheme** in your Info.plist, The URL scheme must be declared the same as **Your Bundle ID** (the one you used on Google's portal). In order to add this URL scheme you need to open your Info.plist > Select Advanced Tab > clic Add URL Type button > on URL Schemes box write your url scheme > save Info.plist.

### AppDelegate

```csharp
using Google.Plus;
using Google.OpenSource;
//...

const string ClientId = "<Get your ID at https://developers.google.com/console>";

public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	// Configure the SignIn shared singleton instance by declaring 
	// its client ID, delegate, and scopes.
	var signIn = SignIn.SharedInstance;
	signIn.ClientId = ClientId;
	signIn.Scopes = new [] { PlusConstants.AuthScopePlusLogin, PlusConstants.AuthScopePlusMe };
	signIn.ShouldFetchGoogleUserEmail = true;
	//...
}

public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
	// This handler will properly handle the URL that your application 
	// receives at the end of the authentication process.
	return UrlHandler.HandleUrl (url, sourceApplication, annotation);
}
```

### Your View Controller

```csharp
using Google.Plus;
using Google.OpenSource;
//...

SignInButton btnSignIn;

public override void ViewDidLoad ()
{
	base.ViewDidLoad ();

	// Create the G+ SignIn button
	btnSignIn = new SignInButton () {
		Frame = new RectangleF (85, 176, 151, 48)
	};

	// Setup event listener when user logins

	SignIn.SharedInstance.Finished += (s, e) => {
		if (e.Error != null) {
			InvokeOnMainThread (()=> new UIAlertView ("Error", "Could not sign in.\nError: " + e.Error.Description, null, "Ok", null).Show ());
		} else {
			InvokeOnMainThread (()=> {
				new UIAlertView ("Success", "LogIn complete, your email is: " + SignIn.SharedInstance.UserEmail, null, "Ok", null).Show ();
				btnSignIn.Hidden = true;
			});
		}
	};

	// Add it to your subview
	View.AddSubview (btnSignIn);
}
```
## Documentation

* Google+ Platform for iOS: [https://developers.google.com/+/mobile/ios/](https://developers.google.com/+/mobile/ios/)
* Getting Started: [https://developers.google.com/+/mobile/ios/getting-started](https://developers.google.com/+/mobile/ios/getting-started)
* API Reference: [https://developers.google.com/+/mobile/ios/api/](https://developers.google.com/+/mobile/ios/api/)
* Google Play Game Services: [https://developers.google.com/games/services/](https://developers.google.com/games/services/)