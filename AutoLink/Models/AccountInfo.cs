using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace AutoLink.Models
{
	[Serializable]
	public class AccountInfo
	{
		[Section]
		[Entry("Name")]
		[Caption("")]
		public string Name;

		[Section]
		[Entry("Email")]
		[Caption("")]
		public string Email;

		[Section]
		[Entry("Zip Code")]
		[Caption("")]
		public string Zip;

		[Section]
		[Entry("Password")]
		[Caption("")]
		public string Password;


		public AccountInfo()
		{
		}

	

	
	}
}

