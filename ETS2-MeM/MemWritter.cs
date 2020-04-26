using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Capture;
using Capture.Hook;
using Capture.Interface;
using ETS2_MeM.Properties; //For overlay

namespace ETS2_MeM.Hooks
{
    static class MemWritter
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        public static int Width = 0;
        public static int Height = 0;

        public static CaptureProcess CaptureProcess = null;

        public static System.Timers.Timer Timer = new System.Timers.Timer();

        public static void DrawMEM(string text, Brush color1, string coloredText, Brush color2,  string stringPosition, string logoPath, int showTime)
        {
            try
            {
                if (CaptureProcess == null)
                {
                    AttachProcess(Program.currentGame == "ets2" ? "eurotrucks2" : "amtrucks");
                    Log.Write("No capture process bound");
                    if (CaptureProcess == null)
                    {
                        return;
                    }
                }

                Rect rectangle = new Rect();
                GetWindowRect(CaptureProcess.Process.MainWindowHandle, ref rectangle);
                Width = rectangle.Right - rectangle.Left;
                Height = rectangle.Bottom - rectangle.Top;

                Image bmp = new Bitmap(Resources.overlay_double);

                RectangleF rectf = new RectangleF(0, 0, bmp.Width, bmp.Height);

                Graphics g = Graphics.FromImage(bmp);
                
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var font = new Font("Microsoft Sans Serif", 15, FontStyle.Bold);

                var stringSize = g.MeasureString(text + " " + coloredText, font);
                var TextSize = g.MeasureString(text + " ", font);
                var coloredTextSize = g.MeasureString(coloredText, font);
                PointF topLeft = new PointF((512 / 2) - (stringSize.Width / 2) + 123,
                        (bmp.Height / 2) - (stringSize.Height / 2));
                if (stringPosition == "left")
                {
                    topLeft = new PointF((bmp.Width / 3) - (stringSize.Width),
                        (bmp.Height / 2) - (stringSize.Height / 2));
                }


                g.DrawString(coloredText, font, color2, new PointF(topLeft.X + TextSize.Width, topLeft.Y));
                g.DrawString(text, font, Brushes.White, topLeft);

                if (logoPath != null)
                {
                    try
                    {
                        var logo = new Bitmap(logoPath);

                        var logoHeight = (float)logo.Height;
                        var logoWidth = (float)logo.Width;
                        if (logoHeight > 0.41f * logoWidth)
                        {
                            logoWidth = (float)((90f / logoHeight) * logoWidth);
                            logoHeight = 90;
                        }
                        else if (logoHeight <= 0.41f * logoWidth)
                        {
                            logoHeight = (float)((220f / logoWidth) * logoHeight);
                            logoWidth = 220;
                        }

                        g.DrawImage(logo, (256 / 2) - (logoWidth / 2) + 645, (bmp.Height / 2) - (logoHeight / 2), logoWidth,
                            logoHeight);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(logoPath);
                        Log.Write(ex.ToString());
                    }
                }

                g.Flush();

                var overlay = new Capture.Hook.Common.Overlay
                {
                    Elements = new List<Capture.Hook.Common.IOverlayElement>
                        {
                            new Capture.Hook.Common.ImageElement()
                            {
                                Location = new Point((Width / 2) - (bmp.Width / 2), (Height / 4)),
                                Image = bmp.ToByteArray(System.Drawing.Imaging.ImageFormat.Png)
                            }
                        },
                    Hidden = false
                };
                CaptureProcess.CaptureInterface.DrawOverlayInGame(overlay);
                bmp.Dispose();

                Timer.Interval = showTime; //Overlay will show for 4 seconds
                Timer.Elapsed += (sender, args) =>
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                    overlay.Hidden = true;
                    Log.Write("Hide overlay");
                    CaptureProcess?.CaptureInterface.DrawOverlayInGame(
                        new Capture.Hook.Common.Overlay
                        {
                            Elements = new List<Capture.Hook.Common.IOverlayElement>()
                        }
                    );
                };
                Timer.Enabled = true;
                Timer.Start();
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
            }
        }

        public static void AttachProcess(string name)
        {
            if (CaptureProcess != null)
            {
                HookManager.RemoveHookedProcess(CaptureProcess.Process.Id);
                CaptureProcess.CaptureInterface.Disconnect();
                CaptureProcess = null;
            }

            try
            {
                Process[] processes = Process.GetProcessesByName(name);
                foreach (Process p in processes)
                {
                    if (p.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }

                    if (HookManager.IsHooked(p.Id))
                    {
                        continue;
                    }

                    CaptureConfig cc = new CaptureConfig()
                    {
                        Direct3DVersion = Direct3DVersion.AutoDetect,
                        ShowOverlay = true
                    };

                    var captureInterface = new CaptureInterface();
                    CaptureProcess = new CaptureProcess(p, cc, captureInterface);
                }
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
            }
        }
    }
}
