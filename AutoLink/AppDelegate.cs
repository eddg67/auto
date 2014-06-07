using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Google.Plus;
using Google.OpenSource;
using MonoTouch.FacebookConnect;
using AutoLink.Services;
using AutoLink.Utilities;

namespace AutoLink
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public override UIWindow Window {
			get;
			set;
		}

		public LoginService loginService;
		public UINavigationController RootController;
		public Validator validator;
		public SearchService searchService;

		#region OverWrites on Delegate

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				this.loadInitForPad ();
			}
			//check status
		
			NetworkStatus noNetwork = Reachability.InternetConnectionStatus();
			//check connection 
			if (noNetwork == NetworkStatus.NotReachable) 
			{
				using(var alert = new UIAlertView("No Connection Found", "No Network Connections "+noNetwork,null,"OK",null))
				{
					alert.Show ();
				}

				return false;
			}

			//loading services
			loadServices ();

			//create and add root controller
			RootController = new UINavigationController(new RootController()); 
			RootController.ToolbarHidden = true;


			Window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = RootController
			};

			this.Window.MakeKeyAndVisible (); 

			return true;
		}
		//
		// This method is invoked when the application is about to move from active to inactive state.
		//
		// OpenGL applications should use this method to pause.
		//
		public override void OnResignActivation (UIApplication application)
		{
		}
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// This handler will properly handle the URL that your application 
			// receives at the end of the authentication process.
			return (loginService.isFaceBookLogin) 
					? FBSession.ActiveSession.HandleOpenURL (url)
					:	UrlHandler.HandleUrl (url, sourceApplication, annotation);
		}

		public override void OnActivated (UIApplication application)
		{
			// We need to properly handle activation of the application with regards to SSO
			// (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
			FBSession.ActiveSession.HandleDidBecomeActive ();

		}

		#endregion

		#region Private Methods

		private void loadInitForPad ()
		{
			var splitViewController = (UISplitViewController)Window.RootViewController;

			// Get the UINavigationControllers containing our master & detail view controllers
			var masterNavigationController = (UINavigationController)splitViewController.ViewControllers [0];
			var detailNavigationController = (UINavigationController)splitViewController.ViewControllers [1];

			var masterViewController = (MasterViewController)masterNavigationController.TopViewController;
			var detailViewController = (DetailViewController)detailNavigationController.TopViewController;

			masterViewController.DetailViewController = detailViewController;

			// Set the DetailViewController as the UISplitViewController Delegate.
			splitViewController.WeakDelegate = detailViewController;
		}

		private void loadServices()
		{
			loginService = new LoginService ();
			validator = new Validator ();
			searchService = new SearchService ();
		}

		private void loadControllers()
		{
		}

		private void loadViews()
		{
		}

		#endregion
	}
}

