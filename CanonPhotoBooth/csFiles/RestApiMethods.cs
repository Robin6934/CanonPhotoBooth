using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Threading;

namespace PhotoBooth
{
    internal class RestApiMethods
    {

        private static CancellationTokenSource? cancellationTokenSource;

        public static async void Init()
        {
            await RestApi.RestApiGet("http://localhost:6969/PhotoBoothApi/Init");
        }

        public static async Task PolingForPictureTrigger(MainWindow mainWindow, string dir)
        {
            cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(async () =>
            {
                while(!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Perform the GET request asynchronously
                    using HttpResponseMessage response =  await RestApi.RestApiGetReturn("http://localhost:6969/PhotoBoothCommunication/Poling");
                    
                    string responseString = await response.Content.ReadAsStringAsync();

                    PolingDTO? polingDTO = JsonSerializer.Deserialize<PolingDTO>(responseString);

                    if (polingDTO?.triggerPicture == true)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // This code will run on the UI thread
                            mainWindow.TriggerPicture();
                        });
                    }

                    if (polingDTO?.printPictureName != "")
                    {
                        string ImagePath = dir + "\\Photos\\" + polingDTO?.printPictureName;
                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            // This code will run on the UI thread
                            await PhotoBoothLib.PrintImageAsync(ImagePath);
                        });
                    }

                    await Task.Delay(1000,cancellationTokenSource.Token);
                }
                
            });
        }

        public static async void StartPolingForPicture(MainWindow mainWindow, string dir)
        {
            // Perform the GET request asynchronously
            await PolingForPictureTrigger(mainWindow, dir);
        }

        public static void StopPolingForPicture()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
