using Mapbox.Mono;
using Mapbox.Platform;
using Mapbox.VectorTile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace async_tile_fetching {


	class Program {


		static void Main(string[] args) {


			FileSource fs = new FileSource();


			//HTTPRequest_v1 request = (HTTPRequest_v1)fs.Request(
			//	"https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/74984/100276.vector.pbf"
			//	, (Response r) => {
			//		if (null != r.Exceptions && r.Exceptions.Count > 0) {
			//			foreach (var ex in r.Exceptions) {
			//				Console.WriteLine(ex);
			//			}
			//			return;
			//		}

			//		VectorTile vt = new VectorTile(r.Data);
			//		Console.WriteLine(string.Join(", ", vt.LayerNames().ToArray()));
			//	}
			//);

			//request.Cancel();
			//while (!request.IsCompleted) { }


			//https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/14/3410/6200.vector.pbf?access_token=pk.eyJ1IjoiZGF2aWRyaG9kZXMiLCJhIjoiY2owd3htdmlmMDA2bDMybmRyNzZ0ZWd0YSJ9.p3D6QfRRpcd0jQDDvrWTaA

			//for (int x = 74904; x < 74984; x++) {

				HTTPRequest_v2 request = (HTTPRequest_v2)fs.Request(
					//"https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/74984/100276.vector.pbf"
					/*//string.Format("https://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/18/{0}/100276.vector.pbf", x)*/
					"https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/14/3410/6200.vector.pbf"
					, (Response r) => {
						//Console.WriteLine(string.Format("{3} --- LimitInterval:{0} LimitLimit:{1} LimitReset:{2}", r.XRateLimitInterval, r.XRateLimitLimit, r.XRateLimitReset, x));
						if (null != r.Exceptions && r.Exceptions.Count > 0) {
							Console.WriteLine();
							foreach (var ex in r.Exceptions) {
								Console.WriteLine(ex);
							}
							Console.WriteLine();
							return;
						}
						VectorTile vt = new VectorTile(r.Data);
						Console.WriteLine(string.Join(", ", vt.LayerNames().ToArray()));
						VectorTileLayer roads= vt.GetLayer("road");
						int featCnt = roads.FeatureCount();
						for (int i = 0; i < featCnt; i++) {
							VectorTileFeature feat = roads.GetFeature(i);
							foreach (var p in feat.GetProperties()) {
								Console.WriteLine(string.Format("{0}:{1}", p.Key, p.Value));
							}
						}

					}
				);

			//}





			Console.WriteLine("waiting ...");
			//while (!request.IsCompleted) {  }
			fs.WaitForAllRequests();

		}




	}
}
