using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace AutoLink.Models
{
	public class ActionSheetPicker
	{

		UIActionSheet actionSheet;
		UIButton doneButton = UIButton.FromType (UIButtonType.System);
		UIView owner;
		UILabel titleLabel = new UILabel ();
		public string Id;



		/// <summary>
		/// Set any datepicker properties here
		/// </summary>
		public UIPickerView Picker
		{
			get { return _picker; }
			set { _picker = value; }
		}
		UIPickerView _picker = new UIPickerView(RectangleF.Empty);

		public UIActionSheet ActionSheet
		{
			get { return actionSheet; }
			set { actionSheet = value; }
		}

		/// <summary>
		/// The title that shows up for the date picker
		/// </summary>
		public string Title
		{
			get { return titleLabel.Text; }
			set { titleLabel.Text = value; }
		}

		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public ActionSheetPicker (UIView owner)
		{

			UIWindow window = app.Window;
			// save our uiview owner
		
			this.owner = owner.Window != null?owner:window;
			
			owner.BackgroundColor = UIColor.White;


			// configure the title label
			titleLabel.BackgroundColor = UIColor.Clear;
			titleLabel.TextColor = UIColor.LightTextColor;
			titleLabel.Font = UIFont.BoldSystemFontOfSize (18);
			Picker.BackgroundColor = UIColor.Clear;
			Picker.SizeToFit ();

			// configure the done button
			doneButton.SetTitle ("Done", UIControlState.Normal);
			doneButton.TouchUpInside += (s, e) => { 
				actionSheet.DismissWithClickedButtonIndex (0, true); 
			};

			// create + configure the action sheet
			actionSheet = new UIActionSheet () { Style = UIActionSheetStyle.BlackOpaque };//Style = UIActionSheetStyle.BlackTranslucent
			actionSheet.Clicked += (s, e) => { Console.WriteLine ("Clicked on item {0}", e.ButtonIndex); };
			actionSheet.BackgroundColor = UIColor.Clear;

			// add our controls to the action sheet
			actionSheet.AddSubview (Picker);
			actionSheet.AddSubview (titleLabel);
			actionSheet.AddSubview (doneButton);

		}


		/// <summary>
		/// Shows the action sheet picker from the view that was set as the owner.
		/// </summary>
		public void Show (UIPickerViewModel model)
		{
			// declare vars
			float titleBarHeight = 40;
			SizeF doneButtonSize = new SizeF (71, 30);
			SizeF actionSheetSize = new SizeF (owner.Frame.Width, Picker.Frame.Height + titleBarHeight);
			RectangleF actionSheetFrame = new RectangleF (0, owner.Frame.Height - actionSheetSize.Height
				, actionSheetSize.Width, actionSheetSize.Height);

			// show the action sheet and add the controls to it
			actionSheet.ShowInView (owner);

			// resize the action sheet to fit our other stuff
			actionSheet.Frame = actionSheetFrame;

			// move our picker to be at the bottom of the actionsheet (view coords are relative to the action sheet)
			Picker.Frame = new RectangleF 
				(Picker.Frame.X, titleBarHeight, Picker.Frame.Width, Picker.Frame.Height);

			Picker.Model = model;

	

			// move our label to the top of the action sheet
			titleLabel.Frame = new RectangleF (10, 4, owner.Frame.Width - 100, 35);

			// move our button
			doneButton.Frame = new RectangleF (actionSheetSize.Width - doneButtonSize.Width - 10, 7, doneButtonSize.Width, doneButtonSize.Height);
		}

		/// <summary>
		/// Dismisses the action sheet date picker
		/// </summary>

		public void Hide (bool animated)
		{
			actionSheet.DismissWithClickedButtonIndex (0, animated);
		}
	}


}

