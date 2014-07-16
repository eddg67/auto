using System;
using AutoLink.Utilities;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace AutoLink
{
	public class ImageService : ImageDownloader
	{
		public ImageService () : base()
		{

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

