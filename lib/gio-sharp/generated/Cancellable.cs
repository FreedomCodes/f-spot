// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class Cancellable : GLib.Object {

		[Obsolete]
		protected Cancellable(GLib.GType gtype) : base(gtype) {}
		public Cancellable(IntPtr raw) : base(raw) {}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_cancellable_new();

		public Cancellable () : base (IntPtr.Zero)
		{
			if (GetType () != typeof (Cancellable)) {
				CreateNativeObject (new string [0], new GLib.Value[0]);
				return;
			}
			Raw = g_cancellable_new();
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void CancelledVMDelegate (IntPtr cancellable);

		static CancelledVMDelegate CancelledVMCallback;

		static void cancelled_cb (IntPtr cancellable)
		{
			try {
				Cancellable cancellable_managed = GLib.Object.GetObject (cancellable, false) as Cancellable;
				cancellable_managed.OnCancelled ();
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		private static void OverrideCancelled (GLib.GType gtype)
		{
			if (CancelledVMCallback == null)
				CancelledVMCallback = new CancelledVMDelegate (cancelled_cb);
			OverrideVirtualMethod (gtype, "cancelled", CancelledVMCallback);
		}

		[GLib.DefaultSignalHandler(Type=typeof(GLib.Cancellable), ConnectionMethod="OverrideCancelled")]
		protected virtual void OnCancelled ()
		{
			GLib.Value ret = GLib.Value.Empty;
			GLib.ValueArray inst_and_params = new GLib.ValueArray (1);
			GLib.Value[] vals = new GLib.Value [1];
			vals [0] = new GLib.Value (this);
			inst_and_params.Append (vals [0]);
			g_signal_chain_from_overridden (inst_and_params.ArrayPtr, ref ret);
			foreach (GLib.Value v in vals)
				v.Dispose ();
		}

		[GLib.Signal("cancelled")]
		public event System.EventHandler Cancelled {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (this, "cancelled");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (this, "cancelled");
				sig.RemoveDelegate (value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_disconnect(IntPtr raw, UIntPtr handler_id);

		public void Disconnect(ulong handler_id) {
			g_cancellable_disconnect(Handle, new UIntPtr (handler_id));
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_release_fd(IntPtr raw);

		public void ReleaseFd() {
			g_cancellable_release_fd(Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe bool g_cancellable_set_error_if_cancelled(IntPtr raw, out IntPtr error);

		public unsafe bool SetErrorIfCancelled() {
			IntPtr error = IntPtr.Zero;
			bool raw_ret = g_cancellable_set_error_if_cancelled(Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_push_current(IntPtr raw);

		public void PushCurrent() {
			g_cancellable_push_current(Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_cancellable_is_cancelled(IntPtr raw);

		public bool IsCancelled { 
			get {
				bool raw_ret = g_cancellable_is_cancelled(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_pop_current(IntPtr raw);

		public void PopCurrent() {
			g_cancellable_pop_current(Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_cancel(IntPtr raw);

		public void Cancel() {
			g_cancellable_cancel(Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern int g_cancellable_get_fd(IntPtr raw);

		public int Fd { 
			get {
				int raw_ret = g_cancellable_get_fd(Handle);
				int ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_cancellable_get_type();

		public static new GLib.GType GType { 
			get {
				IntPtr raw_ret = g_cancellable_get_type();
				GLib.GType ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_cancellable_get_current();

		public static GLib.Cancellable Current { 
			get {
				IntPtr raw_ret = g_cancellable_get_current();
				GLib.Cancellable ret = GLib.Object.GetObject(raw_ret) as GLib.Cancellable;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_cancellable_reset(IntPtr raw);

		public void Reset() {
			g_cancellable_reset(Handle);
		}

#endregion
	}
}
