﻿using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	public class UIScrollViewImage: UIScrollView
	{
		/// <summary>
		/// The double tap interval. Measured in ms
		/// </summary>

		const float defaultZoom = 2f;
		const float minZoom = 0.1f;
		const float maxZoom = 3f;
		float sizeToFitZoom = 1f;

		UIImageView ivMain;

		UITapGestureRecognizer grTap;
		UITapGestureRecognizer grDoubleTap;

		public event Action OnSingleTap;

		public UIScrollViewImage ()
		{
			AutoresizingMask = UIViewAutoresizing.All;

			ivMain = new UIImageView ();
			//			ivMain.AutoresizingMask = UIViewAutoresizing.All;
			ivMain.ContentMode = UIViewContentMode.ScaleAspectFit;
			//			ivMain.BackgroundColor = UIColor.Red; // DEBUG
			AddSubview (ivMain);

			// Setup zoom
			MaximumZoomScale = maxZoom;
			MinimumZoomScale = minZoom;
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			ViewForZoomingInScrollView += (UIScrollView sv) => {
				return ivMain;
			};

			// Setup gestures
			grTap = new UITapGestureRecognizer (() => {
				if (OnSingleTap != null) {
					OnSingleTap();
				}
			});
			grTap.NumberOfTapsRequired = 1;
			AddGestureRecognizer (grTap);

			grDoubleTap = new UITapGestureRecognizer (() => {
				if (ZoomScale >= defaultZoom) {
					SetZoomScale (sizeToFitZoom, true);
				} else {
					//SetZoomScale (defaultZoom, true);

					// Zoom to user specified point instead of center
					var point = grDoubleTap.LocationInView(grDoubleTap.View);
					var zoomRect = GetZoomRect(defaultZoom, point);
					ZoomToRect(zoomRect, true);
				}
			});
			grDoubleTap.NumberOfTapsRequired = 2;
			AddGestureRecognizer (grDoubleTap);

			// To use single tap and double tap gesture recognizers together. See for reference:
			// http://stackoverflow.com/questions/8876202/uitapgesturerecognizer-single-tap-and-double-tap
			grTap.RequireGestureRecognizerToFail (grDoubleTap);
		}

		public void SetImage(UIImage image)
		{
			ZoomScale = 1;
			ivMain.Image = image;
			ivMain.Frame = new RectangleF (new PointF(), image.Size);
			ContentSize = image.Size;

			float wScale = Frame.Width / image.Size.Width;
			float hScale = Frame.Height / image.Size.Height;

			MinimumZoomScale = Math.Min(wScale, hScale);
			sizeToFitZoom = MinimumZoomScale;
			ZoomScale = MinimumZoomScale;

			//
			ivMain.Frame = CenterScrollViewContents ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			ivMain.Frame = CenterScrollViewContents ();
		}

		public override RectangleF Frame
		{
			get
			{
				return base.Frame;
			}
			set
			{
				base.Frame = value;

				if (ivMain != null) {
					ivMain.Frame = value;
				}
			}
		}

		public RectangleF CenterScrollViewContents ()
		{
			var boundsSize = Bounds.Size;
			var contentsFrame = ivMain.Frame;

			if (contentsFrame.Width < boundsSize.Width) {
				contentsFrame.X = (boundsSize.Width - contentsFrame.Width) / 2;
			} else {
				contentsFrame.X = 0;
			}

			if (contentsFrame.Height < boundsSize.Height) {
				contentsFrame.Y = (boundsSize.Height - contentsFrame.Height) / 2;
			} else {
				contentsFrame.Y = 0;
			}

			return contentsFrame;
		}

		// Reference:
		// http://stackoverflow.com/a/11003277/548395
		RectangleF GetZoomRect(float scale, PointF center)
		{
			var size = new SizeF (ivMain.Frame.Size.Height / scale, ivMain.Frame.Size.Width / scale);

			var center2 = ConvertPointToView(center, ivMain);
			var location2 = new PointF(center2.X - (size.Width / 2.0f),
				center2.Y - (size.Height / 2.0f)
			);

			var zoomRect = new RectangleF(location2, size);
			return zoomRect;
		}
	}
}

