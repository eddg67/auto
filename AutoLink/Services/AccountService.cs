using System;
using PerpetualEngine.Storage;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using AutoLink.Models;

namespace AutoLink.Services
{
	public class AccountService
	{

		public string AccessToken {
			get;
			set;
		}

		public API api;
		SimpleStorage storage;

		public AccountService ()
		{
			AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
			api = app.webService;
			storage = SimpleStorage.EditGroup("Account");
			AccessToken = storage.Get ("token");

		}

		public Task<APIResponse<LoginResult>> GetAccount()
		{
			return api.CreateAsync<LoginResult>(
				@"account.getAuthUser",
				new {}
			);
		}
	}
}

