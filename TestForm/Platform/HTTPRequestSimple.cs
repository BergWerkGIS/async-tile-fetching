using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Mapbox.Platform {


	public class HTTPRequestSimple {

		private SynchronizationContext _sync;
		private Action<Response> _callback;

		public HTTPRequestSimple(string url, Action<Response> callback) {

			_callback = callback;
			 _sync = AsyncOperationManager.SynchronizationContext;

			HttpWebRequest hwr = WebRequest.Create(url) as HttpWebRequest;
			getResponseAsncy(hwr);
		}


		private void getResponseAsncy(HttpWebRequest hwr) {

			Action actionWrapper = () => {
				hwr.BeginGetResponse((asyncResult) => {
					try {
						HttpWebResponse response = hwr.EndGetResponse(asyncResult) as HttpWebResponse;
						SynchronizationContext ctxt = asyncResult.AsyncState as SynchronizationContext;
						ctxt.Post(delegate { _callback(new Response()); }, null);
					}
					catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
					}
				}
				, _sync);
			};

			actionWrapper.BeginInvoke(new AsyncCallback((iAsyncResult) => {
				var action = (Action)iAsyncResult.AsyncState;
				action.EndInvoke(iAsyncResult);
			})
			, actionWrapper);

		}


	}
}
