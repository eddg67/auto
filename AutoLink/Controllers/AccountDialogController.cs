
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using AutoLink.Models;
using AutoLink.Services;

namespace AutoLink
{
	public partial class AccountDialogController : DialogViewController
	{
		LoginResult result;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		AccountService service; 

		public AccountDialogController () : base (UITableViewStyle.Grouped, null)
		{
			Title = "Account Profile";
			service = app.accountService;
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

					if(!task.IsFaulted && task.Result.Result != null){
						result = task.Result.Result;

						var name = new EntryElement ("Your Name:", "Enter name", result.name);
						var email = new EntryElement ("Your Email:", "Enter email", result.email);
						var zip = new EntryElement ("Your Zip Code:", "Enter zip code",result.zip);
						var password = new EntryElement ("Password:", "Enter new password", string.Empty);
						var confirmPassword = new EntryElement ("Confirm Password:", "Enter new password", string.Empty);
						var notify = new BooleanElement ("Receive Auto Link emails?", result.emailNotifications);


						name.Changed += (object sender, EventArgs e) => {
							result.name = name.Value;
						};

						email.Changed += (object sender, EventArgs e) => {
							result.email = email.Value;
						};

						zip.Changed += (object sender, EventArgs e) => {
							result.zip = zip.Value;
						};

						password.Changed += (object sender, EventArgs e) => {
							result.password = password.Value;
						};

						notify.ValueChanged += (object sender, EventArgs e) => {
							result.emailNotifications = notify.Value;
						};
							
						var saveBtn = new StyledStringElement("Save"){
							Alignment = UITextAlignment.Center,
							TextColor = UIColor.Blue
						};

					

						saveBtn.Tapped += () => {
							service.UpdateAccount(result);
							NavigationController.PopViewControllerAnimated(true);

						};

						var deleteBtn = new StringElement("Delete"){
							Alignment = UITextAlignment.Center
						};

						deleteBtn.Tapped += () => {
							new UIAlertView("Delete Profile", "Delete Profile",null,"ok").Show();
						};

						Root = new RootElement ("Account") {
							new Section ("Account Settings") {
								name,email,zip,password,confirmPassword,notify,
								saveBtn,deleteBtn
							}
						};
					}

				}));

		}
	}
}
