using System;

namespace AutoLink.Services
{
	public class APIResponse<T>
	{
		public string jsonrpc;
		public string id;
		public Error error;
		public string data;
		public T Result;

	}

	public class Error
	{
		public int code;
		public string message;
		public string data;

	}




}

