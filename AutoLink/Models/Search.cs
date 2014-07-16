using System;

namespace AutoLink.Models
{
	[Serializable]
	public class Search
	{
		public string searchName { get; set; }

		public enum searchType {
			Both = 0,
			New = 1,
			Used = 2
		}

		public DateTime YearFrom { get; set; }
		public DateTime YearTo { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }

		public float PriceFrom { get; set; }
		public float PriceTo { get; set; }

		public int DistanceMiles { get; set; }
		public int DistanceFrom { get; set; }

		public string Trim { get; set; }
		public string Exterior { get; set; }

		public int MileageFrom{ get; set;}
		public int MileageTo { get; set; }

		public string Seller { get; set; }

		public bool SearchAuctions { get; set; }
		public bool SendNotifications { get; set; }


		public Search ()
		{
		}
	}
}

