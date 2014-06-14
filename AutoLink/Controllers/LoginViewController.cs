using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Services;
using AutoLink.Utilities;
using BigTed;

namespace AutoLink
{
	public partial class LoginViewController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		LoginService loginService;
		Validator validate;
		//UIView ContentView;
		public event Action LoginSucceeded = delegate {};

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public LoginViewController ()
			: base (UserInterfaceIdiomIsPhone ? "LoginViewController_iPhone" : "LoginViewController_iPad", null)
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

			this.NavigationController.NavigationBarHidden = true;


			this.btnLogin.Layer.BorderWidth = 1.0f;
			this.btnLogin.Layer.BorderColor = UIColor.White.CGColor;

			HandleTxt ();

			HandleBtn ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			//add facebook/google+ btn
			//float x=22,float y=120,float width=130,float height = 200
			View.AddSubview( loginService.CreateFaceBookBtn (22,120,270,50));
			View.AddSubview( loginService.CreateGooglePlusBtn (22,170,270,50));
		}

		void HandleTxt()
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

		void HandleBtn()
		{
			this.btnLogin.Layer.BorderWidth = 1.0f;
			this.btnLogin.Layer.BorderColor = UIColor.White.CGColor;

			this.btnLogin.TouchUpInside += (object sender, EventArgs e) => {
				if(this.validate.isEmail(txtEmail) && !this.validate.isEmptyTxt(txtPassword)){
					//loading screen
					BTProgressHUD.Show ("Logging in...");

					if(loginService.login(txtEmail.Text,txtPassword.Text))
					{
						//success
						LoginSucceeded ();
						BTProgressHUD.Dismiss ();
					
					}else{
						BTProgressHUD.Dismiss ();
					}

				}else{
					using(var alert = new UIAlertView("Form Errors", "Please try again",null,"OK",null))
					{
						alert.Show ();
						BTProgressHUD.Dismiss ();
					}
				}
			};

			this.btbSignUp.TouchUpInside += (object sender, EventArgs e) => {
				DismissViewController(true,null);
				app.ShowSignUp();
			};

			this.btnForgot.TouchUpInside += (sender, e) => {
				DismissViewController(true,null);
				app.ShowForgotPassword();
				//ForgotSelect();
			};



		}
	}
}

