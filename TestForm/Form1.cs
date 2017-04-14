using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mapbox.Platform;
using System.Threading;
using System.Net;

namespace Mapbox.Platform {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void btnGo_Click(object sender, EventArgs e) {

			SynchronizationContext sync = SynchronizationContext.Current;
			lvInfo.Items.Clear();

			int countReq = 0;
			int countResp = 0;
			FileSource fs = new FileSource();

			//https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/14/3410/6200.vector.pbf
			//https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/14/3410/6200.vector.pbf
			//https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/74984/100276.vector.pbf

			object locker = new object();

			System.Diagnostics.Debug.WriteLine(string.Format(" ============> WinForm, thread id:{0} <==============", System.Threading.Thread.CurrentThread.ManagedThreadId));
			for (int i = (int)outerLoopStart.Value; i < (int)outerLoopStop.Value; i++) {
				for (int x = (int)tileXstart.Value; x < tileXstop.Value; x++) {

					countReq++;
					lblReqCnt.Text = countReq.ToString();

					HTTPRequest request = (HTTPRequest)fs.Request(
						string.Format("https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/{0}/100276.vector.pbf", x)
						, (Response r) => {
							System.Diagnostics.Debug.WriteLine(string.Format("winform response, thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
							//lock (locker) {
								countResp++;
								try {
									if (lblRespCnt.InvokeRequired) {
										System.Diagnostics.Debug.WriteLine(string.Format("winform lblRespCnt.InvokeRequired, thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
										//lblRespCnt.Invoke(new Action(() => { lblRespCnt.Text = countResp.ToString(); }));
										//Invoke((MethodInvoker)delegate { lblRespCnt.Text = countResp.ToString(); });
										//sync.Send(delegate { lblRespCnt.Text = countResp.ToString(); }, null);
										//sync.Send(delegate { lblRespCnt.Text = countResp.ToString(); }, null);
										sync.Post(delegate { lblRespCnt.Text = countResp.ToString(); }, null);
									} else {
										System.Diagnostics.Debug.WriteLine(string.Format("winform !lblRespCnt.InvokeRequired, thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
										lblRespCnt.Text = countResp.ToString();
									}
								}
								catch (Exception ex) {
									addItem(ex.ToString());
								}
							//}
							if (r.RateLimitHit) {
								addItem(string.Format("{3} statuscode:{4} rate limit hit:{5} --- LimitInterval:{0} LimitLimit:{1} LimitReset:{2}", r.XRateLimitInterval, r.XRateLimitLimit, r.XRateLimitReset, x, r.StatusCode, r.RateLimitHit));
							}
							if (r.StatusCode != 200) {
								addItem(Encoding.UTF8.GetString(r.Data));
							}
							if (null != r.Exceptions && r.Exceptions.Count > 0) {
								addItem("");
								foreach (var ex in r.Exceptions) {
									addItem(ex.ToString());
								}
								addItem("");
								return;
							}

							//VectorTile vt = new VectorTile(r.Data);
							//Console.WriteLine(string.Join(", ", vt.LayerNames().ToArray()));
							//VectorTileLayer roads= vt.GetLayer("road");
							//int featCnt = roads.FeatureCount();
							//for (int i = 0; i < featCnt; i++) {
							//	VectorTileFeature feat = roads.GetFeature(i);
							//	foreach (var p in feat.GetProperties()) {
							//		Console.WriteLine(string.Format("{0}:{1}", p.Key, p.Value));
							//	}
							//}

						}
					);

				}
			}





			addItem("waiting ...");
			//while (!request.IsCompleted) {  }
			fs.WaitForAllRequests();
			addItem(string.Format("finished! {0}", countResp));

		}



		private void addItem(string msg) {
			msg = string.Format("{0}: {1}", DateTime.Now.ToString("hh:mm:ss:.fff"), msg);
			ListViewItem lvi = new ListViewItem(msg);
			lvInfo.Items.Add(lvi);
		}

		private void Form1_Load(object sender, EventArgs e) {
			calcReqCnt();
		}

		private void outerLoopStart_ValueChanged(object sender, EventArgs e) {
			calcReqCnt();
		}

		private void outerLoopStop_ValueChanged(object sender, EventArgs e) {
			calcReqCnt();
		}

		private void tileXstart_ValueChanged(object sender, EventArgs e) {
			calcReqCnt();
		}

		private void tileXstop_ValueChanged(object sender, EventArgs e) {
			calcReqCnt();
		}


		private void calcReqCnt() {
			int outer = (int)(outerLoopStop.Value - outerLoopStart.Value);
			int tiles = (int)(tileXstop.Value - tileXstart.Value);

			lblTodoCnt.Text = (outer * tiles).ToString();
		}

		private void btnGo2_Click(object sender, EventArgs e) {

			SynchronizationContext sync = AsyncOperationManager.SynchronizationContext;

			string url = "https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/74984/100276.vector.pbf";
			url += "?access_token=" + Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN");

			HttpWebRequest hwr = WebRequest.Create(url) as HttpWebRequest;
			hwr.BeginGetResponse((asyncResult) => {
				try {
					HttpWebResponse response = hwr.EndGetResponse(asyncResult) as HttpWebResponse;
					//System.InvalidOperationException: Cross-thread operation not valid: Control 'lblRespCnt' accessed from a thread other than the thread it was created on.
					//lblRespCnt.Text = "got response";
					SynchronizationContext ctxt = asyncResult.AsyncState as SynchronizationContext;
					ctxt.Post(delegate { lblRespCnt.Text = "got response"; }, null);

				}
				catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex);
				}
			},
			sync);
		}

		private void btnGo3_Click(object sender, EventArgs e) {

			string url = "https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/74984/100276.vector.pbf";
			url += "?access_token=" + Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN");

			HTTPRequestSimple hrs = new HTTPRequestSimple(
				url
				, (Response resp) => {
					lblRespCnt.Text = "got response 3";
				}
			);
		}
	}
}
