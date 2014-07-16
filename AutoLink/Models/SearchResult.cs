using System;
using System.Collections.Generic;


namespace AutoLink.Models
{
	[Serializable]
	public class SearchResult
	{
		public string __v;
		public string _id;
		public bool auction;
		public DateTime ? created;
		public string exteriorColor;
		public string id;
		public string interiorColor;
		public DateTime ? lastNotified;
		public DateTime ? lastRun;
		public Listings listings;
		public string make;
		public string model;
		public string name;
		public int newListingsCount;
		public bool ? notify;
		public string owner;
		public string seller;
		public string trim;
		public DateTime ? updated;
		public int version;
		public bool waitingNotifications;
		public Type type;
		public Location location;
		public Price price;
		public Years years;
		public Mileage miles;

	}
	public class Location
	{
		public string range;
		public string zip;

	}
	public class Mileage
	{
		public int? max;
		public int? min;

	}

	public class Price
	{
		public int? max;
		public int? min;


	}

	public class Years
	{
		public string max;
		public string min;
	}

	public class Listings
	{
		public string[] all;
		public string[] contacted;
		public string[] deleted;
		public string[] notified;
		public string[] seen;
		public string[] starred;

	}

}


/*
 * "_id" : "535ed96ac2f9646166566242",
        "auctions" : true,
        "created" : "2014-04-28T22:42:50.997Z",
        "exteriorColor" : null,
        "id" : "535ed96ac2f9646166566242",
        "interiorColor" : null,
        "lastNotified" : "2014-04-28T22:47:58.697Z",
        "lastRun" : "2014-05-14T20:22:47.570Z",
        "listings" : { "all" : [ "535ed971fd3a928593f13aff" ],
            "contacted" : [  ],
            "deleted" : [  ],
            "notified" : [ "535ed971fd3a928593f13aff" ],
            "seen" : [  ],
            "starred" : [  ]
          },
        "location" : { "range" : 100000000,
            "zip" : ""
          },
        "make" : "Mercedes Benz",
        "mileage" : { "max" : 100000000,
            "min" : 0
          },
        "model" : "GLK",
        "name" : "mercedes",
        "newListingsCount" : 1,
        "notify" : false,
        "owner" : "532bf47c7e960a9445b81595",
        "price" : { "max" : 100000000,
            "min" : 0
          },
        "seller" : null,
        "trim" : null,
        "type" : { "both" : true,
            "new" : false,
            "used" : false
          },
        "updated" : "2014-05-14T20:22:47.570Z",
        "version" : 2,
        "waitingNotifications" : false,
        "years" : { "max" : 3000,
            "min" : 0
          }
      },
 * 
 * 
 */

