using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Services;
using Google.Plus;
using MonoTouch.FacebookConnect;
using AutoLink.Utilities;
using Autolink;
using MonoTouch.Dialog;
using AutoLink.Models;

namespace AutoLink
{
	public partial class LoginScreenController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		LoginService loginService;
		Validator validate;
		//FlyoutController flyOut;
		LoadingOverlay loadingOverlay;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public LoginScreenController ()
			: base (UserInterfaceIdiomIsPhone ? "LoginScreenController_iPhone" : "LoginScreenController_iPad", null)
		{
			loginService = app.loginService;
			validate = app.validator;

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			app.RootController.AddChildViewController(this);
			//flyOut = new FlyoutController();
			// Perform any additional setup after loading the view, typically from a nib.
			HandleBtn ();
			HandleTxt ();

			//UITapGestureRecognizer g = new UITapGestureRecognizer(() => this.View.EndEditing(true));
			//UITapGestureRecognizer h = new UITapGestureRecognizer(() => this.txtPassword.EndEditing(true));
			//this.View.AddGestureRecognizer(g);
			//this.txtPassword.AddGestureRecognizer(h);


		}


		private void HandleTxt()
		{
			this.txtEmail.EditingDidBegin += (sender, e) => {
				this.txtEmail.Layer.BorderColor = UIColor.Clear.CGColor;

			};
			//this.txtEmail.ShouldEndEditing = true;
			this.txtEmail.EditingDidEndOnExit += (sender, e) => {
				this.txtEmail.EndEditing (true);
			};
				
			this.txtPassword.EditingDidBegin += (sender, e) => {
				this.txtPassword.Layer.BorderColor = UIColor.Clear.CGColor;

			};

			this.txtPassword.EditingDidEndOnExit += (sender, e) => {
				this.txtEmail.EndEditing (true);
			};


		}

		private void HandleBtn()
		{
			this.btnLogin.Layer.BorderWidth = 1.0f;
			this.btnLogin.Layer.BorderColor = UIColor.White.CGColor;

			this.btnLogin.TouchUpInside += (object sender, EventArgs e) => {

				if(this.validate.isEmail(txtEmail) && !this.validate.isEmptyTxt(txtPassword)){

					//loading screen
					loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
					View.Add (loadingOverlay);

					if(loginService.login("",txtEmail.Text,txtPassword.Text))
					{
						loadingOverlay.Hide();
						//success
						var nScreen = new SearchResultController();
						//this.NavigationController.PopViewControllerAnimated(true);
						this.NavigationController.PushViewController(nScreen,true);
					}

				}else{
					using(var alert = new UIAlertView("Form Errors", "Please try again",null,"OK",null))
					{
						alert.Show ();
						loadingOverlay.Hide();
					}
				}
			};

			this.btnSignup.TouchUpInside += (object sender, EventArgs e) => {
				var screen = new SignUpController();
				this.PresentViewController(screen,true,null);
				screen.Dispose();
			};

			this.btnForgotPassword.TouchUpInside += (sender, e) => {
				var screen = new PasswordResetController();
				this.PresentViewController(screen,true,null);
				screen.Dispose();

			};


			//add facebook/google+ btn
			//float x=22,float y=120,float width=130,float height = 200
			View.AddSubview( loginService.CreateFaceBookBtn (22,120,270,50));
			View.AddSubview( loginService.CreateGooglePlusBtn (22,170,270,50));
		

		}



	}
}

