using Camera.MAUI;
using System.Reflection.Metadata.Ecma335;
using Camera.MAUI.ZXing;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcPhotos;
using System.Text.Json;
using util.PhotoEvent;

namespace QRtest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
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
            cameraView.BarcodeDetected += cameraView_BarcodeDetected;
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new()
            {
                //PossibleFormats = { BarcodeFormat.QR_CODE },
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true,
                AutoRotate = true
            };
            cameraView.BarCodeDetectionFrameRate = 10;
            cameraView.BarCodeDetectionMaxThreads = 5;
            cameraView.ControlBarcodeResultDuplicate = true;
            cameraView.BarCodeDetectionEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Shell.Current.CurrentItem.SetValue(Shell.TabBarIsVisibleProperty, true);

            barcodeResult.Text = string.Empty;

            if (cameraView != null)
            {
                cameraViewContainer.Children.Remove(cameraView);
                cameraView = null; 
            }
            //var screenHeight = DeviceDisplay.MainDisplayInfo.Height;
            //var screenWidth = DeviceDisplay.MainDisplayInfo.Width;

            cameraView = new CameraView
            {
                WidthRequest = 400,
                HeightRequest = 400,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BarCodeDetectionEnabled = true,
            };

            cameraView.CamerasLoaded += cameraView_CamerasLoaded;
            cameraView.BarcodeDetected += cameraView_BarcodeDetected;
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new()
            {
                //PossibleFormats = { BarcodeFormat.QR_CODE },
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true,
                AutoRotate = true
            };
            cameraView.BarCodeDetectionFrameRate = 10;
            cameraView.BarCodeDetectionMaxThreads = 5;
            cameraView.ControlBarcodeResultDuplicate = true;
            cameraView.BarCodeDetectionEnabled = true;

            cameraViewContainer.Children.Add(cameraView);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (cameraView != null)
            {
                cameraView.CamerasLoaded -= cameraView_CamerasLoaded;
                cameraView.BarcodeDetected -= cameraView_BarcodeDetected;
                cameraViewContainer.Children.Remove(cameraView);
                cameraView = null; 
            }
        }
        private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    loadingIndicator.IsRunning = true;
                    loadingIndicator.IsVisible = true;

                    string qrContent = args.Result[0].Text;
                    var jsonDocument = JsonSerializer.Deserialize<PhotoEventPayload>(qrContent);

                    if (jsonDocument != null)
                    {
                        barcodeResult.Text = jsonDocument.Name;

                        //TODO walidacja czy expired ED

                        await Task.Delay(1000);
                        await Shell.Current.GoToAsync("TakePhotoPage");
                    }
                    else
                    {
                        await DisplayAlert("Scan QR Code", "Invalid QR code (1).", "OK");
                    }

                    //TUTAJ PRZESLANIE DO SERWERA
                    //{"ID":"296e2c35e9694850bc710c60b61611f2","N":"aaa","ED":"2025-01-15T20:40:18"}

                    // TUTAJ ZAPYTANIE PRZESYLANE DALEJ PO GRPC CZY KOD QR OK - czy ID exists
                    // przeslanie tylko ID? i czy valid - i serwer robi check i sprawdza czy valid
                    // od razu linki do zdjec


                    // tablica linkow
                }
                catch (JsonException)
                {
                    //barcodeResult.Text = "Invalid QR code (2).";
                    await DisplayAlert("Scan QR Code", "Invalid QR code (2).", "OK");
                }
                catch (Exception ex)
                {
                    //barcodeResult.Text = $"An error occurred: {ex.Message}";
                    await DisplayAlert("Scan QR Code", $"An error occurred: {ex.Message}", "OK");
                }
                finally
                {
                    loadingIndicator.IsRunning = false;
                    loadingIndicator.IsVisible = false;
                }

            });
        }

        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras[0];

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);

                await cameraView.StopCameraAsync();
                var res = await cameraView.StartCameraAsync();

                //if (res != Camera.MAUI.CameraResult.Success)
                //{
                    
                //}
            });
        }

        //private void Button_Clicked(object sender, EventArgs e)
        //{
        //    //string filePath = @"C:\Users\hijacky\Desktop\zdjecie.png";
        //    myImage.Source = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
        //    //bool result = cameraView.SaveSnapShot(Camera.MAUI.ImageFormat.PNG, filePath);
        //}

        //private async void Button_Clicked(object sender, EventArgs e)
        //{
        //    var stream = await cameraView.TakePhotoAsync();
        //    if (stream != null)
        //    {
        //        var result = ImageSource.FromStream(() => stream);
        //        //snapPreview.Source = result;

        //        var socketHttpHandler = new SocketsHttpHandler
        //        {
        //            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        //            {
        //                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        //            },
        //            EnableMultipleHttp2Connections = true
        //        };

        //        using (var channel = GrpcChannel.ForAddress("https://192.168.0.30:7244", new GrpcChannelOptions
        //        {
        //            HttpHandler = socketHttpHandler
        //        }))
        //        {
        //            var client = new PhotosA.PhotosAClient(channel);
        //            using (var call = client.UploadPhoto())
        //            {
        //                var buffer = new byte[4096];
        //                int bytesRead;
        //                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        //                {
        //                    var chunk = new PhotoChunk { Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead) };
        //                    await call.RequestStream.WriteAsync(chunk);
        //                }

        //                await call.RequestStream.CompleteAsync();
        //                var response = await call.ResponseAsync;
        //                await DisplayAlert("Upload Status", response.Message, "OK");
        //            }
        //        }
        //    }
        //}

    }

}
