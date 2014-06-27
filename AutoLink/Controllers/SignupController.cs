using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Services;
using BigTed;

namespace AutoLink
{
	public partial class SignupController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		LoginService loginService;
		AutoLink.Utilities.Validator validate;
	
		public static UIView activeview{ get; set; }            // Controller that activated the keyboard
		static float scrollamount = 0.0f;    // amount to scroll 
		static float bottom = 0.0f;           // bottom point
		static float offset = 5.0f; 
		RectangleF startBounds {  get; set; }
		static bool moveViewUp { get; set; }

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SignupController ()
			: base (UserInterfaceIdiomIsPhone ? "SignupController_iPhone" : "SignupController_iPad", null)
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
			this.btnSubmit.Layer.BorderWidth = 1.0f;
			this.btnSubmit.Layer.BorderColor = UIColor.White.CGColor;

			handleBtn ();
			handleTxt ();
			startBounds = View.Bounds;
			activeview = View;

			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public virtual bool HandlesKeyboardNotifications
		{
			get { return false; }
		}


		/// <summary>
		/// This is how orientation is setup on iOS 6
		/// </summary>
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		private void OnKeyboardNotification (NSNotification notification)
		{
			if (IsViewLoaded) {

				//Check if the keyboard is becoming visible
				bool visible = notification.Name == UIKeyboard.WillShowNotification;

				//Start an animation, using values from the keyboard
				UIView.BeginAnimations ("AnimateForKeyboard");
				UIView.SetAnimationBeginsFromCurrentState (true);
				UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
				UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

				//Pass the notification, calculating keyboard height, etc.
				bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
				if (visible) {
					var keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);

					OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				} else {
					var keyboardFrame = UIKeyboard.FrameBeginFromNotification (notification);

					OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				}

				//Commit the animation
				UIView.CommitAnimations ();	
			}
		}

		/// <summary>
		/// Override this method to apply custom logic when the keyboard is shown/hidden
		/// </summary>
		/// <param name='visible'>
		/// If the keyboard is visible
		/// </param>
		/// <param name='height'>
		/// Calculated height of the keyboard (width not generally needed here)
		/// </param>
		protected virtual void OnKeyboardChanged (bool visible, float height)
		{

			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews) {
				if (view.IsFirstResponder)
					activeview = view;
			}

			// Bottom of the controller = initial position + height + offset      
			bottom = (activeview.Frame.Y + activeview.Frame.Height + offset);

			// Calculate how far we need to scroll
			scrollamount = (height - (View.Frame.Size.Height - bottom)) ;
			if (visible) {
				// Perform the scrolling
				if (scrollamount > 0) {
					moveViewUp = true;
					ScrollTheView (moveViewUp);
				} else {
					moveViewUp = false;
					ScrollTheView (moveViewUp);
				}
			}

		}

		public void KeyBoardDownNotification(NSNotification notification)
		{
			if(moveViewUp){ScrollTheView(false);}
		}

		public void ScrollTheView(bool move)
		{

			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);

			RectangleF frame = View.Frame;

			if (move) {
				frame.Y -= scrollamount;
			} else {
				frame = startBounds;
				scrollamount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}

		private void handleBtn()
		{
		

			this.btnClose.TouchUpInside += (sender, e) => {
				app.ShowLogin();
			};

			this.btnSubmit.TouchUpInside += (sender, e) => {
				BTProgressHUD.Show ("Signing You Up...");
				signUp();
			};
			//10f, 145f, 141f, 48f
			//150f, 145f, 141f, 48f
			View.AddSubview( loginService.CreateFaceBookBtn (10f, 130f, 130f, 48f));
			View.AddSubview( loginService.CreateGooglePlusBtn (150f, 130f, 130f, 48f));
		}


		private void handleTxt()
		{
			this.txtEmail.AllTouchEvents += (object sender, EventArgs e) => {
				txtEmail.AllowsEditingTextAttributes = true;
			};

			//this.txtEmail.ShouldEndEditing = true;
			this.txtEmail.EditingDidEndOnExit += (sender, e) => {
				this.txtEmail.EndEditing (true);
				ScrollTheView(false);
			};


			this.txtPassword.EditingDidEndOnExit += (sender, e) => {
				this.txtPassword.EndEditing (true);
				ScrollTheView(false);
			};


			//this.txtEmail.ShouldEndEditing = true;
			this.txtName.EditingDidEndOnExit += (sender, e) => {
				this.txtName.EndEditing (true);
				ScrollTheView(false);
			};

			this.txtZip.EditingDidEndOnExit += (sender, e) => {
				this.txtZip.EndEditing (true);
				ScrollTheView(false);
			};

			this.txtPassword.EditingDidEndOnExit += (sender, e) => {
				this.txtPassword.EndEditing (true);
				ScrollTheView(false);
			};

			this.txtEmail.AttributedPlaceholder = new NSAttributedString ("   Email", null, UIColor.White, null, UIColor.White, null);
			this.txtPassword.AttributedPlaceholder = new NSAttributedString ("   Password", null, UIColor.White, null, UIColor.White, null);
			this.txtName.AttributedPlaceholder = new NSAttributedString ("   Name", null, UIColor.White, null, UIColor.White, null);
			this.txtZip.AttributedPlaceholder = new NSAttributedString ("   Zip", null, UIColor.White, null, UIColor.White, null);

			this.txtEmail.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .5f);
			this.txtPassword.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .5f);
			this.txtName.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .5f);
			this.txtZip.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .5f);

		}



		private void signUp()
		{

			if (validate.isEmail(txtEmail)
				&& !validate.isEmptyTxt (txtName)
				&& !validate.isEmptyTxt (txtPassword)
				&& !validate.isEmptyTxt (txtZip)) {

				if (post ()) {
					//go to search start fresh signup
					app.ShowSearch ();
				} 

				BTProgressHUD.Dismiss ();

			} else {
				BTProgressHUD.Dismiss ();
			}
		}


		private bool post()
		{
			bool result = false;
			if (loginService.signUp (txtName.Text, txtEmail.Text, txtPassword.Text)) {
				result = true;
			}

			return result;
		}


		private void KeyBoardUpNotification(NSNotification notification) {

			//get size of keyboard
			var val = new NSValue(notification.UserInfo.ValueForKey(UIKeyboard.FrameBeginUserInfoKey).Handle);
			RectangleF r = val.RectangleFValue;

			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews) {
				if (view.IsFirstResponder)
					activeview = view;
			}

			// Bottom of the controller = initial position + height + offset      
			bottom = (activeview.Frame.Y + activeview.Frame.Height + offset);

			// Calculate how far we need to scroll
			scrollamount = (r.Height - (View.Frame.Size.Height - bottom)) ;
			scrollamount = -(scrollamount);
			// Perform the scrolling
			if (scrollamount > 0) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);
			} else {
				moveViewUp = false;
			}

		}

	}
}

