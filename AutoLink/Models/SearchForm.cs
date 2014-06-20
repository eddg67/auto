using System;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;
using AutoLink.Services;
using BigTed;


namespace AutoLink.Models
{
	public class SearchForm: DialogViewController
	{
		public EntryElement Name { get; set; }
		public StyledStringElement Type { get; set; }

		public StyledStringElement YearTo { get; set; }
		public StyledStringElement YearFrom { get; set; }
		public StyledStringElement Make { get; set; }
		public StyledStringElement Model { get; set; }
		public StyledStringElement PriceFrom { get; set; }
		public StyledStringElement PriceTo { get; set; }
		public StyledStringElement Distance { get; set; }
		public StyledStringElement Trim { get; set; }
		public StyledStringElement Exterior { get; set; }
		public StyledStringElement Interior { get; set; }

		public StyledStringElement MileageTo { get; set; }
		public StyledStringElement MileageFrom { get; set; }

		public StyledStringElement Seller { get; set; }

		public BooleanElement SearchAuctions { get; set; }
		public BooleanElement SendNotifications { get; set; }

		public EntryElement Zip { get; set; }

		public ActionSheetPicker actionSheet;
		public SearchService service;
		public SearchRequest searchReq { get; set; }
		public event EventHandler RequestUpdated;

		List<string> yearsList;
		List<string> maxPriceList;
		List<string> minPriceList;
		List<string> makeList;
		List<string> modelList;
		List<string> colors;
		List<string> minDistanceList;
		List<string> maxDistanceList;



		#region Header/Footer

		UITextView TitleHeader;
		UITextView BodyHeader;
		UITextView TitleFoot;
		UITextView BodyFoot;
		UIView Header;
		UIView Footer;

		#endregion


		public SearchForm (RootElement _root,SearchRequest req = null):base(new RootElement("Root"), true)
		{
			Root =(_root != null) ? _root : GetRoot("root") ;
			searchReq = (req == null) ? new SearchRequest () : req;
	
			AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

			TableView.AutoresizingMask = UIViewAutoresizing.None;
			service = app.searchService;
			yearsList = service.GetYearList ();
			maxPriceList = service.GetMaxPricelist ();
			minPriceList = service.GetMinPricelist ();
			minDistanceList = service.GetMinDistancelist ();
			maxDistanceList = service.GetMaxDistancelist ();
			maxPriceList = service.GetMaxDistancelist ();

			//TODO add to storage
			if (colors == null) {

				colors = service.GetColors ();
				//colors = task.Result;
			}

			Name = new EntryElement ("Name", "Create Search Name", string.Empty);
			Name.ReturnKeyType = UIReturnKeyType.Done;
			Name.ShouldReturn += () => {
				Name.ResignFirstResponder(true);
				return true;
			};
			//Name.ShouldReturn
			Name.EntryEnded += (s,e) => {
				searchReq.name = Name.Value;
				//this.View.EndEditing(true);
			};
		
			Type = GetSearchType ();
			Model = GetCarModel ();
			Make = GetCarMake ();
			Zip = new EntryElement ("Zip", "Postal Code",string.Empty);
			Distance = GetDistance ();
			Trim = GetTrim ();
			Exterior = GetExterior ();
			Interior = GetInterior ();
			Seller = GetSeller ();
			SearchAuctions = new BooleanElement ("Search Auctions", true);
			SendNotifications = new BooleanElement ("Send Notifications", true);

			SendNotifications.ValueChanged += (sender, e) => {
				searchReq.notify = SendNotifications.Value;
				if (this.RequestUpdated != null)
				{

					this.RequestUpdated(this, new PostDataEvent{Data=searchReq});

				}
			};

			SearchAuctions.ValueChanged += (sender, e) => {
				searchReq.auctions = SearchAuctions.Value;
				if (this.RequestUpdated != null)
				{
					this.RequestUpdated(this, new PostDataEvent{Data=searchReq});
				}
			};


			//header footer view for sections
			Header = GetHeader ();
			Footer = GetFooter ();

			View.BackgroundColor = UIColor.White;
		}

		public RootElement GetPage(int pg=1)
		{
			RootElement result = null;
			switch (pg) {
			case 1:
				result = Page1 ();
				break;
			case 2:
				result = Page2 ();
				break;
			case 3:
				result = Page3 ();
				break;

			}
			return result;
		}

		#region Header/Footer

		public UIView GetHeader ()
		{

			TitleHeader = new UITextView (new RectangleF (
				0, 
				10, 
				UIScreen.MainScreen.Bounds.Width, 
				50 
			)) {
				TextAlignment = UITextAlignment.Center,
				Text = @"New live search",
				Editable = false,
				AutosizesSubviews = true,
				Font = UIFont.FromName ("KannadaSangamMN-Bold", 30f)

			}; 

			TitleHeader.ContentInset = new UIEdgeInsets (10, 0, 0, 0);

			BodyHeader = new UITextView (new RectangleF (
				20, 
				60, 
				UIScreen.MainScreen.Bounds.Width - 40, 
				100 
			)) {
				TextAlignment = UITextAlignment.Center,
				Text = @"New live searches are always working in the background, you dont have to.",
				Editable = false,
				AutosizesSubviews = true,
				Font = UIFont.FromName ("KannadaSangamMN", 12)

			};

			var hvHght = TitleHeader.Frame.Height + BodyHeader.Frame.Height + 10;
			Header = new UIView (new RectangleF (0, 0, View.Frame.Width - 20, hvHght));
			Header.Add (TitleHeader);
			Header.Add (BodyHeader);
			return Header;
			
		}

		public UIView GetFooter ()
		{

			TitleFoot = new UITextView (new RectangleF (
				0, 
				10, 
				UIScreen.MainScreen.Bounds.Width, 
				50 
			)) {
				TextAlignment = UITextAlignment.Left,
				Text = @"Note",
				Editable = false,
				AutosizesSubviews = true,
				Font = UIFont.FromName ("KannadaSangamMN", 20f)

			}; 

			BodyFoot = new UITextView (new RectangleF (
				20, 
				60, 
				UIScreen.MainScreen.Bounds.Width - 40, 
				100 
			)) {
				TextAlignment = UITextAlignment.Left,
				Text = @"You can always add an advance search after a live search is created.",
				Editable = false,
				AutosizesSubviews = true,
				Font = UIFont.FromName ("Oriya Sangam MN", 12)

			};

			var hvHght = TitleFoot.Frame.Height + BodyFoot.Frame.Height + 10;
			Footer = new UIView (new RectangleF (0, 0, View.Frame.Width - 20, hvHght));
			Footer.Add (TitleFoot);
			Footer.Add (BodyFoot);

			return Footer;

		}

		#endregion

		public void PostSearch(SearchRequest req)
		{
			BTProgressHUD.Show ("Search Millions of Records...");
			service.PostData (req).ContinueWith((task) => InvokeOnMainThread(() =>
				{
					BTProgressHUD.Dismiss ();
				}
			));
		}


		private RootElement GetRoot(string caption)
		{
			return new RootElement (caption);
		}
	
		private RootElement Page1()
		{
			var first = new Section () {
				Name,
				Type
			};

			first.HeaderView = Header;

			Root = new RootElement ("New Live Search") { 
					first,
					GetYear(),
				new Section("Make and Model"){
					Make,Model
				},
				GetPrice(),
				new Section("Distance"){
					Distance,Zip
				}

			};
		
			return Root;

		}

		private RootElement Page2()
		{
			var first = new Section () {
				Trim, Exterior, Interior
			};
			TitleHeader.Text = @"Advanced";
			BodyHeader.Text =  @"Adding advance criteria may limit your results";
			first.HeaderView = Header;

			Root = new RootElement ("Advanced") { 
				first,
				GetMileage(),
				new Section("Seller"){
					Seller,
					new UIViewElement("",Footer,true)
				}

			};

			return Root;

		}

		private RootElement Page3()
		{
			var first = new Section () {

			};
			var auctions = new Section ("Search Auctions"){ 
				SearchAuctions
			};
			var notifications = new Section ("Notifications") {
				SendNotifications
			};
			TitleHeader.Text = @"Search Options";
			BodyHeader.Text =  @"Adjust settings for this live search.";
			first.HeaderView = Header;

			Root = new RootElement ("New Live Search") { 
				first,auctions,notifications
			};

			return Root;

		}

		StyledStringElement GetTrim ()
		{
			var el = new StyledStringElement ("Trim", "All Trims", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet("Trim",el,
					new List<string> {
					"All Trims"
				});

			};

			return el;
		}

		StyledStringElement GetExterior ()
		{
			var el = new StyledStringElement ("Exterior", "All Colors", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet("Exterior",el,colors);

			};

			return el;
		}

		StyledStringElement GetInterior ()
		{
			var el = new StyledStringElement ("Interior", "All Interior", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet("Interior",el,colors);

			};

			return el;
		}

		private Section GetMileage()
		{
			MileageTo = new StyledStringElement ("To","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};


			MileageTo.Tapped += () => {
				SetActionSheet("MileageTo",MileageTo,maxDistanceList);
			
			};

			MileageFrom = new StyledStringElement ("From","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			MileageFrom.Tapped += () => {
				SetActionSheet("MileageFrom",MileageFrom,minDistanceList);
			};


			return new Section ("Mileage") {
				MileageTo,
				MileageFrom
			};


		}

		//TODO create custom extention of section
		//http://fastchicken.co.nz/2012/05/20/earnest-debrief-visual-styles-in-ios-apps-uiappearence-custom-sections-in-monotouch-dialog/
		private Section GetYear()
		{
		
			YearTo = new StyledStringElement ("To","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};


			YearTo.Tapped += () => {
				SetActionSheet("YearTo",YearTo,yearsList);
			};

			YearFrom = new StyledStringElement ("From","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			YearFrom.Tapped += () => {
				SetActionSheet("YearFrom",YearFrom,yearsList);
			};


			return new Section ("Year") {
				YearFrom,YearTo

			};


		}

		//TODO create custom extention of section
		//http://fastchicken.co.nz/2012/05/20/earnest-debrief-visual-styles-in-ios-apps-uiappearence-custom-sections-in-monotouch-dialog/
		private Section GetPrice()
		{
			PriceFrom = new StyledStringElement ("From","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};


			PriceFrom.Tapped += () => {
				SetActionSheet("PriceFrom",PriceFrom,minPriceList);
			};
				

			PriceTo = new StyledStringElement ("To","All",UITableViewCellStyle.Value1) 
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			PriceTo.Tapped += () => {
				SetActionSheet("PriceTo",PriceTo,minPriceList);
			};


			return new Section ("Price Range") {
				PriceFrom,PriceTo
			};

		

		}

		private StyledStringElement GetCarModel()
		{
			var el = new StyledStringElement ("Model", "Choose", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet("Model",el,modelList);

			};

			return el;
		}

		private StyledStringElement GetSeller()
		{
			var el = new StyledStringElement ("Seller", "All Sellers", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet(
					"Seller", 
					el, 
					new List<string> () 
					{"All Sellers","Cars.com","Craiglist","Ebay","Hemmings","Oodle"}
				);

			};

			return el;
		}


		private StyledStringElement GetCarMake()
		{
			var el = new StyledStringElement ("Make", "Choose", UITableViewCellStyle.Value1)
			{
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};

			el.Tapped += ()=>{
				SetActionSheet("Make", el,makeList);
	
			};

			return el;
		}

		//TODO 
		private StyledStringElement GetSearchType()
		{
			var str = new StyledStringElement ("Type", "All", UITableViewCellStyle.Value1);
			str.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			str.Tapped += () => {
				SetActionSheet( "Type", str,new List<string> () {"All", "New", "Used"});
			};

			return str;
		}

		//TODO 
		private StyledStringElement GetDistance()
		{
			var str = new StyledStringElement ("Distance", "All miles", UITableViewCellStyle.Value1);
			str.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			var list = new List<string> () {
				"All", "5 miles", "15 miles", "30 miles",
				"50 miles", "100 miles", "200 miles",
				"400 miles", "600 miles", "800 miles", "1000 miles", "2000 miles"
			};
			str.Tapped += () => {
				SetActionSheet( "Distance",str,list);
			};

			return str;
		}

		private StringElement GetDoneButton()
		{
			StringElement result = new StringElement ("Done", () => {
				this.ParentViewController.DismissViewController(true,()=>{
				});
			});


			return result;
		}

		private void SetActionSheet(string id,StyledStringElement str, List<string> list)
		{
			actionSheet = new ActionSheetPicker(View);
			actionSheet.Id = id;
			actionSheet.ActionSheet.Canceled += (object sender, EventArgs e) => {
				var p = actionSheet.Picker;
				var listPicker = (ListPicker<string>)p.Model;
				str.Value = listPicker.SelectedItem;

			};

			actionSheet.Show(SetActionPickerModel(list));
		}

		private UIPickerViewModel SetActionPickerModel <T>(List<T> list)
		{
			var model = new ListPicker<T> (list);

			//returns selected value 
			model.PickerChanged += (object sender, EventArgs e) => {
				var eve = (PickerChangedEventArgs)e;
				if(eve.SelectedItem != null){
					UpdateValueOnSelect(eve.SelectedItem.ToString());
				}
				//sender
				actionSheet.Hide(true);
			
			};

			return model;
		}

		private void UpdateValueOnSelect(string val)
		{
			StyledStringElement str = null;


			switch (actionSheet.Id) {
			case "Seller":
				str = Seller;
				searchReq.seller = str.Value;
				break;
			case "Type":
				str = Type;
				searchReq.type = val;
				break;
			case "Model":
				str = Model;
				searchReq.model = val;
				break;
			case "Make":
				Make.Value = val;
				UpdateModels ();
				str = Make;
				searchReq.make = val;
				break;
			case "Distance":
				str = Distance;
				val = val.Replace ("miles", string.Empty).Trim();
				int.TryParse(val, out searchReq.distance);
				break;
			case "Trim":
				Trim.Value = val;
				str = Trim;
				searchReq.trim = val;
				break;
			case "Exterior":
				str = Exterior;
				searchReq.exteriorColor = val;
				break;
			case "Interior":
				str = Interior;
				searchReq.interiorColor = val;
				break;
			case "MileageTo":
				MileageTo.Value = val;
				searchReq.mileage = searchReq.mileage == null ? new Range () : searchReq.mileage;
				int.TryParse(val, out searchReq.mileage.min );
				MileageTo.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (MileageTo, UITableViewRowAnimation.Automatic);
				break;
			case "MileageFrom":
				MileageFrom.Value = val;
				searchReq.mileage = searchReq.mileage == null ? new Range () : searchReq.mileage;
				int.TryParse(val, out searchReq.mileage.max);
				MileageFrom.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (MileageFrom, UITableViewRowAnimation.Automatic);
				break;
			case "PriceTo":
				PriceTo.Value = val;
				searchReq.price = searchReq.price == null ? new Range () : searchReq.price;
				val = val.Replace ("$", string.Empty);
				int.TryParse(val, out searchReq.price.min);
				PriceTo.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (PriceTo, UITableViewRowAnimation.Automatic);
				break;
			case "PriceFrom":
				PriceFrom.Value = val;
				searchReq.price = searchReq.price == null ? new Range () : searchReq.price;
				val = val.Replace ("$", string.Empty);
				int.TryParse(val, out searchReq.price.max);
				PriceFrom.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (PriceFrom, UITableViewRowAnimation.Automatic);
				break;
			case "YearTo":
				YearTo.Value = val;
				UpdateMakes ();
				searchReq.years = searchReq.years == null ? new Range () : searchReq.years;
				int.TryParse (val, out searchReq.years.min);
				YearTo.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (YearTo, UITableViewRowAnimation.Automatic);
				break;
			case "YearFrom":
				YearFrom.Value = val;
				UpdateMakes ();
				searchReq.years = searchReq.years == null ? new Range () : searchReq.years;
				int.TryParse(val, out searchReq.years.max);
				YearFrom.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (YearFrom, UITableViewRowAnimation.Automatic);
				break;
	
		
			}

			//update
			if (str != null) {
				str.Value = val;
				str.Accessory = UITableViewCellAccessory.Checkmark;
				Root.Reload (str, UITableViewRowAnimation.Automatic);
			}

			if (this.RequestUpdated != null)
			{

				this.RequestUpdated(this, new PostDataEvent{Data=searchReq});

			}
				
		}




		private void UpdateMakes()
		{
			var max = YearFrom.Value == "All" ? string.Empty : YearFrom.Value;
			var min = YearTo.Value == "All" ? string.Empty : YearTo.Value;

			//var result = await service.api.CreateAsync<List<string>> (@"vehicles.listMakes", new {max = max,min = min});
			//makeList = result.Result;

			makeList = service.GetMakes (max, min);

		}


		private void UpdateModels()
		{
			var model = Make.Value == "Choose" ? string.Empty : Make.Value;

			modelList = service.GetModels (model);

		}

		private void UpdateTrims()
		{

		}


	

	}

	public class PostDataEvent : EventArgs{
		public object Data {get;set;}
	}
}

