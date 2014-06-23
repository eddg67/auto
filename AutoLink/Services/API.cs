using System;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonoTouch.Foundation;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using PerpetualEngine.Storage;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace AutoLink.Services
{
	public class API
	{
		public string Token { get; set; }
		public string URL = @"http://api.autolink.co/rpc/";
		public HttpWebRequest request;
		public HttpWebResponse response;
		public int TimeOutFlag = 8000;

		private SimpleStorage storage;


		public API ()
		{
			storage = SimpleStorage.EditGroup("login");
			Token = storage.Get("token") ?? null;
		}

		public APIResponse<T> CreateRequest<T>(string method,object parameters=null)
		{
			APIResponse<T> result=null;
			try{
				Console.WriteLine(method);
			this.request = (HttpWebRequest)HttpWebRequest.Create(URL);
			this.request.ContentType = "application/json";
			this.request.Timeout =TimeOutFlag;
			this.request.Method = "POST";
			this.request.Headers["Authorization-Type"] = "token";

			if (Token != null) {
				this.request.Headers ["Authorization"] = Token;
			}
			var req = new APIRequest {
				method = method
			} ;

			ASCIIEncoding encoding=new ASCIIEncoding();
			string stringData = req.BuildRequestBody (parameters);
			byte[] data = encoding.GetBytes(stringData);

			this.request.ContentLength = data.Length;

			Stream newStream = this.request.GetRequestStream();
			newStream.Write(data,0,data.Length);
			newStream.Close ();


			using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
			{
				if (resp.StatusCode == HttpStatusCode.OK) {
					
					CheckToken(resp.Headers[1]);
					
					using (StreamReader reader = new StreamReader (resp.GetResponseStream ())) {
						var content = reader.ReadToEnd ();
						if (string.IsNullOrWhiteSpace (content)) {

							Console.Out.WriteLine ("Response contained empty body...");

							return null;

						} else {
								Console.Out.WriteLine ("Response Body: \r\n");
								//Console.Out.WriteLine ("\r\n {0}", content);
								result = ParseJson<T> (content);
						}

					}
				} else {
					Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", resp.StatusCode);
				}
			}
			}catch(Exception exp)
			{
				HandleError<T>(exp);

			}

			return result;
		}
			

		public async Task<APIResponse<T>> CreateAsync<T>(string method,object parameters = null)
		{
			try{
			// Create a client
				WebClient client = new WebClient();

			//client.BaseAddress = new i(URL);
			client.Headers["Content-Type"]="application/json";
				client.Headers["Authorization-Type"]= "token";


			if (Token != null) {
				client.Headers.Add ("Authorization", Token);
			}

			var content = string.Empty;
			if (parameters != null) 
			{
					var req = new APIRequest {
						method = method
					} ;
					content = req.BuildRequestBody (parameters);

			}
		
			var result = await client.UploadStringTaskAsync(new Uri(URL), content);
			Console.Out.WriteLine ("Response Body: \r\n");
			//Console.Out.WriteLine ("\r\n {0}", result);
			return ParseJson<T> (result);

			}catch(Exception exp){
				HandleError<T>(exp);
				return null;
			}
		}



		private void ResponseCallback<T>(IAsyncResult result)
		{
			try
			{
				var request = (HttpWebRequest)result.AsyncState;
				var response = request.EndGetResponse(result);

				using (var stream = response.GetResponseStream()){
					using (var reader = new StreamReader(stream))
					{
						string contents = reader.ReadToEnd();
						ParseJson<T> (contents);
					}
				}
			}
			catch (Exception ex)
			{
				HandleError<T>(ex);
			}
		}


		private APIResponse<T> ParseJson<T>(string jsonString)
		{
			var result = new APIResponse<T> ();
			try
			{
				JObject obj = JObject.Parse(jsonString);
				result.id = obj["id"].ToString();
				result.jsonrpc = obj["jsonrpc"].ToString();
				if(obj["result"] != null){

					var res = obj["result"];
				
					result.Result = JsonConvert.DeserializeObject<T>(obj["result"].ToString(),new JsonSerializerSettings
						{
							ContractResolver = new CamelCasePropertyNamesContractResolver()

						});

				}else if(obj["error"] != null){
					Console.Write (jsonString);
					result.error = JsonConvert.DeserializeObject<Error>(obj["error"].ToString());
				}
					
			}
			catch (Exception ex)
			{
				Console.Write (jsonString);
				HandleError<T>(ex);

			}

			return result;
		}

		private APIResponse<T> HandleError<T>(Exception exp)
		{
			var result = new APIResponse<T> ();
			Console.Write (exp.Message);
			result.error = new Error
			{ 
				code = exp.HResult,
				message = exp.Message,
				data = exp.StackTrace
			
			};

			return result;
		}

		void CheckToken(string token)
		{
			if (Token == null) {
				Token = storage.Get ("token") ?? token;
				Console.Write (Token);
				//storage.Put ("token", Token);
			}
		}
			
	}


}

