using Camera.MAUI;
using System.Reflection.Metadata.Ecma335;
using Camera.MAUI.ZXing;
using Grpc.Core;
using Grpc.Net.Client;
//using GrpcPhotos;
using GrpcEvent;

namespace QRtest
{
    public partial class TakePhotoPage : ContentPage
    {
        public TakePhotoPage()
        {
            InitializeComponent();
            //InitializeCameraView();
            //this.Appearing += MainPage_Appearing;
            //this.Disappearing += MainPage_Disappearing;

            //cameraView.BarcodeDetected += cameraView_BarcodeDetected;
            //cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            //cameraView.BarCodeOptions = new()
            //{
            //    //PossibleFormats = { BarcodeFormat.QR_CODE },
            //    ReadMultipleCodes = false,
            //    TryHarder = true,
            //    TryInverted = true,
            //    AutoRotate = true
            //};
            //cameraView.BarCodeDetectionFrameRate = 10;
            //cameraView.BarCodeDetectionMaxThreads = 5;
            //cameraView.ControlBarcodeResultDuplicate = true;
            //cameraView.BarCodeDetectionEnabled = true;
        }

        private void InitializeCameraView()
        {
            //cameraView.BarcodeDetected += cameraView_BarcodeDetected;
            //cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            //cameraView.BarCodeOptions = new()
            //{
            //    //PossibleFormats = { BarcodeFormat.QR_CODE },
            //    ReadMultipleCodes = false,
            //    TryHarder = true,
            //    TryInverted = true,
            //    AutoRotate = true
            //};
            //cameraView.BarCodeDetectionFrameRate = 10;
            //cameraView.BarCodeDetectionMaxThreads = 5;
            //cameraView.ControlBarcodeResultDuplicate = true;
            //cameraView.BarCodeDetectionEnabled = true;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            Shell.Current.CurrentItem.SetValue(Shell.TabBarIsVisibleProperty, false);

            Title = EventInfoManager.EventName;

            //Shell.Current.GoToAsync("MainPage");

            if (cameraView != null)
            {
                cameraViewContainer.Children.Remove(cameraView);
                cameraView = null;
            }

            cameraView = new CameraView
            {
                WidthRequest = 400,
                HeightRequest = 400,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BarCodeDetectionEnabled = true,
            };

            cameraView.CamerasLoaded += cameraView_CamerasLoaded;
            //cameraView.BarcodeDetected += cameraView_BarcodeDetected;
            //cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            //cameraView.BarCodeOptions = new()
            //{
            //    //PossibleFormats = { BarcodeFormat.QR_CODE },
            //    ReadMultipleCodes = false,
            //    TryHarder = true,
            //    TryInverted = true,
            //    AutoRotate = true
            //};
            //cameraView.BarCodeDetectionFrameRate = 10;
            //cameraView.BarCodeDetectionMaxThreads = 5;
            //cameraView.ControlBarcodeResultDuplicate = true;
            //cameraView.BarCodeDetectionEnabled = true;

            cameraViewContainer.Children.Add(cameraView);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (cameraView != null)
            {
                cameraView.CamerasLoaded -= cameraView_CamerasLoaded;
                //cameraView.BarcodeDetected -= cameraView_BarcodeDetected;
                cameraViewContainer.Children.Remove(cameraView);
                cameraView = null;
            }
        }

        //private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        barcodeResult.Text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
        //    });
        //}

        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras[0];

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);

                await cameraView.StopCameraAsync();
                var res = await cameraView.StartCameraAsync();
            });
        }

        private async void TakePhoto(object sender, EventArgs e)
        {
            var stream = await cameraView.TakePhotoAsync();
            if (stream != null)
            {
                var result = ImageSource.FromStream(() => stream);

                try
                {
                    var socketHttpHandler = new SocketsHttpHandler
                    {
                        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                        {
                            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                        },
                        EnableMultipleHttp2Connections = true
                    };

                    using (var channel = GrpcChannel.ForAddress($"https://{Globals.IP_ADDRESS}:7244", new GrpcChannelOptions
                    {
                        HttpHandler = socketHttpHandler
                    }))
                    {
                        var client = new PhotoEvent.PhotoEventClient(channel);
                        var metadata = new Metadata
                        {
                            { "eventId", EventInfoManager.EventId.ToString() },
                            { "email", "brygidapapier@gmail.com" }
                        };

                        using (var call = client.AddPhoto(metadata))
                        {
                            var buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                var chunk = new PhotoChunk { Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead) };
                                await call.RequestStream.WriteAsync(chunk);
                            }

                            await call.RequestStream.CompleteAsync();
                            var response = await call.ResponseAsync;
                            await DisplayAlert("Upload Status", response.Message, "OK");
                        }
                    }
                }
                catch (Grpc.Core.RpcException ex)
                {
                    await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
                }
            }
        }

        //private void Button_Clicked(object sender, EventArgs e)
        //{
        //    //string filePath = @"C:\Users\hijacky\Desktop\zdjecie.png";
        //    myImage.Source = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
        //    //bool result = cameraView.SaveSnapShot(Camera.MAUI.ImageFormat.PNG, filePath);
        //}

        //private async void TakePhoto(object sender, EventArgs e)
        //{
        //    var stream = await cameraView.TakePhotoAsync();
        //    if (stream != null)
        //    {
        //        var result = ImageSource.FromStream(() => stream);

        //        try
        //        {
        //            var socketHttpHandler = new SocketsHttpHandler
        //            {
        //                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        //                {
        //                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        //                },
        //                EnableMultipleHttp2Connections = true
        //            };

        //            using (var channel = GrpcChannel.ForAddress($"https://{Globals.IP_ADDRESS}:7244", new GrpcChannelOptions
        //            {
        //                HttpHandler = socketHttpHandler
        //            }))
        //            {
        //                var client = new PhotosA.PhotosAClient(channel);
        //                using (var call = client.UploadPhoto())
        //                {
        //                    var buffer = new byte[4096];
        //                    int bytesRead;
        //                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        //                    {
        //                        var chunk = new PhotoChunk { Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead) };
        //                        await call.RequestStream.WriteAsync(chunk);
        //                    }

        //                    await call.RequestStream.CompleteAsync();
        //                    var response = await call.ResponseAsync;
        //                    await DisplayAlert("Upload Status", response.Message, "OK");
        //                }
        //            }
        //        }
        //        catch (Grpc.Core.RpcException ex)
        //        {
        //            await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
        //        }
        //        catch (System.Net.Http.HttpRequestException ex)
        //        {
        //            await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
        //        }
        //        catch (Exception ex)
        //        {
        //            await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
        //        }
        //    }
        //}

        private async void BrowsePhotos(object sender, EventArgs e)
        {
            // todo tutaj najpierw zaciagniecie zdjec z folderu i ladne 
            //DisplayAlert("Browse photos", "Tutaj bedzie obserwacja zdjec", "OK");
            //await Navigation.PushAsync(new mobile_app.BrowsePage());
            await Shell.Current.GoToAsync("BrowsePage");
        }
    }

}
