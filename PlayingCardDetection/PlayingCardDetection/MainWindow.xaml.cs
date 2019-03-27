using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Cuda;

namespace PlayingCardDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCapture videoCapture;

        public MainWindow()
        {
            InitializeComponent();
            videoCapture = new VideoCapture(0);
            videoCapture.SetCaptureProperty(CapProp.FrameHeight,1080);
            videoCapture.SetCaptureProperty(CapProp.FrameWidth,1920);
        }

        private Mat GrabFrame()
        {
            Mat image = new Mat();
            //Capture frame by frame
            videoCapture.Read(image);
            return image;
        }

        public void btnClickMe_Click(Object sender, RoutedEventArgs e)
        {
            long matchTime;
            using (Mat modelImage = CvInvoke.Imread("card.png", ImreadModes.AnyColor))
            using (Mat observedImage = GrabFrame())
            {
                Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                Title = "Playing card detection - " + matchTime + "ms";
                VectorOfByte res = new VectorOfByte();
                CvInvoke.Imencode(".png", result, res);
                ImageDisplay.Source = ToImage(res.ToArray());
            }
        }

        public BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
