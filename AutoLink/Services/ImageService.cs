using System;
using AutoLink.Utilities;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace AutoLink
{
	public class ImageService : ImageDownloader
	{
		const string proxy = "http://images-g.autolink.co/gadgets/proxy?container=focus&refresh=86400&url=";
		public ImageService () : base()
		{

		}

		public string AddProxyToURL(string url,string width,string height)
		{
			return string.Format ("{0}{1}&resize_w={2}&resize_h={3}", proxy,url,width,height);
		}

		protected override object LoadImage (Stream stream)
		{
			return UIImage.LoadFromData(NSData.FromArray(ReadFully(stream)));
		}

		Byte[] ReadFully(Stream input)
		{
			Byte[] buffer = new Byte[16*1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
	}
}

