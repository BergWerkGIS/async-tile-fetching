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

	internal sealed class HTTPRequest_v1 : IAsyncRequest {


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
		public HTTPRequest_v1(string url, Action<Response> callback, int timeOut = 10) {

			_callback = callback;
			_timeOut = timeOut;

			_hwr = WebRequest.Create(url) as HttpWebRequest;
			_hwr.UserAgent = "mapbox-sdk-cs";
			_hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			_hwr.CachePolicy = _cachePolicy;
			//_hwr.Timeout = timeOut * 1000; doesn't work in async calls, see below

			triggerRequest(url);
		}


		public void Cancel() {

			_callback = null;
			if (null != _hwr) {
				_hwr.Abort();
			}
			IsCompleted = true;
		}


		private void triggerRequest(string url) {

			IEnumerator e = null;
			Action nextStep = () => e.MoveNext();
			e = DoRequestAsync(url);
			nextStep();
		}


		private IEnumerator DoRequestAsync(string url) {

			TaskInfo taskInfo = new TaskInfo() { Request = _hwr };

			// Do the actual async call here
			IAsyncResult asyncResult = (IAsyncResult)_hwr.BeginGetResponse(
				new AsyncCallback(responseCallback)
				, taskInfo
			);



			// TODO: remove this is just for debugging
			//for (int i = 0; i < 10; i++) { Thread.Sleep(500); }



			// WebRequest timeout won't work in async calls, so we need this instead
			taskInfo.WaitHandle = ThreadPool.RegisterWaitForSingleObject(
				asyncResult.AsyncWaitHandle
				, new WaitOrTimerCallback(timeoutCallback)
				, taskInfo
				, _timeOut * 1000
				, true
			);

			while (!asyncResult.IsCompleted) { yield return null; }
			// wait a bit more: 'asyncResult.IsCompleted' is true before the response callback has finished
			while (!_responseCallbackCompleted) { }

			WebResponse apiResponse = taskInfo.Response;

			var response = new Response();
			//response.Exceptions = taskInfo.Exceptions.Count > 0 ? taskInfo.Exceptions : null;
			response.Exceptions = taskInfo.Exceptions;

			// timeout: API response is null
			if (null == apiResponse) {
				response.Exceptions.Add(new Exception("No Reponse."));
			} else {
				// TODO: evaluate headers and add custom exception, eg if rate limit is exceeded
				// https://www.mapbox.com/api-documentation/#rate-limits
				// X-Rate-Limit-Interval
				// X-Rate-Limit-Limit
				// X-Rate-Limit-Reset
				response.Headers = new Dictionary<string, string>();
				for (int i = 0; i < apiResponse.Headers.Count; i++) {
					response.Headers.Add(apiResponse.Headers.Keys[i], apiResponse.Headers[i]);
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

			IsCompleted = true;
			_callback(response);
		}


		static private void responseCallback(IAsyncResult asyncResult) {

			TaskInfo taskInfo = (TaskInfo)asyncResult.AsyncState;
			try {
				taskInfo.Response = taskInfo.Request.EndGetResponse(asyncResult);
			}
			catch (Exception ex) {
				taskInfo.Exceptions.Add(ex);
			}
			_responseCallbackCompleted = true;
		}


		static private void timeoutCallback(object state, bool timedOut) {

			TaskInfo taskInfo = (TaskInfo)state;
			if (timedOut) {
				if (taskInfo != null && taskInfo.Request != null) {
					taskInfo.Request.Abort();
				}
				taskInfo.Exceptions.Add(new Exception("Request timed out."));
			} else {
				if (taskInfo.WaitHandle != null) {
					taskInfo.WaitHandle.Unregister(null);
				}
			}
		}


		// Helper class to put request into the state object so it can be passed around
		internal class TaskInfo {
			public HttpWebRequest Request;
			public WebResponse Response;
			public List<Exception> Exceptions = new List<Exception>();
			public RegisteredWaitHandle WaitHandle = null;
		}

	}
}
