using System;
using System.Collections.Generic;

namespace AutoLink.Models
{
	public class Custom
	{
		public string __v;
		public string _id;
		public string owner;
		public string name;
		public int order;
		public DateTime updated;
		public DateTime created;
		public string id;
		public int count;
	}

	public class CustomResult : Custom
	{
		public List<Listing> listings;
	}
}



