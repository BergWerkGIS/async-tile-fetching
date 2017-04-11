//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
//     Based on http://wiki.unity3d.com/index.php/WebAsync
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Mono {


	using System;
	using System.Collections;
	using System.Net;
	using System.Net.Cache;
	using System.Threading;
	using Mapbox.Platform;
	using System.IO;
	using System.Collections.Generic;

	internal sealed class HTTPRequest_v2 : IAsyncRequest {


		public bool IsCompleted = false;


		private Action<Response> _callback;
		private HttpWebRequest _hwr;
		//private RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
		private RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
		private int _timeOut;
		private static bool _responseCallbackCompleted = false;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="callback"></param>
		/// <param name="timeOut">seconds</param>
		public HTTPRequest_v2(string url, Action<Response> callback, int timeOut = 10) {

			_callback = callback;
			_timeOut = timeOut;

			_hwr = WebRequest.Create(url) as HttpWebRequest;
			_hwr.Method = "GET";
			_hwr.UserAgent = "mapbox-sdk-cs";
			_hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			_hwr.CachePolicy = _cachePolicy;
			//_hwr.Timeout = timeOut * 1000; doesn't work in async calls, see below

			GetResponseAsync(_hwr, EvaluateResponse);
		}


		private void GetResponseAsync(HttpWebRequest request, Action<HttpWebResponse, Exception> gotResponse) {

			// create an additional action wrapper, because of:
			// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse.aspx
			// The BeginGetResponse method requires some synchronous setup tasks to complete (DNS resolution,
			//proxy detection, and TCP socket connection, for example) before this method becomes asynchronous.
			// As a result, this method should never be called on a user interface (UI) thread because it might
			// take considerable time(up to several minutes depending on network settings) to complete the
			// initial synchronous setup tasks before an exception for an error is thrown or the method succeeds.

			Action actionWrapper = () => {
				request.BeginGetResponse((r) => {
					try { // there's a try/catch here because execution path is different from invokation one, exception here may cause a crash
						HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(r);
						gotResponse(response, null);
					}
					catch (Exception ex) {
						gotResponse(null, ex);
					}
				}
			, request);
			};

			actionWrapper.BeginInvoke(new AsyncCallback((iar) => {
				var action = (Action)iar.AsyncState;
				action.EndInvoke(iar);
			})
			, actionWrapper
			);
		}


		private void EvaluateResponse(HttpWebResponse apiResponse, Exception apiEx) {

			var response = new Response();
			//response.Exceptions = taskInfo.Exceptions.Count > 0 ? taskInfo.Exceptions : null;
			if (null != apiEx) {
				response.AddException(apiEx);
			}

			// timeout: API response is null
			if (null == apiResponse) {
				response.AddException(new Exception("No Reponse."));
			} else {
				// TODO: evaluate headers and add custom exception, eg if rate limit is exceeded
				// https://www.mapbox.com/api-documentation/#rate-limits
				// X-Rate-Limit-Interval
				// X-Rate-Limit-Limit
				// X-Rate-Limit-Reset
				if (null != apiResponse.Headers) {
					response.Headers = new Dictionary<string, string>();
					for (int i = 0; i < apiResponse.Headers.Count; i++) {
						string key = apiResponse.Headers.Keys[i];
						string val = apiResponse.Headers[i];
						response.Headers.Add(key, val);
						if (key.Equals("X-Rate-Limit-Interval", StringComparison.InvariantCultureIgnoreCase)) {
							int limitInterval;
							if (int.TryParse(val, out limitInterval)) { response.XRateLimitInterval = limitInterval; }
						} else if (key.Equals("X-Rate-Limit-Limit", StringComparison.InvariantCultureIgnoreCase)) {
							long limitLimit;
							if (long.TryParse(val, out limitLimit)) { response.XRateLimitLimit = limitLimit; }
						} else if (key.Equals("X-Rate-Limit-Reset", StringComparison.InvariantCultureIgnoreCase)) {
							double unixTimestamp;
							if (double.TryParse(val, out unixTimestamp)) {
								DateTime beginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
								response.XRateLimitReset = beginningOfTime.AddSeconds(unixTimestamp).ToLocalTime();
							}
						}
					}
				}

				if (apiResponse.StatusCode != HttpStatusCode.OK) {
					response.AddException(new Exception(string.Format("{0}: {1}", apiResponse.StatusCode, apiResponse.StatusDescription)));
				}

				if (null == response.Exceptions || response.Exceptions.Count < 1) {
					using (Stream responseStream = apiResponse.GetResponseStream()) {
						byte[] buffer = new byte[0x1000];
						int bytesRead;
						using (MemoryStream ms = new MemoryStream()) {
							while (0 != (bytesRead = responseStream.Read(buffer, 0, buffer.Length))) {
								ms.Write(buffer, 0, bytesRead);
							}
							response.Data = ms.ToArray();
						}
					}
				}
			}

			_callback(response);
			IsCompleted = true;
		}

		public void Cancel() {

			_callback = null;
			if (null != _hwr) {
				_hwr.Abort();
			}
			IsCompleted = true;
		}


	}
}
