/*
 * Widgets.Filmstrip.cs
 *
 * Author(s)
 * 	Stephane Delcroix  <stephane@delcroix.org>
 *
 * This is free software. See COPYING for details.
 */

//TODO:
//	* deal with vertical orientations (medium)
//	* only redraw required parts on ExposeEvents (low)
//	* Handle orientation changes (low) (require gtk# changes, so I can trigger an OrientationChanged event)

using System;
using System.Collections;

using Gtk;
using Gdk;

namespace FSpot.Widgets
{
	public class Filmstrip : EventBox, IDisposable
	{

		public event OrientationChangedHandler OrientationChanged;
		public event EventHandler PositionChanged;

		bool extendable = true;
		public bool Extendable {
			get { return extendable; }
			set { extendable = value; }
		}

		Orientation orientation = Orientation.Horizontal;
		public Orientation Orientation {
			get { return orientation; }
			set {
				if (orientation == value)
					return;

				throw new NotImplementedException ();
				if (OrientationChanged != null) {
					OrientationChangedArgs args = new OrientationChangedArgs ();
					//args.Orientation = value;
					//OrientationChanged (this, args);
				}
			}
		}

		int spacing = 6;
		public int Spacing {
			get { return spacing; }
			set {
				if (value < 0)
					throw new ArgumentException ("Spacing is negative!");
				spacing = value;
			}
		}

		int thumb_offset = 17;
		public int ThumbOffset {
			get { return thumb_offset; }
			set { 
				if (value < 0)
					throw new ArgumentException ("ThumbOffset is negative!");
				thumb_offset = value;
			}
		}

		int thumb_size = 67;
		public int ThumbSize {
			get { return thumb_size; }
			set { 
				if (value < 0)
					throw new ArgumentException ("ThumbSize is negative!");
				thumb_size = value;
			}
		}

		static string [] film_100_xpm = {
		"14 100 2 1",
		" 	c None",
		".	c #000000",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		".....    .....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		".....    .....",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		"..............",
		".....    .....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		"....      ....",
		".....    .....",
		"..............",
		"..............",
		"..............",
		"..............",
		".............."};
		Pixbuf background_tile;
		public Pixbuf BackgroundTile {
			get {
				if (background_tile == null)
					if (orientation == Orientation.Horizontal)
						background_tile = new Pixbuf(film_100_xpm);
					else
						throw new NotImplementedException ("doesn't support Vertical orientation yet");
				return background_tile;
			}
			set { 
				if (background_tile != value && background_tile != null)
					background_tile.Dispose ();
				background_tile = value;
				BackgroundPixbuf = null;
			}
		}

		int x_offset = 2, y_offset = 2;
		public int XOffset {
			get { return x_offset; }
			set { 
				if (value < 0)
					throw new ArgumentException ("value is negative!");
				x_offset = value;
			}
		}

		public int YOffset {
			get { return y_offset; }
			set { 
				if (value < 0)
					throw new ArgumentException ("value is negative!");
				y_offset = value;
			}
		}

		float x_align = 0.5f, y_align = 0.5f;
		public float XAlign {
			get { return x_align; }
			set {
				if (value < 0.0 || value > 1.0)
					throw new ArgumentException ("value is not between 0.0 and 1.0");
				x_align = value;
			}
		}

		public float YAlign {
			get { return y_align; }
			set {
				if (value < 0.0 || value > 1.0)
					throw new ArgumentException ("value is not between 0.0 and 1.0");
				y_align = value;
			}
		}

		IAnimator animator;
		IAnimator Animator {
			get {
				if (animator == null)
					animator = new AcceleratedAnimator (this, OnPositionChanged);
				return animator;
			}
		}
		public int AnimatorOrder {
			set {
				switch (value) {
				case 0:
					animator = new DirectAnimator (OnPositionChanged);
					break;
				case 1:
					animator = new ConstantSpeedAnimator (this, OnPositionChanged);
					break;
				case 2:
					animator = new AcceleratedAnimator (this, OnPositionChanged);
					break;
				default:
					throw new ArgumentException ("No animator of that order defined");
				}
			}
		}

		public int ActiveItem {
			get { return selection.Index; }
			set { selection.Index = value; }
		}

		float position;
		public float Position {
			get { 
				position = Math.Min (position, selection.Collection.Count - 1);
				return position; 
			}
			set {
				if (value == position)
					return;
				if (value < 0)
					value = 0;
				if (value > selection.Collection.Count - 1)
					value = selection.Collection.Count - 1;

				Animator.MoveTo (value);
				if (PositionChanged != null)
					PositionChanged (this, EventArgs.Empty);
			}
		}

		FSpot.BrowsablePointer selection;
		ThumbnailCache thumb_cache;

		public Filmstrip (FSpot.BrowsablePointer selection) : base ()
		{
			CanFocus = true;
			this.selection = selection;
			this.selection.Changed += HandlePointerChanged;
			this.selection.Collection.Changed += HandleCollectionChanged;
			this.selection.Collection.ItemsChanged += HandleCollectionItemsChanged;
			thumb_cache = new ThumbnailCache (30);
		}
	
		int min_length = 400;
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			requisition.Width = min_length + 2 * x_offset;
			if (min_length % BackgroundTile.Width != 0)
				requisition.Width += BackgroundTile.Width - min_length % BackgroundTile.Width;	

			requisition.Height = BackgroundTile.Height + (2 * y_offset);
			base.OnSizeRequested(ref requisition);
		}

		Pixbuf background_pixbuf;
		protected Pixbuf BackgroundPixbuf {
			get {
				if (background_pixbuf == null) {
					int length;
					if (Allocation.Width < min_length || !extendable)
						length = min_length;
					else
						length = Allocation.Width;

					length = length - length % BackgroundTile.Width;

					background_pixbuf = new Pixbuf (Gdk.Colorspace.Rgb, true, 8, length, BackgroundTile.Height);
					for (int i = 0; i < length; i += BackgroundTile.Width) {
						BackgroundTile.CopyArea (0, 0, BackgroundTile.Width, BackgroundTile.Height, 
								background_pixbuf, i, 0);
					}
				}
				return background_pixbuf;
			}
			set {
				if (background_pixbuf != value && background_pixbuf != null) {
					background_pixbuf.Dispose ();
					background_pixbuf = value;
				}
			}
		}

	
		Hashtable start_indexes;
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			if (evnt.Window != GdkWindow)
				return true;

			if (extendable && Allocation.Width >= BackgroundPixbuf.Width + (2 * x_offset) + BackgroundTile.Width)
				BackgroundPixbuf = null;

			if (extendable && Allocation.Width < BackgroundPixbuf.Width + (2 * x_offset))
				BackgroundPixbuf = null;

			int xpad = 0, ypad = 0;
			if (Allocation.Width > BackgroundPixbuf.Width + (2 * x_offset))
				xpad = (int) (x_align * (Allocation.Width - (BackgroundPixbuf.Width + (2 * x_offset))));

			if (Allocation.Height > BackgroundPixbuf.Height + (2 * y_offset))
				ypad = (int) (y_align * (Allocation.Height - (BackgroundPixbuf.Height + (2 * y_offset))));

			GdkWindow.DrawPixbuf (Style.BackgroundGC (StateType.Normal), BackgroundPixbuf, 
					0, 0, x_offset + xpad, y_offset + ypad, 
					BackgroundPixbuf.Width, BackgroundPixbuf.Height, Gdk.RgbDither.None, 0, 0);

			//drawing the icons...
			start_indexes = new Hashtable ();

			Pixbuf icon_pixbuf = new Pixbuf (Gdk.Colorspace.Rgb, true, 8, BackgroundPixbuf.Width, thumb_size);
			icon_pixbuf.Fill (0x00000000);

			Pixbuf current = GetPixbuf ((int) Math.Round (Position));
			int ref_x = (int)(icon_pixbuf.Width / 2.0 - current.Width * (Position + 0.5f - Math.Round (Position))); //xpos of the reference icon

			int start_x = ref_x;
			for (int i = (int) Math.Round (Position); i < selection.Collection.Count; i++) {
				current = GetPixbuf (i, ActiveItem == i);
				current.CopyArea (0, 0, Math.Min (current.Width, icon_pixbuf.Width - start_x) , current.Height, icon_pixbuf, start_x, 0);
				start_indexes [start_x] = i; 
				start_x += current.Width + spacing;
				if (start_x > icon_pixbuf.Width)
					break;
			}

			start_x = ref_x;
			for (int i = (int) Math.Round (Position) - 1; i >= 0; i--) {
				current = GetPixbuf (i, ActiveItem == i);
				start_x -= (current.Width + spacing);
				current.CopyArea (Math.Max (0, -start_x), 0, Math.Min (current.Width, current.Width + start_x), current.Height, icon_pixbuf, Math.Max (start_x, 0), 0);
				start_indexes [Math.Max (0, start_x)] = i; 
				if (start_x < 0)
					break;
			}
			
			GdkWindow.DrawPixbuf (Style.BackgroundGC (StateType.Normal), icon_pixbuf,
					0, 0, x_offset + xpad, y_offset + ypad + thumb_offset,
					icon_pixbuf.Width, icon_pixbuf.Height, Gdk.RgbDither.None, 0, 0);

			icon_pixbuf.Dispose ();

			return true;
		}

		protected override bool OnScrollEvent (EventScroll args)
		{
			switch (args.Direction) {
			case ScrollDirection.Up:
			case ScrollDirection.Right:
				Position -= 0.5f;
				return true;
			case Gdk.ScrollDirection.Down:
			case Gdk.ScrollDirection.Left:
				Position += 0.5f;
				return true;
			}
			return false;
		}

		public delegate void PositionChangedHandler (float position);

		protected virtual void OnPositionChanged (float position)
		{
			this.position = position;
			QueueDraw ();
		}

		public void HandlePointerChanged (BrowsablePointer pointer, BrowsablePointerChangedArgs old)
		{
			Position = ActiveItem;
		}

		public void HandleCollectionChanged (IBrowsableCollection coll)
		{
			Position = ActiveItem;
			QueueDraw ();
		}

		public void HandleCollectionItemsChanged (IBrowsableCollection coll, BrowsableEventArgs args)
		{
			//FIXME: need to be smarter here...

			//invalidate the thumbs cache
			thumb_cache.Dispose ();
			thumb_cache = new ThumbnailCache (30);
			QueueDraw ();
		}

		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			if(evnt.Button != 1) 
				return false;
			HasFocus = true;
			int pos = -1;
			foreach (int key in start_indexes.Keys)
				if (key <= evnt.X && key > pos)
					pos = key;
			try {
				ActiveItem = (int)start_indexes [pos];
				return true;
			} catch {
				return false;
			}
		}
 
 		protected Pixbuf GetPixbuf (int i)
		{
			return GetPixbuf (i, false);
		}

 		protected virtual Pixbuf GetPixbuf (int i, bool highlighted)
		{
			Pixbuf current;
			string thumb_path = FSpot.ThumbnailGenerator.ThumbnailPath ((selection.Collection [i]).DefaultVersionUri);
			current = thumb_cache.GetThumbnailForPath (thumb_path);
			if (current == null) {
				current = new Pixbuf (thumb_path, -1, ThumbSize);
				thumb_cache.AddThumbnail (thumb_path, current);
			}

			if (!highlighted)
				return current;

			Pixbuf highlight = new Pixbuf (Gdk.Colorspace.Rgb, true, 8, current.Width, current.Height);
			Gdk.Color color = Style.Background (StateType.Selected);
			uint ucol = (uint)((uint)color.Red / 256 << 24 ) + ((uint)color.Green / 256 << 16) + ((uint)color.Blue / 256 << 8) + 255;
			highlight.Fill (ucol);
			current.CopyArea (1, 1, current.Width - 2, current.Height - 2, highlight, 1, 1);	
			return highlight;
		}

		~Filmstrip ()
		{
			this.Dispose ();
		}
			
		public override void Dispose ()
		{
			lock (this) {
				if (background_pixbuf != null)
					background_pixbuf.Dispose ();
				if (background_tile != null)
					background_tile.Dispose ();
			}
			background_pixbuf = null;
			background_tile = null;
			System.GC.SuppressFinalize (this);
		}

		public interface IAnimator
		{
			void MoveTo (float target);
		}

		public class DirectAnimator : IAnimator
		{
			PositionChangedHandler handler;

			public DirectAnimator (PositionChangedHandler handler)
			{
				this.handler = handler;
			}

			public void MoveTo (float target)
			{
				handler (target);
			}
		}

		public class ConstantSpeedAnimator : IAnimator
		{
			PositionChangedHandler handler;
			float speed = 20f; // images/second
			uint interval = 40;
			float target;
			Filmstrip filmstrip;

			public ConstantSpeedAnimator (Filmstrip filmstrip, PositionChangedHandler handler)
			{
				this.handler = handler;
				this.filmstrip = filmstrip;
			}

			public void MoveTo (float target)
			{
				this.target = target;
				GLib.Timeout.Add (interval, new GLib.TimeoutHandler (Step)); 
			}

			bool Step ()
			{
				float increment = speed * interval / 1000f;
				if (Math.Abs (filmstrip.Position - target) < increment) {
					handler (target);
					return false;
				}
				if (target > filmstrip.Position)
					handler (filmstrip.Position + increment);
				else
					handler (filmstrip.Position - increment);

				return true;
			}
		}

		public class AcceleratedAnimator : IAnimator
		{
			PositionChangedHandler handler;
			Filmstrip filmstrip;
			uint interval = 10;
			float target;
			float speed;
			float acc = 50f; //images/second^2

			public AcceleratedAnimator (Filmstrip filmstrip, PositionChangedHandler handler)
			{
				this.handler = handler;
				this.filmstrip = filmstrip;
			}

			public void MoveTo (float target)
			{
				this.target = target;
				GLib.Timeout.Add (interval, new GLib.TimeoutHandler (Step)); 	
			}

			bool Step ()
			{
				float distance = Math.Abs (filmstrip.Position - target);
				if ( distance < speed * speed / acc )	//SLOW DOWN!
					speed -= acc * interval / 1000f;

				else	//SPEED UP
					speed += acc * interval / 1000f;

				float increment = speed * interval / 1000f;

				if (Math.Abs (distance - increment) < 0.0001) {
					handler (target);
					return false;
				}

				if (target > filmstrip.Position)
					handler (filmstrip.Position + increment);
				else
					handler (filmstrip.Position - increment);

				return true;
			}
		}
	}
}
