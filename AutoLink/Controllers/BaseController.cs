using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	/// <summary>
	/// Base controller for all controllers in application
	/// </summary>
	public abstract class BaseController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public static string XIB_pad{ get; set; }
		public static string XIB_phone{ get; set; }

		public static UIView activeview{ get; set; }            // Controller that activated the keyboard
		static float scrollamount = 0.0f;    // amount to scroll 
		static float bottom = 0.0f;           // bottom point
		static float offset = 10.0f;          // extra offset
		static bool moveViewUp { get; set; }

		/// <summary>
		/// Constructor for use when controller is not in a storyboard
		/// </summary>

		public BaseController ()
			: base (UserInterfaceIdiomIsPhone ? XIB_phone: XIB_pad, null)
		{
			moveViewUp = false;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			/*if (Theme.IsiOS7) {
				EdgesForExtendedLayout = UIRectEdge.None;
			}*/
		}

		/// <summary>
		/// Required constructor for Storyboard to work
		/// </summary>
		/// <param name='handle'>
		/// Handle to Obj-C instance of object
		/// </param>
		public BaseController (IntPtr handle) : base (handle)
		{
			//Only do this if required
			if (HandlesKeyboardNotifications) {
				NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
				NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
			}
		}

		public virtual bool HandlesKeyboardNotifications
		{
			get { return false; }
		}

		public void RegisterForKeyboardNotifications()
		{
			activeview = this.PresentedViewController == null ? 
						 this.NavigationController.TopViewController.View
						 :this.PresentedViewController.View;

			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, KeyBoardDownNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		/// <summary>
		/// This is how orientation is setup on iOS 6
		/// </summary>
		public override bool ShouldAutorotate ()
		{
			return true;
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
				frame.Y += scrollamount;
				scrollamount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}
	}
}

