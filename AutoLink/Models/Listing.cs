using System;
using System.Collections.Generic;

namespace AutoLink.Models
{
	public enum ListType{
		Bin,
		Listings,
		Custom
	};
	public class Listing
	{
		public ListType listType;
		public string _id;
		public DateTime ? created;
		public string id;
		public string source;
		public string type;
		public DateTime ? updated;
		public string url;
		public Pricing pricing;
		public Address address;
		public string latitude;
		public string longitude;
		public SellerContact sellerContact;
		public string price;
		public string mileage;
		public string interiorColor;
		public string exteriorColor;
		public string trim;
		public string model;
		public string make;
		public string year;
		public List<string> images;
		public string descriptionCollapsed;
		public string description;
		public string title;
		public bool deleted;
		public bool @new;
		public bool contacted;
		public bool starred;
	}
}

/*
"_id":"5386978d08fa7b61fc6ac7d7",
            "created":"2014-05-29T02:06:39.243Z",
            "id":"607632485",
            "source":"cars",
            "type":"used",
            "updated":"2014-05-29T10:16:04.955Z",
            "url":"http://www.cars.com/go/search/detail.jsp?tracktype=usedcc&csDlId=&csDgId=&listingId=607632485&listingRecNum=2573&criteria=feedSegId%3D28705%26rpp%3D250%26isDealerGrouping%3Dfalse%26ldId%3D28882%26sf2Nm%3Dprice%26requestorTrackingInfo%3DRTB_SEARCH%26sf1Nm%3DlistingDateSeconds%26sf2Dir%3DDESC%26stkTypId%3D28881%26PMmt%3D0-0-0%26rn%3D2500%26zc%3D80108%26rd%3D100000%26crSrtFlds%3DstkTypId-feedSegId-ldId%26ldNmb%3D1%26stkTyp%3DU%26sf1Dir%3DDESC&aff=national&listType=1",
            "pricing":{
               "trade":[
                  4500,
                  6500,
                  7500
               ],
               "private":[
                  6500,
                  7500,
                  8500
               ],
               "retail":[
                  750000,
                  850000,
                  950000
               ]
            },
            "address":{
               "country":null,
               "zip":null,
               "state":null,
               "city":null,
               "street":null
            },
            "longitude":-95.665,
            "latitude":37.6,
            "sellerContact":{
               "url":"http://www.cars.com/go/search/detail.jsp?tracktype=usedcc&csDlId=&csDgId=&listingId=607632485&listingRecNum=2573&criteria=feedSegId%3D28705%26rpp%3D250%26isDealerGrouping%3Dfalse%26ldId%3D28882%26sf2Nm%3Dprice%26requestorTrackingInfo%3DRTB_SEARCH%26sf1Nm%3DlistingDateSeconds%26sf2Dir%3DDESC%26stkTypId%3D28881%26PMmt%3D0-0-0%26rn%3D2500%26zc%3D80108%26rd%3D100000%26crSrtFlds%3DstkTypId-feedSegId-ldId%26ldNmb%3D1%26stkTyp%3DU%26sf1Dir%3DDESC&aff=national&listType=1",
               "email":null,
               "phone":"877-563-0026"
            },
            "price":14990,
            "mileage":51136,
            "interiorColor":"ash",
            "exteriorColor":"white",
            "trim":null,
            "model":"Camry",
            "make":"Toyota",
            "year":2011,
            "images":[
               "http://www.cstatic-images.com/stock/310x208/256510.jpg",
               "http://www.cstatic-images.com/stock/310x208/256516.jpg",
               "http://www.cstatic-images.com/stock/310x208/256528.jpg",
               "http://www.cstatic-images.com/stock/310x208/256519.jpg",
               "http://www.cstatic-images.com/stock/310x208/256511.jpg",
               "http://www.cstatic-images.com/stock/310x208/256522.jpg",
               "http://www.cstatic-images.com/stock/310x208/256521.jpg",
               "http://www.cstatic-images.com/stock/310x208/256520.jpg",
               "http://www.cstatic-images.com/stock/310x208/256518.jpg",
               "http://www.cstatic-images.com/stock/310x208/256517.jpg",
               "http://www.cstatic-images.com/stock/310x208/256515.jpg",
               "http://www.cstatic-images.com/stock/310x208/256514.jpg",
               "http://www.cstatic-images.com/stock/310x208/256513.jpg",
               "http://www.cstatic-images.com/stock/310x208/256512.jpg",
               "http://www.cstatic-images.com/stock/310x208/256509.jpg",
               "http://www.cstatic-images.com/stock/310x208/256508.jpg",
               "http://www.cstatic-images.com/stock/310x208/256527.jpg",
               "http://www.cstatic-images.com/stock/310x208/256526.jpg",
               "http://www.cstatic-images.com/stock/310x208/256525.jpg",
               "http://www.cstatic-images.com/stock/310x208/256524.jpg",
               "http://www.cstatic-images.com/stock/310x208/256523.jpg"
            ],
            "descriptionCollapsed":"Auto Wholesale is the premier pre-owned car retailer in Wilmington NC, in a truthful and honest manner, giving buyer what they really need...A quality car or truck at a fair price, with no gimmicks.",
            "description":"Auto Wholesale is the premier pre-owned car retailer in Wilmington NC, in a truthful and honest manner, giving buyer what they really need...A quality car or truck at a fair price, with no gimmicks.",
            "title":"2011 Toyota Camry LE",
            "deleted":false,
            "new":true,
            "contacted":false,
            "starred":false

*/
