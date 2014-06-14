using System;
using Alliance.Carousel;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Drawing;

namespace AutoLink
{
	public class LinearDataSource : CarouselViewDataSource
	{
		Listing item;
		const float indentX = 50;
		const float indentY = 10;
		const float imageHeight = 100;

		UIViewFullscreen vMain;

		UILabel lBigImage;
		UIImageViewClickable ivcThumbnail1;

		UILabel lSmallImage;
		UIImageViewClickable ivcThumbnail2;

		public LinearDataSource(Listing list)
		{
			this.item = list;
		}

		public override uint NumberOfItems(CarouselView carousel)
		{
			return (uint)item.images.Count;
		}

		public override UIView ViewForItem(CarouselView carousel, uint index, UIView reusingView)
		{

			UILabel label;

			// create new view if no view is available for recycling
			if (reusingView == null)
			{
				// don't do anything specific to the index within
				// this `if (view == null) {...}` statement because the view will be
				// recycled and used with other index values later
				var imgView = new UIImageViewClickable()
				{
					Frame = new RectangleF(0, 0, 200, 200),
					ContentMode = UIViewContentMode.Center
				};

				imgView.DownloadImageAsync (item.images[Convert.ToInt32(index)]).ContinueWith ((task) => InvokeOnMainThread (() => {
					//DetailTextLabel.Text = list.description;
					if (!task.IsFaulted) {
						imgView.Image = task.Result;
					
						imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
						imgView.AutoresizingMask = UIViewAutoresizing.All;
						imgView.OnClick += () => {
							if (vMain == null) {
								vMain = new UIViewFullscreen ();
							}
							vMain.SetImage (ivcThumbnail2.Image);
							vMain.Show ();
						};

						imgView.Frame = new RectangleF (indentX, lSmallImage.Frame.Bottom + indentY, 
							reusingView.Frame.Width - indentX * 2, imageHeight);

						reusingView.AddSubview (ivcThumbnail2);


					}

				}));

				label = new UILabel(imgView.Bounds)
				{
					BackgroundColor = UIColor.Clear,
					TextAlignment = UITextAlignment.Center,
					Tag = 1
				};
				label.Font = label.Font.WithSize(50);
				imgView.AddSubview(label);
				reusingView = imgView;
			}
			else
			{
				// get a reference to the label in the recycled view
				label = (UILabel)reusingView.ViewWithTag(1);
			}

			// set item label
			// remember to always set any properties of your carousel item
			// views outside of the `if (view == null) {...}` check otherwise
			// you'll get weird issues with carousel item content appearing
			// in the wrong place in the carousel
			//label.Text = vc.items[(int)index].ToString();

			return reusingView;
		}
	}

	public class LinearDelegate : CarouselViewDelegate
	{
		UIViewController vc;

		public LinearDelegate(UIViewController vc)
		{
			this.vc = vc;
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

