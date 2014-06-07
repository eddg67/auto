using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutoLink.Services
{
	public class APIRequest
	{
		public string jsonrpc = "2.0";
		public string id = string.Empty;
		public string method = string.Empty;
		public object Params;

		public string BuildRequestBody(object parameters=null)
		{
			if (parameters != null) 
			{
				this.Params = parameters;
			}
			return Newtonsoft.Json.JsonConvert.SerializeObject (this,new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()

				}).Replace("Params","params");

		}
	}
}

