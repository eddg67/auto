using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Services;
using AutoLink.Utilities;
using AutoLink.Models;
//http://stackoverflow.com/questions/9614203/uitextfield-and-uitextview-cant-type-in-them
namespace AutoLink
{
	public partial class SignUpController : BaseController
	{
		LoginService loginService;
		Validator validate;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SignUpController ()
		{
			AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
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
			base.RegisterForKeyboardNotifications();

			this.handleBtn ();
			this.handleTxt ();

			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		private void handleBtn()
		{

			this.btnSubmit.Layer.BorderWidth = 1.0f;
			this.btnSubmit.Layer.BorderColor = UIColor.White.CGColor;

			this.btnClose.TouchUpInside += (sender, e) => {

				var loginVC = new LoginScreenController();
				this.PresentViewController(loginVC,true,null);
			};

			this.btnSubmit.TouchUpInside += (sender, e) => {
				signUp();
			};
			//10f, 145f, 141f, 48f
			View.AddSubview( loginService.CreateFaceBookBtn ());
			//150f, 145f, 141f, 48f
			View.AddSubview( loginService.CreateGooglePlusBtn ());
		}


		private void handleTxt()
		{
			this.txtEmail.AllTouchEvents += (object sender, EventArgs e) => {
				txtEmail.AllowsEditingTextAttributes = true;
			};
		
			//this.txtEmail.ShouldEndEditing = true;
			this.txtEmail.EditingDidEndOnExit += (sender, e) => {
				this.txtEmail.EndEditing (true);
			};
				

			this.txtPassword.EditingDidEndOnExit += (sender, e) => {
				this.txtPassword.EndEditing (true);
			};

	
			//this.txtEmail.ShouldEndEditing = true;
			this.txtName.EditingDidEndOnExit += (sender, e) => {
				this.txtName.EndEditing (true);
			};
				
			this.txtZip.EditingDidEndOnExit += (sender, e) => {
				this.txtZip.EndEditing (true);
			};

			this.txtPassword.EditingDidEndOnExit += (sender, e) => {
				this.txtPassword.EndEditing (true);
			};




		}



		private void signUp()
		{

			if (validate.isEmail(txtEmail)
			   && !validate.isEmptyTxt (txtName)
			   && !validate.isEmptyTxt (txtPassword)
			   && !validate.isEmptyTxt (txtZip)) {

				if (post ()) 
				{
					//go to search start fresh signup
				}

			} else {
			}
		}

		private bool post()
		{
			bool result = false;
			APIResponse<LoginResult> response = loginService.api.CreateRequest<LoginResult>(
				"account.signup",
				new {name=txtName,email=txtEmail.Text,password=txtPassword.Text}
			);

			if (response != null)
			{
				if (response.error != null) 
				{
					using(var alert = new UIAlertView("Sign Up Error", 
						string.Format("Please try again--{0}--Code:{1} ",
							response.error.message,response.error.code),null,"OK",null))
					{
						alert.Show ();
					}
					//we good TODO
				}else if(response.Result != null){
					processResponse (response.Result);
					result = true;

				}
			}
			return result;
		}
		//process response
		private void processResponse(LoginResult result)
		{
		} 




	}
}

