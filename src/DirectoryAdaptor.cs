using System;
using System.Collections;

namespace FSpot {
	public class DirectoryAdaptor : GroupAdaptor {
		public PhotoQuery query;
		public ArrayList dirs = new ArrayList ();


		struct DirInfo {
			public string Path;
			public int Count;
		}
		
		public override event GlassSetHandler GlassSet;

		public override void SetGlass (int group)
		{
			Console.WriteLine ("Selected Path {0}", ((DirInfo)dirs [group]).Path);
			
			int item = 0;
			int i = 0;
			while (i < group) {
				item += ((DirInfo)dirs [i++]).Count;
			}

			GlassSet (this, item);
		}

		public override int Count ()
		{
			return dirs.Count;
		}

		public override string GlassLabel (int item)
		{
			DirInfo info = (DirInfo)dirs[item];
			return info.Path;
		}
		
		public override string TickLabel (int item)
		{
			return null;
		}

		public override int Value (int item) 
		{
			DirInfo info = (DirInfo)dirs[item];	
			return info.Count;
		}	

		public void Load () {
			dirs.Clear ();
			Photo [] photos = query.Store.Query (null, null);
			
			Array.Sort (photos, new Photo.CompareDirectory ());
			Array.Sort (query.Photos, new Photo.CompareDirectory ());

			DirInfo info = new DirInfo ();
			foreach (Photo p in photos) {
				string dir = p.DirectoryPath;
				if (dir != info.Path) {
					if (info.Path != null)
						dirs.Add (info);

					info.Path = dir;
					info.Count = 0;
				}
				info.Count += 1;
			}
		}

		public DirectoryAdaptor (PhotoQuery query) {
			this.query = query;

			Load ();
		}
	}
}
