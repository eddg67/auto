using System;
using MonoTouch.UIKit;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoLink.Utilities
{
	public class Validator
	{
		public Validator ()
		{
		}

		public bool isEmptyTxt(UITextField item)
		{
			bool result = false;
			if (item.Text.Length <= 0) {
				item.Layer.BorderColor = UIColor.Red.CGColor;
				item.Layer.BorderWidth = 3;
				item.Layer.CornerRadius = 5;
				result = true;
			}
			return result;
		}

		public bool isEmail(UITextField item)
		{
			bool result = false;
			string strIn;

			if(!isEmptyTxt(item)){
				// Use IdnMapping class to convert Unicode domain names. 
				try {
					strIn = Regex.Replace(item.Text, @"(@)(.+)$", this.DomainMapper,
					RegexOptions.None, TimeSpan.FromMilliseconds(200));
				}
				catch (RegexMatchTimeoutException) {
					return false;
				}

				try {
					result = Regex.IsMatch(strIn,
						@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
						@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
						RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
				}
				catch (RegexMatchTimeoutException) {
					return false;
				}

			}

			return result;
		}

		public bool isEmail(string item)
		{
			bool result = false;
			string strIn;

			if (!string.IsNullOrEmpty(item)) 
			{
				try {
					strIn = Regex.Replace(item, @"(@)(.+)$", this.DomainMapper,
						RegexOptions.None, TimeSpan.FromMilliseconds(200));
				}
				catch (RegexMatchTimeoutException) {
					return false;
				}

				try {
					result = Regex.IsMatch(strIn,
						@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
						@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
						RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
				}
				catch (RegexMatchTimeoutException) {
					return false;
				}

			}

			return result;
		}


		private string DomainMapper(Match match)
		{
			// IdnMapping class with default property values.
			IdnMapping idn = new IdnMapping();

			string domainName = match.Groups[2].Value;
			try {
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException) {
				return string.Empty;
			}
			return match.Groups[1].Value + domainName;
		}
	}
}

