using System;
using System.Linq;
using MonoTouch.UIKit;
using AutoLink.Models;
using MonoTouch.Dialog;

namespace AutoLink
{
	public class PriceTabController : UITabBarController {

		UIViewController tab1, tab2, tab3;

		public PriceTabController (Pricing price)
		{
			if (price.@private.Count == 3) {
			}


			var rootPrivate = new RootElement ("private") {
				new Section("Private"){
					new StyledStringElement("Fair",string.Format("${0}",price.@private[0].ToString())),
					new StyledStringElement("Good",string.Format("${0}",price.@private[1].ToString())),
					new StyledStringElement("Excellent",string.Format("${0}",price.@private[2].ToString())),
				}
			};

			var rootRetail = new RootElement ("retail") {
				new Section("Retail Price"){
					new StyledStringElement("Fair",string.Format("${0}",price.retail[0].ToString())),
					new StyledStringElement("Good",string.Format("${0}",price.retail[1].ToString())),
					new StyledStringElement("Excellent",string.Format("${0}",price.retail[2].ToString())),
				}

			};

			var rootTrade = new RootElement ("trade") {
				new Section("Trade in Price"){
					new StyledStringElement("Fair",string.Format("${0}",price.trade[0].ToString())),
					new StyledStringElement("Good",string.Format("${0}",price.trade[1].ToString())),
					new StyledStringElement("Excellent",string.Format("${0}",price.trade[2].ToString())),
				}

			};

			tab1 = new DialogViewController(rootPrivate);
			tab1.Title = "Private";
	

			tab2 = new DialogViewController(rootRetail);
			tab2.Title = "Retail";
	

			tab3 = new DialogViewController(rootTrade);
			tab3.Title = "Trade In";

			var tabs = new UIViewController[] {
				tab1, tab2, tab3
			};

			ViewControllers = tabs;

		}
	}
}