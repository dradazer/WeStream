using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WeStream.DataModel
{
    public class PicInfo
    {
        public string picTitle { get; set; }
        public DateTime created { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DataSource
    {
        private ObservableCollection<PicInfo> _picInfo;
        const string fileName = "picinfo.json";

        public DataSource()
        {
            _picInfo = new ObservableCollection<PicInfo>();
        }

        public async Task<ObservableCollection<PicInfo>> getPicInfo()
        {
            //wait for ensureDataLoaded to be completed then return ObservableCollection _picInfo
            await ensureDataLoaded();
            return _picInfo;
        }

        private async Task ensureDataLoaded()
        {
            if (_picInfo.Count == 0)
                await getPicInfoAsync();
            return;
        }

        private async Task getPicInfoAsync()
        {
            if (_picInfo.Count != 0)
                return;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<PicInfo>));

            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    //try to read the stream object
                    _picInfo = (ObservableCollection<PicInfo>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                _picInfo = new ObservableCollection<PicInfo>();
            }
        }

        public async void addPicInfo(PicInfo picInfo)
        {
            //add picInfo to the ObservableCollection
            _picInfo.Add(picInfo);
            await savePicInfoDataAsync();
        }

        private async Task savePicInfoDataAsync()
        {
            //write into the json file replacing existing json file
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<PicInfo>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _picInfo);
            }
        }


    }
}
