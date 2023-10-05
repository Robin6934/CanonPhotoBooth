﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using System.Text.Json;

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

        public static async Task PolingForPictureTrigger(MainWindow mainWindow, string dir)
        {
            await Task.Run(async () =>
            {
                while(true)
                {
                    // Perform the GET request asynchronously
                    HttpResponseMessage response =  await RestApi.RestApiGetReturn("http://localhost:6969/TriggerPicture/Poling");
                    
                    string responseString = await response.Content.ReadAsStringAsync();

                    PolingDTO polingDTO = JsonSerializer.Deserialize<PolingDTO>(responseString);

                    if (polingDTO.triggerPicture == true)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // This code will run on the UI thread
                            mainWindow.TriggerPicture();
                        });
                    }

                    if (polingDTO.printPictureName != "")
                    {
                        string ImagePath = dir + "\\Photos\\" + polingDTO.printPictureName;
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // This code will run on the UI thread
                            PhotoBoothLib.PrintImageAsync(ImagePath);
                        });
                    }

                    await Task.Delay(1000);
                }
                
            });
        }

        public static async void StartPolingForPicture(MainWindow mainWindow, string dir)
        {
            // Perform the GET request asynchronously
            await PolingForPictureTrigger(mainWindow, dir);
        }
    }
}
