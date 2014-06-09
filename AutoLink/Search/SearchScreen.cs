using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Collections.Generic;
using AutoLink.Models;
using System.Linq;

namespace AutoLink
{
	public partial class SearchScreen : UIViewController
	{

		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchForm form,form2,form3;
		SearchRequest postData;
		//UITextView title,header;
		UIBarButtonItem btnCancel,btnNext,btnSpace;
		private int stepOn = 1;
		//private ActionSheetPicker actionSheet;
		public Search searchModel = new Search ();

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SearchScreen () : base (UserInterfaceIdiomIsPhone ? "SearchScreen_iPhone" : "SearchScreen_iPad", null)
		{

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
			//RegisterForKeyboardNotifications ();
			InitLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			if (stepOn <= 1) {
				SearchStep1 ();
			}


				
		}

		private void SearchStep1()
		{
			form.RequestUpdated += (sender, e) => {
				var data = (PostDataEvent)e;
				updatePostData ((SearchRequest)data.Data);
			};
			form.Root = form.GetPage (1);

			SetToolBar (new UIBarButtonItem[] { btnCancel, btnSpace, btnNext });
			btnNext.Title = "Next";
			form.View.BackgroundColor = UIColor.White;

			this.AddChildViewController (form);

			form.TableView.ScrollsToTop = true;
			form.View.Tag = 1;

			View.AddSubview(form.View); 


		}

	
		private void SearchStep2()
		{
			if (form2 == null) {

				form2 = new SearchForm (null,form.searchReq);
				form2.RequestUpdated += (sender, e) => {
					var data = (PostDataEvent)e;
					updatePostData ((SearchRequest)data.Data);
					//var data = ()
				};

				form2.Root = form2.GetPage (2);
				btnCancel.Title = "Back";
				btnNext.Title = "Skip";
		
				this.AddChildViewController (form2);
				form2.View.Tag = 2;
			}
			this.Add (form2.View);
		
		}

		private void SearchStep3()
		{
			if (form3 == null) {
				form3 = new SearchForm (null, form.searchReq);

				form3.RequestUpdated += (sender, e) => {
					var data = (PostDataEvent)e;
					updatePostData ((SearchRequest)data.Data);
					//var data = ()
				};
		
				form3.Root = form3.GetPage (3);

				btnNext.Title = "Start Searching";

				this.AddChildViewController (form3);
				form3.View.Tag = 3;
			}
			this.Add (form3.View);

		}

		private void SetToolBar(UIBarButtonItem[] items)
		{
			if (items != null) {
				//toolBar.SetItems (items,false);
				this.NavigationController.NavigationBarHidden = true;
				this.NavigationController.ToolbarHidden = false;
				this.NavigationController.Toolbar.Layer.BorderWidth = 0f;
				this.ToolbarItems = items;
				//this.NavigationController.SetNavigationBarHidden (false, true);

			}
				
		}

		private UIPickerViewModel SetActionPickerModel <T>(List<T> list)
		{
			var model = new ListPicker<T> (list);

			//returns selected value 
			model.PickerChanged += (object sender, EventArgs e) => {
				var eve = (PickerChangedEventArgs)e;
				Console.WriteLine(eve.SelectedItem);
			};
				
			return model;
		}



		private List<CheckboxElement> GetCheckboxs(Dictionary<string,string> list=null,string group="checkboxes"){
			var result = new List<CheckboxElement> ();

			if (list != null) {
				result = list.Select (x => {
					int i= 0;
					i++;
					return new CheckboxElement(x.Key,(i>1)?false:true,group);
				}).ToList();
			}

			list = null;

			return result;
		}

		private void InitLoad()
		{

			form = new SearchForm (null);

			if (btnCancel == null) 
			{
				btnCancel = new UIBarButtonItem ();
				btnCancel.Title = "Cancel";
				btnCancel.TintColor = UIColor.Gray;
				btnCancel.Clicked += (sender, e) => {

					stepOn--;

					if(stepOn > 0){
						if(stepOn == 1){
							SearchStep1();
						}else if(stepOn == 2){
							SearchStep2();
						}else{
							SearchStep3();
						}
							
					}else{
						app.RootController.ToolbarHidden = true;
						app.RootController.PopViewControllerAnimated(true);
					}
	
				};

				btnSpace = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);

			}

			if (btnNext == null) 
			{

				btnNext = new UIBarButtonItem ();
				btnNext.Title = "Next";
				btnNext.Clicked += (sender, e) => {

					//postData
					if(btnNext.Title.Contains("Searching")){
						//var v = View.FindFirstResponder();
						Console.Write(postData);
						form.PostSearch(postData);

						app.ShowResultList();


					}else{
						stepOn++;
						if(stepOn ==2){
							SearchStep2();
						}else if(stepOn ==3){
							SearchStep3();
						}else{
							SearchStep1();
							stepOn =1;
						}
					}
						
				};

			}
				
		}

		private void updatePostData(SearchRequest req)
		{
			postData = req;
			form.searchReq = req;
		}

	}
}

