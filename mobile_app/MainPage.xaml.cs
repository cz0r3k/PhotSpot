using Camera.MAUI;
using System.Reflection.Metadata.Ecma335;
using Camera.MAUI.ZXing;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcPhotos;
using System.Text.Json;
using util.PhotoEvent;
using GrpcEvent;
using System;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using GrpcUser;
using System.Net.Mail;

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
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            string nick = NickEntry.Text;
            string email = EmailEntry.Text;

            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Please fill in both fields!", "OK");
                return;
            }
            try
            {
                var mailAddress = new MailAddress(email);
            }
            catch (FormatException)
            {
                await DisplayAlert("Error", "Please provide correct email address.", "OK");
                return;
            }
            Preferences.Set("Nickname", nick);
            Preferences.Set("Email", email);

            await RegisterNewUser(nick, email);

            //RegisterInsecureRequest rpc TODO
            ModalForm.IsVisible = false;
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
            //Preferences.Default.Clear(); // todo wywalic!

            // ew zamiast tego IsRegistered wywolanie

            if (Preferences.Get("Nickname", "") == "" || Preferences.Get("Email", "") == "")
            {
                ModalForm.IsVisible = true;
            }
            else
            {
                cameraViewContainer.IsVisible = true;
            }


            Shell.Current.CurrentItem.SetValue(Shell.TabBarIsVisibleProperty, true);


            if (cameraView != null)
            {
                cameraViewContainer.Children.Remove(cameraView);
                cameraView = null; 
            }
            //var screenHeight = DeviceDisplay.MainDisplayInfo.Height;
            //var screenWidth = DeviceDisplay.MainDisplayInfo.Width;

            cameraView = new CameraView
            {
                //WidthRequest = 300,
                //HeightRequest = 300,
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
                        
                        if (await IsEventActive(jsonDocument.EventId))
                        {
                            EventInfoManager.EventId = jsonDocument.EventId;
                            EventInfoManager.EventName = jsonDocument.Name;
                        }
                        else
                        {
                            throw new InvalidOperationException("Event is not active.");
                        }

                        // GetPhotoLinks
                        if (!EventInfoManager.EventId.HasValue)
                        {
                            throw new InvalidOperationException("Event ID is empty.");
                        }
                        PhotoGuidManager.PhotoGuids = await GetPhotoLinks((Guid)EventInfoManager.EventId);
                        //string photoGuidsString = string.Join(" ", photoGuids);
                        //await DisplayAlert("Photo IDs:", photoGuidsString, "OK"); // tylko debug
                        

                        //TODO * walidacja czy expired ED

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


        //        message RegisterInsecureRequest
        //        {
        //  string name = 1;
        //        string email = 2;
        //}
        private async Task<bool> RegisterNewUser(string name, string email)
        {
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
                    var client = new UserManagement.UserManagementClient(channel);

                    var resp = client.RegisterInsecure(new RegisterInsecureRequest{ Name = name, Email = email });
                    // tutaj mozna badac response?
                    return true;
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
                return false;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
                return false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
                return false;
            }
        }
        private async Task<IEnumerable<Guid>> GetPhotoLinks(Guid event_id)
        {

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

                    var response = await client.GetPhotosAsync(new UUID { Value = event_id.ToString() });

                    return response.PhotoIds.Select(photoId => Guid.Parse(photoId.Value));
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
                return Enumerable.Empty<Guid>();
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
                return Enumerable.Empty<Guid>();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
                return Enumerable.Empty<Guid>();
            }
        }

        private async Task<bool> IsEventActive(Guid event_id)
        {

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

                    var response = await client.GetActiveEventsAsync(new Empty());

                    foreach (var photoEvent in response.Event)
                    {
                        if (Guid.TryParse(photoEvent.Id.Value, out var eventId) && eventId == event_id)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
                return false;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
                return false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
                return false;
            }
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
