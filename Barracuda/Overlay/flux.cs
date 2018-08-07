using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Flux.Helpers;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.DirectWrite.Factory;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace Flux.Overlay
{
    public partial class flux : Form
    {
        public flux()
        {
            InitializeComponent();
            var sc = Screen.AllScreens;
            Bounds = sc[0].Bounds;
            BackColor = System.Drawing.Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = false;
            WindowState = FormWindowState.Maximized;
            Win32.SetWindowLong(Handle, Win32.GWL_EXSTYLE,
                (IntPtr)(Win32.GetWindowLong(Handle, Win32.GWL_EXSTYLE) ^ Win32.WS_EX_LAYERED ^
                          Win32.WS_EX_TRANSPARENT));
            Win32.SetLayeredWindowAttributes(Handle, 0, 255, Win32.LWA_ALPHA);

            var targetProperties = new HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new Size2(Bounds.Right - Bounds.Left, Bounds.Bottom - Bounds.Top),
                PresentOptions = PresentOptions.Immediately
            };

            var prop = new RenderTargetProperties(RenderTargetType.Hardware,
                new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), 0, 0, RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);

            var d3DFactory = new SharpDX.Direct2D1.Factory();

            var device = new WindowRenderTarget(d3DFactory, prop, targetProperties)
            {
                TextAntialiasMode = TextAntialiasMode.Cleartype,
                AntialiasMode = AntialiasMode.Aliased
            };

            var dxthread = new Thread(() =>
            {
                var dangerClose = new SolidColorBrush(device, RawColorFromColor(Color.Green));
                var black = new SolidColorBrush(device, RawColorFromColor(Color.Black));
                var farAway = new SolidColorBrush(device, RawColorFromColor(Color.Red));
                var darkGreen = new SolidColorBrush(device, RawColorFromColor(Color.DarkGreen));
                var red = new SolidColorBrush(device, RawColorFromColor(Color.DarkRed));
                var package = new SolidColorBrush(device, RawColorFromColor(Color.Purple));
                var vehicle = new SolidColorBrush(device, RawColorFromColor(Color.Orange));
                var fontFactory = new Factory();
                var fontEsp = new TextFormat(fontFactory, "Helvetica", FontWeight.DemiBold, FontStyle.Normal, 16);
                while (true)
                {
                    device.BeginDraw();
                    device.Clear(null);
                    try
                    {
                        // Note: this will throw an error anyways if not defined...
                        H.Players.Any();
                    }
                    catch
                    {
                        device.EndDraw();
                        Thread.Sleep(1000 / 144);
                        continue;
                    }

                    foreach (var player in H.Players.ToList())
                    {
                        var distance = H.ViewTranslation.Distance(player.Position);

                        foreach (var bone in player.W2SBones)
                        {
                            //check to see if bone visible on screen
                            if (bone.Z < 1.0f)
                            {
                                continue;
                            }

                            DrawText(".", (int) bone.X, (int) bone.Y, dangerClose, fontFactory, fontEsp, device);
                        }

                        //Head location
                        DrawText("O", (int) player.W2SHead.X, (int) player.W2SHead.Y, package, fontFactory, fontEsp,
                            device);

                        //Player name, distance, and health
                        DrawText($"{player.Name}", (int) player.W2SHead.X + 50, (int) player.W2SHead.Y, package,
                            fontFactory, fontEsp, device);
                        DrawText($"{distance:F}", (int) player.W2SHead.X + 50, (int) player.W2SHead.Y + 15,
                            package, fontFactory, fontEsp, device);
                        DrawText($"{player.Health:F}", (int) player.W2SHead.X + 50,
                            (int) player.W2SHead.Y + 30, package, fontFactory, fontEsp, device);

                        if (player.Health <= 0 || player.Health >= 200 || !(player.W2SHead.Z >= 1.0f)) continue;
                        var x = (player.W2SHead.X - H.Aimbot.CrosshairX);
                        var y = (player.W2SHead.Y - H.Aimbot.CrosshairY);

                        var distanceFromXHair = (float) Math.Sqrt(x * x + y * y);
                        if (distanceFromXHair <= H.MaxFOV)
                        {
                            H.Aimbot.GetClosestTargetFromCrosshair(player);
                        }
                    }

                    H.Aimbot.LockonPlayer(H.Aimbot.TempClosestTarget);

                    if (H.TriggerEnabled)
                    {
                        DrawText("Trigger Enabled", 0, 0, dangerClose, fontFactory, fontEsp, device);
                    }

                    if (H.AimbotEnabled)
                    {
                        DrawText("Aimbot Enabled", 0, 15, dangerClose, fontFactory, fontEsp, device);
                    }

                    if (!String.IsNullOrEmpty(H.CurrentTargetName) && H.AimbotEnabled)
                    {
                        DrawText(H.CurrentTargetName, 0, 30, dangerClose, fontFactory, fontEsp, device);
                    }
                    device.EndDraw();
                    Thread.Sleep(1000 / 144);
                }
            }) {IsBackground = true};
            dxthread.Start();
        }

        public sealed override System.Drawing.Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        private static RawColor4 RawColorFromColor(Color color)
        {
            return new RawColor4(color.R, color.G, color.B, color.A);
        }

        private static TextLayout TextLayout(string szText, Factory factory, TextFormat font)
        {
            return new TextLayout(factory, szText, font, float.MaxValue, float.MaxValue);
        }

        private static void DrawText(string szText, int x, int y, Brush foregroundBroush, Factory fontFactory, TextFormat font,
            RenderTarget device)
        {
            var tempTextLayout = TextLayout(szText, fontFactory, font);
            device.DrawTextLayout(new RawVector2(x, y), tempTextLayout, foregroundBroush, DrawTextOptions.NoSnap);
            tempTextLayout.Dispose();
        }
    }
}