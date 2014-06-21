using System;
using System.Linq;
using MonoTouch.UIKit;
using AutoLink.Models;
using MonoTouch.Dialog;

namespace AutoLink
{
	public class PriceTabController : UITabBarController {

		DialogViewController tab1, tab2, tab3;
		Pricing price;

		public PriceTabController (Pricing _price)
		{
			price = _price;

			var rootPrivate = new RootElement ("Private") {
				new Section("Private","Prices From Edmunds. Used for Calculations"){
					new StyledStringElement("Fair",formatPrice(price.@private, 0)),
					new StyledStringElement("Good",formatPrice(price.@private, 1)),
					new StyledStringElement("Excellent",formatPrice(price.@private, 2))
				}
			};

			var rootRetail = new RootElement ("Retail") {
				new Section("Retail Price","Prices From Edmunds. Used for Calculations"){
					new StyledStringElement("Fair",formatPrice(price.retail, 0)),
					new StyledStringElement("Good",formatPrice(price.retail, 1)),
					new StyledStringElement("Excellent",formatPrice(price.retail, 2))
				}

			};

			var rootTrade = new RootElement ("Trade") {
				new Section("Trade in Price","Prices From Edmunds. Used for Calculations"){
					new StyledStringElement("Fair",formatPrice(price.trade, 0)),//string.Format("${0}",price.trade[0].ToString())),
					new StyledStringElement("Good",formatPrice(price.trade, 1)),//string.Format("${0}",price.trade[1].ToString())),
					new StyledStringElement("Excellent",formatPrice(price.trade, 2))//string.Format("${0}",price.trade[2].ToString())),
				}

			};

			tab1 = new DialogViewController(rootPrivate);
			tab1.Title = "Private";
			tab1.TableView.BackgroundColor = UIColor.White;
			tab1.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, (s, e) => {
				NavigationController.NavigationBarHidden = false;
				NavigationController.ToolbarHidden = false;
				NavigationController.PopViewControllerAnimated(true);
			});

			var tab1Nav = new UINavigationController (tab1);
		
		
			tab2 = new DialogViewController(rootRetail);
			tab2.Title = "Retail";
			tab2.TableView.BackgroundColor = UIColor.White;
			tab2.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, (s, e) => {
				NavigationController.NavigationBarHidden = false;
				NavigationController.ToolbarHidden = false;
				NavigationController.PopViewControllerAnimated(true);
			});
			var tab2Nav = new UINavigationController (tab2);
	

			tab3 = new DialogViewController(rootTrade);
			tab3.Title = "Trade In";
			tab3.TableView.BackgroundColor = UIColor.White;
			tab3.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, (s, e) => {
				NavigationController.NavigationBarHidden = false;
				NavigationController.ToolbarHidden = false;
				NavigationController.PopViewControllerAnimated(true);
			});
			var tab3Nav = new UINavigationController (tab3);

			var tabs = new UIViewController[] {
				tab1Nav, tab2Nav, tab3Nav
			};

			ViewControllers = tabs;

		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

		}

		string formatPrice(string[] list, int index)
		{
			string result = string.Empty;

			if(list.Length > index){
				result = string.Format ("${0}", list[index].ToString ());

			}
			return result;
		}
	}
}