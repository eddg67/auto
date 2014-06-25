using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Google.Plus;
using Google.OpenSource;
using MonoTouch.FacebookConnect;
using AutoLink.Services;
using AutoLink.Utilities;
using BigTed;
using AutoLink.Models;
using MonoTouch.MessageUI;

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
		public API webService;
		public UINavigationController RootController;
		public Validator validator;
		public PasswordResetController forgot;
		public SearchService searchService;
		public SearchResultController searchResult;
		public MainScreenController splash;
		public SignupController signup;
		public SearchScreen search;
		public ResetController reset;
		public DetailViewController detail;
		public ImageViewerViewController imageViewerController;
		public PriceTabController priceTab;
		public ImageViewController imageViewController;

		MFMailComposeViewController mailController = new MFMailComposeViewController();
		public NetworkStatus noNetwork;
		public LoginViewController login;

		public static AppDelegate app;


		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			app = this;
			// Override point for customization after application launch.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				this.loadInitForPad ();
			}
			//check status
		
			noNetwork = Reachability.InternetConnectionStatus();
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

			UINavigationBar.Appearance.BarTintColor = UIColor.Black;
			UINavigationBar.Appearance.BackgroundColor = UIColor.Clear;
			UITextAttributes attr = new UITextAttributes ();
			attr.TextColor = UIColor.White;
			attr.Font = UIFont.FromName ("KannadaSangamMN-Bold", 22);
			UINavigationBar.Appearance.SetTitleTextAttributes (attr);
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, true);

			RootController = new UINavigationController ();

			//Pass Thru TO list if logged in
			if (loginService.IsLoggedIn()) {
				ShowResultList ();
			} else {
				ShowSplash ();
			}
				

			Window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = RootController
			};

			this.Window.MakeKeyAndVisible (); 

			return true;
		}


		public void ShowSplash()
		{
			splash = new MainScreenController ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (splash, true);

		}

		public void ShowLogin()
		{
			login = new LoginViewController ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			var nav = new UINavigationController (login);
			RootController.PresentViewController (nav, true, null);
			//RootController.AddChildViewController (nav);

			login.LoginSucceeded += () => {
				nav.DismissViewController(true,null);
				ShowResultList();
			};
				

		}

		public void ShowSearch()
		{
			BTProgressHUD.Dismiss ();
			search = new SearchScreen ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (search, true);

		}

		public void SendEmail(string email, string title)
		{
			if (MFMailComposeViewController.CanSendMail) {
				try {
					if (validator.isEmail (email)) {
						mailController.SetToRecipients (new string[]{ email });
					}

					if(!string.IsNullOrEmpty(title)){
						mailController.SetSubject (title);

					}
					//_mailController.SetMessageBody ("this is a test", false);
					mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
						Console.WriteLine (args.Result.ToString ());
						RootController.NavigationBarHidden = false;
						RootController.ToolbarHidden = false;
						args.Controller.DismissViewController (true, null);
					};
					RootController.PresentViewController(mailController, true, null);

				} catch (Exception exp) {
					Console.WriteLine (exp.Message);

				}
			}
				
		}

		public void OpenUrl(string url){

			UIApplication.SharedApplication.OpenUrl (new NSUrl (url));
		}

		public void ShowImageController(Listing item)
		{
			var  flowLayout = new UICollectionViewFlowLayout (){
				HeaderReferenceSize = new System.Drawing.SizeF (0, 0),
				SectionInset = new UIEdgeInsets (50,0,50,50),
				ScrollDirection = UICollectionViewScrollDirection.Vertical,
				MinimumInteritemSpacing = 50, // minimum spacing between cells
				MinimumLineSpacing = 100 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
			};

			imageViewerController = new ImageViewerViewController (flowLayout,item);
		
			RootController.NavigationBarHidden = false;
			RootController.ToolbarHidden = true;

			RootController.PushViewController (imageViewerController, true);
		}

		public void ShowLargeImageView(Listing list,int count)
		{
			imageViewController = new ImageViewController (list,count);
			RootController.NavigationBarHidden = false;
			RootController.ToolbarHidden = true;

			RootController.PushViewController (imageViewController, true);
		}

		public void SendPhone(string phone)
		{
			 phone = phone
				.Replace("-",string.Empty)
				.Replace("/(",string.Empty).Replace(")",string.Empty);

			var urlToSend = new NSUrl ("tel:" +phone ); // phonenum is in the format 1231231234

			if (UIApplication.SharedApplication.CanOpenUrl (urlToSend)) {
				UIApplication.SharedApplication.OpenUrl(urlToSend);
			} else {
				// Url is not able to be opened.
			}
		}

		public void ShowDetail(string searchID,Listing item)
		{
			detail = new DetailViewController (searchID,item);
			RootController.NavigationBarHidden = false;
			RootController.ToolbarHidden = false;
		
			RootController.PushViewController (detail, true);
		}

		public void ShowReset()
		{
			reset = new ResetController();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (reset, true);

		}
			
		public void ShowSignUp()
		{
			signup = new SignupController ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (signup, true);


		}

		public void ShowForgotPassword()
		{
			BTProgressHUD.Dismiss ();
			forgot = new PasswordResetController ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (forgot, true);
		}

		public void ShowPriceEdmunds(Pricing price)
		{
			BTProgressHUD.Dismiss ();
			priceTab = new PriceTabController (price);
			RootController.NavigationBarHidden = false;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (priceTab, true);
		}



		public void ShowResultList()
		{
			if (searchResult == null) {
				BTProgressHUD.Show ("Get Search Results...");

			}
			searchResult = new SearchResultController ();
			RootController.NavigationBarHidden = true;
			RootController.ToolbarHidden = true;
			RootController.PushViewController (searchResult, true);

		}

		public void setUpLocalNotifications(int count){

			UILocalNotification notification = new UILocalNotification();
			notification.FireDate = DateTime.Now.AddMinutes(1);
			notification.ApplicationIconBadgeNumber = count;
			UIApplication.SharedApplication.ScheduleLocalNotification(notification);
		}


		#region OverWrites on Delegate
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
			//must load API first since other services use
			webService = new API ();
			loginService = new LoginService ();
			validator = new Validator ();
			searchService = new SearchService ();

		}
			
			

		#endregion
	}
}

