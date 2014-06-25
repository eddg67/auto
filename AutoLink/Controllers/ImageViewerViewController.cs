using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using MonoTouch.ObjCRuntime;

namespace AutoLink
{
	public partial class ImageViewerViewController : UICollectionViewController
	{

		private ImageCollectionSource userSource;
		List<string> images { get; set; }
		static NSString headerId = new NSString ("Header");
		Listing item;

		public ImageViewerViewController(UICollectionViewLayout layout,Listing list) : base (layout)
			{
			item = list;
			images = item.images;

			}
	
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			NavigationController.NavigationBarHidden = false;
			NavigationController.ToolbarHidden = false;

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = item.title;

			userSource = new ImageCollectionSource(item);
			userSource.FontSize = 11f;
			userSource.ImageViewSize = new SizeF(100, 125);

			CollectionView.BackgroundColor = UIColor.Black;
			CollectionView.RegisterClassForCell (typeof(ImageCell), ImageCell.CellID);
			CollectionView.Source = userSource;

			//CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header,headerId );

				// add a custom menu item
				UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] { 
					new UIMenuItem ("Custom", new Selector ("custom:")) 
				};
		}

		public override void LoadView ()
		{
			base.LoadView ();
		
		}
			


	}

	public class Header : UICollectionReusableView
	{
		UIImageView label;

		public UIImage image {
			get {
				return label.Image;
			}
			set {
				label.Image = value;
			}
		}

		[Export ("initWithFrame:")]
		public Header (System.Drawing.RectangleF frame) : base (frame)
		{
			label = new UIImageView (){Frame = frame, BackgroundColor = UIColor.Yellow};

			AddSubview (label);
		}
	}

}
