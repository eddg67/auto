using System;
using System.Collections.Generic;

namespace AutoLink.Models
{
	[Serializable]
	public class Bin
	{
		public BinItems deleted;
		public BinItems contacted;
		public BinItems @new;
		public BinItems seen;
		public BinItems starred;
		public List<Custom> custom;
	}
	[Serializable]
	public class BinItems
	{
		public int count;
		public string id;

	}
}

