using System;
using Alliance.Carousel;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace AutoLink
{
	public class ImageCarousel: UIViewController
	{
		public List<int> items;
		CarouselView carousel;
		public Listing item{ get; set; }

		public ImageCarousel(Listing model) : base()
		{
			item = model;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			items = Enumerable.Range(1, 100).ToList();
			BigTed.BTProgressHUD.Show ("Loading Image");
			View.DownloadImageAsync (item.images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {

				var imgView = new UIImageView(task.Result)
				{
					ContentMode = UIViewContentMode.ScaleToFill,
					AutoresizingMask = UIViewAutoresizing.All,
					Frame = View.Bounds
				};
				View.AddSubview(imgView);

				carousel = new CarouselView(new RectangleF(0,400,View.Bounds.Width,50));
				carousel.DataSource = new LinearDataSource(item);
				carousel.Delegate = new LinearDelegate();
				carousel.CarouselType = CarouselType.Linear;
				carousel.ConfigureView();
				View.AddSubview(carousel);
				carousel.Autoscroll = 1.0f;
				BigTed.BTProgressHUD.Dismiss ();
			}
			));

		
		}
	}

	public class LinearDelegate : CarouselViewDelegate
	{
		//UIViewController vc;

		public LinearDelegate()
		{

		}

		public override float ValueForOption(CarouselView carousel, CarouselOption option, float aValue)
		{
			if (option == CarouselOption.Spacing)
			{
				return aValue * 1.1f;
			}
			return aValue;
		}

		public override void DidSelectItem(CarouselView carousel, int index)
		{
			Console.WriteLine("Selected: " + ++index);
		}
	}
}

