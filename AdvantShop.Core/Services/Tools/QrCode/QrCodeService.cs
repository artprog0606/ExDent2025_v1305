using AdvantShop.Diagnostics;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AdvantShop.Tools.QrCode
{
    public static class QrCodeService
    {
        public static Stream GetQrCode(string url, int width = 8)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return null;

                Gma.QrCodeNet.Encoding.QrCode qrCode;

                var encoder = new QrEncoder(ErrorCorrectionLevel.M);
                encoder.TryEncode(url.Split(new[] { "?" }, StringSplitOptions.None)[0], out qrCode);

                var renderer = new GraphicsRenderer(new FixedModuleSize(width, QuietZoneModules.Two), Brushes.Black, Brushes.White);

                Stream stream = new MemoryStream();
                
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);

                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }
    }
}
