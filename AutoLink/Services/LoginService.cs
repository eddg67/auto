using System;
using MonoTouch.FacebookConnect;
using Google.Plus;
using System.Drawing;
using MonoTouch.UIKit;
using AutoLink.Models;
using PerpetualEngine.Storage;
using System.Threading.Tasks;

namespace AutoLink.Services
{
	public class LoginService
	{
		#region Social SDK Opts
		const string GoogleClientId = "65227870164.apps.googleusercontent.com";
		private string[] ExtendedPermissions = new [] { "user_about_me", "read_stream" };
		const string FacebookAppId = "304421419707605";
		const string DisplayName = "Autolink";
		SimpleStorage storage;

		#endregion

		public string AccessToken {
			get;
			set;
		}

		public API api;

		public LoginType LoggedInServiceType {
			get;
			set;
		}

		public enum LoginType
		{
			Standard = 1,
			GooglePlus = 2,
			FaceBook = 3
		} 

		public LoginService ()
		{
			AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
			InitSocialLogin();
			api = app.webService;
			storage = SimpleStorage.EditGroup("login");
			AccessToken = storage.Get ("token");

		}

		private bool isFaceBookAuth = false;
		private bool isGooglePlusAuth = false;
		public bool isFaceBookLogin   
		{
			get 
			{
				return isFaceBookAuth; 
			}
			set 
			{
				isFaceBookAuth = value; 
			}
		}
		public bool isGooglePlusLogin
		{
			get 
			{
				return isGooglePlusAuth; 
			}
			set 
			{
				isGooglePlusAuth = value; 
			}
		}

		public bool IsLoggedIn()
		{
			bool result = false;
			if (AccessToken != null) {

				this.LoggedInServiceType = LoginType.Standard;
				result = true;

			}else if (SignIn.SharedInstance.Authentication != null) {

				this.LoggedInServiceType = LoginType.GooglePlus;
				this.AccessToken = SignIn.SharedInstance.Authentication.AccessToken;
				result = true;

			} else if (FBSession.ActiveSession.State == MonoTouch.FacebookConnect.FBSessionState.CreatedTokenLoaded) {

				this.LoggedInServiceType = LoginType.FaceBook;
				this.AccessToken = FBSession.ActiveSession.AccessTokenData.ToString();
				result = true;
			} 

				
			return result;
		}

		public void InitSocialLogin()
		{
			InitGooglePlus ();
			InitFaceBook ();
		}


		public void InitGooglePlus ()
		{
			// Configure the SignIn shared singleton instance by declaring 
			// its client ID, delegate, and scopes.
			var signIn = SignIn.SharedInstance;
			signIn.ClientId = GoogleClientId;
			signIn.Scopes = new [] { PlusConstants.AuthScopePlusLogin, PlusConstants.AuthScopePlusMe };
			signIn.ShouldFetchGoogleUserEmail = true;

			signIn.Finished += (s, e) => {

				if (e.Error != null) {
					new UIAlertView ("Failure", "LogIn Failed ", null, "Error", null).Show();

				} else {
					//success move forward
					this.AccessToken = SignIn.SharedInstance.Authentication.AccessToken;
					this.isGooglePlusLogin = true;

					new UIAlertView ("Success", "LogIn complete, your email is: " + SignIn.SharedInstance.UserEmail, null, "Ok", null).Show ();

				}

			};

		}

		public void InitFaceBook()
		{
			FBSettings.DefaultAppID = FacebookAppId;
			FBSettings.DefaultDisplayName = DisplayName;
		}

		//float x=150,float y=120,float width=0,float height = 0
		public SignInButton CreateGooglePlusBtn(float x=160,float y=135,float width=0,float height = 0)
		{
			var btn = new SignInButton () {
				Frame = new RectangleF (x, y, width, height)
			};
			btn.SetStyle (SignInButtonStyle.Standard);
			btn.SetColorScheme (SignInButtonColorScheme.Light);
			btn.Opaque = true;

			//btn.Frame = new RectangleF (x, y, width, height);
			btn.ImageView.Image = UIImage.FromBundle("google_login.png");
			btn.ImageView.Image.Scale(new SizeF (width, height));
			//btn.Frame = new RectangleF (x, y, width, height);
			//btn.BackgroundColor = UIColor.Red;
			return btn;
		
		}
		//float x=22,float y=120,float width=130,float height = 200
		public FBLoginView CreateFaceBookBtn(float x=18,float y=135,float width=135,float height = 400)
		{
			var btn = new FBLoginView (ExtendedPermissions) {
				Frame = new RectangleF (x, y, width, height)

			};
			//the actual button in the view
			var backBTN = (UIButton)btn.Subviews [0];
			backBTN.SetBackgroundImage ( UIImage.FromBundle("facebook_login.png"), UIControlState.Normal);
			//backBTN.ImageView.Image.Scale(new SizeF (width, height));
			btn.FetchedUserInfo += (sender, e) => {

				var user = e.User;
				//var id = user.GetId ();
				//this.txtUser.Text = user.GetName ();
				this.isFaceBookLogin = true;
				this.LoggedInServiceType = LoginType.FaceBook;
				this.AccessToken = FBSession.ActiveSession.AccessTokenData.AccessToken;

			};
			return btn;

		}

		public bool signUp(string name,string email,string password)
		{
			bool result = false;

			APIResponse<LoginResult> response = api.CreateRequest<LoginResult>(
				"account.signup",
				new {name=name,email=email,password=password}
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
					//processResponse (response.Result);
					result = true;

				}
			}

		  return result;
		}


		public bool login(string email,string password)
		{
			bool result = false;
			APIResponse<LoginResult> response = api.CreateRequest<LoginResult>(
				"account.login",
				new {email=email,password=password}
			);
		
			if (response != null)
			{
				if (response.error != null) 
				{
					storage.Delete ("token");
					using(var alert = new UIAlertView("Login Failed", 
						string.Format("Please try again--{0}--Code:{1} ",
							response.error.message,response.error.code),null,"OK",null))
					{
						alert.Show ();
					}

					//we good
				}else if(response.Result != null){
					processResponse (response);
					result = true;

				}
			}
			return result;
		}

		//process response and cache 
		private void processResponse(APIResponse<LoginResult> result)
		{

			AccessToken = api.Token;
			storage.Put("token", api.Token);
		} 


	}
}

