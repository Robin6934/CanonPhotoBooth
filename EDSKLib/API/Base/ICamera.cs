using EOSDigital.API;
using EOSDigital.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSKLib.API
{
    public interface ICamera
    {
        public event LiveViewUpdate LiveViewUpdated;

        public event DownloadHandler DownloadReady;

        public void OpenSession();

        public void CloseSession();

        public void Dispose();

        public void TakePhoto();

        public void TakePhotoAsync();

        public void DownloadFile(DownloadInfo Info, string directory);

        public void SendCommand(CameraCommand command, int inParam = 0);

        public void StartLiveView();

        public void SetCapacity(int bytesPerSector, int numberOfFreeClusters);

        public void SetSetting(PropertyID propID, object value, int inParam = 0);

    }
}
