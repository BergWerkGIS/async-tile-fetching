//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
//     Based on http://stackoverflow.com/a/12606963 and http://wiki.unity3d.com/index.php/WebAsync
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Platform {


	using System;
	using System.Net;
	using System.Net.Cache;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;
	//using System.Windows.Threading;

	internal sealed class HTTPRequest : IAsyncRequest {


		//http://stackoverflow.com/a/33391290
		//	http://answers.unity3d.com/questions/792342/how-to-validate-ssl-certificates-when-using-httpwe.html
		//	https://alexandrebrisebois.wordpress.com/2013/03/24/why-are-webrequests-throttled-i-want-more-throughput/
		//	http://stackoverflow.com/a/17796684

		public bool IsCompleted = false;


		private Action<Response> _callback;
		private HttpWebRequest _hwr;
		//private RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
		private RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
		private int _timeOut;
		private static bool _responseCallbackCompleted = false;
		//private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;



		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="callback"></param>
		/// <param name="timeOut">seconds</param>
		public HTTPRequest(string url, Action<Response> callback, int timeOut = 10) {

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
						System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest before 'gotResponse', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
						gotResponse(response, null);
					}
					// EndGetResponse() throws on on some status codes, try to get response anyway (and status codes)
					catch (WebException wex) {
						HttpWebResponse hwr = wex.Response as HttpWebResponse;
						if (null == hwr) {
							throw;
						}
						gotResponse(hwr, wex);
					}
					catch (Exception ex) {
						gotResponse(null, ex);
					}
				}
			, request);
			};

			// BeginInvoke runs on a thread of the thread pool (!= main thread)
			System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest before 'BeginInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
			actionWrapper.BeginInvoke(new AsyncCallback((iASyncResult) => {
				System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest within 'BeginInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
				var action = (Action)iASyncResult.AsyncState;
				action.EndInvoke(iASyncResult);
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
						} else if (key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase)) {
							response.ContentType = val;
						}
					}
				}

				if (apiResponse.StatusCode != HttpStatusCode.OK) {
					response.AddException(new Exception(string.Format("{0}: {1}", apiResponse.StatusCode, apiResponse.StatusDescription)));
				}
				int statusCode = (int)apiResponse.StatusCode;
				response.StatusCode = statusCode;
				if (429 == statusCode) {
					response.AddException(new Exception("Rate limit hit"));
				}

				if (null != apiResponse) {
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


			//if (_dispatcher.CheckAccess()) {
			//	_callback(response);
			//	IsCompleted = true;
			//} else {
			//	//object retval = _dispatcher.Invoke(new Action(() => _callback(response)));
			//	_dispatcher.BeginInvoke(new Action(() => {
			//		_callback(response);
			//		IsCompleted = true;
			//	}));
			//}


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
