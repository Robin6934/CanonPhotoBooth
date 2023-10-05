using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;

namespace PhotoBooth
{
    internal class RestApiMethods
    {

        public static async void Init(string dir, MainWindow mainWindow)
        {
            PhotoBoothInit photoBoothInit = new PhotoBoothInit();

            photoBoothInit.picturePath = dir+"\\Photos";

            await RestApi.RestApiPost("http://localhost:6969/PhotoBoothApi/Init", photoBoothInit);
        }

        public static async Task NewPictureTakenAsync(string dir)
        {
            await Task.Run(async () =>
            {
                // Perform the GET request asynchronously
                await RestApi.RestApiGet("http://localhost:6969/PhotoBoothApi/newPicture?PicturePath=" + dir);
            });
        }

        public static async Task PolingForPictureTrigger(MainWindow mainWindow)
        {
            await Task.Run(async () =>
            {
                while(true)
                {
                    // Perform the GET request asynchronously
                    HttpResponseMessage response =  await RestApi.RestApiGetReturn("http://localhost:6969/TriggerPicture/Poling");
                    
                    string responseString = await response.Content.ReadAsStringAsync();

                    if(responseString == "true")
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // This code will run on the UI thread
                            mainWindow.TriggerPicture();
                        });
                    }

                    await Task.Delay(1000);
                }
                
            });
        }

        public static async void StartPolingForPicture(MainWindow mainWindow)
        {
            // Perform the GET request asynchronously
            await PolingForPictureTrigger(mainWindow);
        }
    }
}
