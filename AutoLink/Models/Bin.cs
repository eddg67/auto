using System;

namespace AutoLink.Models
{
	public class Bin
	{
		public BinItems deleted;
		public BinItems contacted;
		public BinItems @new;
		public BinItems seen;
		public BinItems starred;
		public string[] custom;
	}

	public class BinItems
	{
		public int count;
		public string id;

	}
}

