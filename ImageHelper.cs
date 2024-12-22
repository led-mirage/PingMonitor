// © 2024 led-mirage. All rights reserved.

using System.Drawing.Drawing2D;

public static class ImageHelper
{
    public static Image ResizeImageWithAntialiasing(Image originalImage, Size targetSize)
    {
        Bitmap resizedImage = new Bitmap(targetSize.Width, targetSize.Height);
        using (Graphics g = Graphics.FromImage(resizedImage))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(originalImage, 0, 0, targetSize.Width, targetSize.Height);
        }
        return resizedImage;
    }
}