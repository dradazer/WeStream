using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WeStream.DataModel;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WeStream
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamPage : Page
    {


        public StreamPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //GPS
            Geopoint myPoint;

            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            var position = await locator.GetGeopositionAsync();
            myPoint = position.Coordinate.Point;

            //Camera
            MediaCapture mediaCapture;

            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
            //assign to XAML element and start preview
            PhotoPreview.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();

        }

        public async Task CapturePhoto()
        {
            MediaCapture mediaCapture;

            mediaCapture = new MediaCapture();

            //creating jpeg properties
            var imgEncodingProperties = ImageEncodingProperties.CreateJpeg();
            imgEncodingProperties.Width = 640;
            imgEncodingProperties.Height = 480;

            //Create new picture and place into Local, if name existing create unique name
            var photoStorageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(titleBox.Text, CreationCollisionOption.GenerateUniqueName);

            await mediaCapture.CapturePhotoToStorageFileAsync(imgEncodingProperties, photoStorageFile);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            PicInfo newPicInfo = new PicInfo();
            newPicInfo.picTitle = titleBox.Text;
            newPicInfo.created = DateTime.Now;
            newPicInfo.Latitude = MyMap.Center.Position.Latitude;
            newPicInfo.Longitude = MyMap.Center.Position.Longitude;
            App.DataModel.addPicInfo(newPicInfo);
            await CapturePhoto();
        }
    }
}
