// © 2024 led-mirage. All rights reserved.

public static class ControlExtensions
{
    public static float GetScaleFactor(this Control ctrl)
    {
        return ctrl.DeviceDpi / 96f;
    }

    public static int ScaleValue(this Control ctrl, int original)
    {
        float scaleFactor = ctrl.GetScaleFactor();
        return (int)(original * scaleFactor);
    }

    public static Size ScaleSize(this Control ctrl, Size original)
    {
        float scaleFactor = ctrl.GetScaleFactor();
        return new Size((int)(original.Width * scaleFactor), (int)(original.Height * scaleFactor));
    }
}
