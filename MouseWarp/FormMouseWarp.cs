using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseWarp
{
    public partial class FormMouseWarp : Form
    {
        private readonly IKeyboardMouseEvents _hook;

        public FormMouseWarp()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var pngResourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.Contains(".png"));
            var handle = default(IntPtr);
            using (var stream = assembly.GetManifestResourceStream(pngResourceName))
            {
                var image = (Bitmap)Image.FromStream(stream);
                handle = image.GetHicon();
            }

            TrayIcon.Icon = Icon.FromHandle(handle);
            DestroyIcon(handle);

            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            // https://github.com/gmamaladze/globalmousekeyhook
            _hook = Hook.GlobalEvents();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern private static bool DestroyIcon(IntPtr handle);

        private void OnMouseMoveExt(object sender, MouseEventExtArgs e)
        {
            var screen = Screen.AllScreens.FirstOrDefault(s => e.X >= s.Bounds.Left && e.X <= s.Bounds.Right);
            if (screen == null)
                return;

            if (e.Y <= screen.Bounds.Top)
                Cursor.Position = new Point(e.X, screen.Bounds.Top - 1);
            else if (e.Y >= screen.Bounds.Bottom)
                Cursor.Position = new Point(e.X, screen.Bounds.Bottom - 1);
        }

        private void FormMouseWarp_Load(object sender, EventArgs e)
        {
            _hook.MouseMoveExt += OnMouseMoveExt;
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
