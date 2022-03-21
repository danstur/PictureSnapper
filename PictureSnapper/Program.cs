using Emgu.CV;

namespace PictureSnapper;

public static class Program
{
    public static void Main()
    {
        DsDevice
        var capture = new VideoCapture()
        var image = capture.QueryFrame();
        image.Save(@"C:\tmp\test.bmp");
    }
}
