using System;
using System.Collections.Generic;

//"result":{"emailNotifications":true,"email":"user@bluerival.com","notifications":[],"waitingNotifications":false,
//"lastNotified":null,"dialogues":{"dontShowDeleteListingWarning":false,"dontShowContactWarning":false},
//"password":null,"zip":null,"name":"A
//User","updated":"2014-05-14T20:00:52.638Z","created":"2014-05-14T20:00:52.638Z","id":"5373cb740dafb5fb39eff0d9"}}
namespace AutoLink.Models
{
	public class LoginResult
	{

		public string _id;
		public bool emailNotifications;
		public string email;
		public List<string> notifications;
		public bool waitingNotifications;
		public DateTime ? lastNotified;
		public string password;
		public string zip;
		public string name;
		public DateTime ? updated;
		public DateTime ? created;
		public string id;
		public Dialogues dialogues;

	}

	public class Dialogues
	{
		public bool dontShowDeleteListingWarning;
		public bool dontShowContactWarning;

	}


}

