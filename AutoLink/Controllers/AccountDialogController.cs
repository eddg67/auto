
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using AutoLink.Models;

namespace AutoLink
{
	public partial class AccountDialogController : DialogViewController
	{
		LoginResult result;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public AccountDialogController () : base (UITableViewStyle.Grouped, null)
		{
			Title = "Account Profile";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadData ();
			TableView.BackgroundColor = UIColor.White;


		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NavigationController.NavigationBarHidden = true;
			NavigationController.ToolbarHidden = true;
		}

		public void LoadData(){

			app.accountService.GetAccount ().ContinueWith (
				(task) => InvokeOnMainThread (() => {

					if(task.Result.Result != null){
						result = task.Result.Result;

						var saveBtn = new StringElement("Save"){
							Alignment = UITextAlignment.Center
						};

						saveBtn.Tapped += () => {
							new UIAlertView("Save Profile", "Profile Updates",null,"ok").Show();
						};

						var deleteBtn = new StringElement("Delete"){
							Alignment = UITextAlignment.Center
						};

						deleteBtn.Tapped += () => {
							new UIAlertView("Delete Profile", "Delete Profile",null,"ok").Show();
						};


						Root = new RootElement ("Account") {
							new Section ("Account Settings") {
								new EntryElement ("Your Name:", "Enter name", result.name){

								},
								new EntryElement ("Your Email:", "Enter email", result.email),
								new EntryElement ("Your Zip Code:", "Enter zip code", result.zip),
								new EntryElement ("Password:", "Enter new password", string.Empty),
								new EntryElement ("Confirm Password:", "Enter new password", string.Empty),
								new BooleanElement ("Receive Auto Link emails?", result.emailNotifications),
								saveBtn,deleteBtn
							}
						};
					}

				}));

		}
	}
}
