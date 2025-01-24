using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcPhoto;
using util.Photo;

namespace QRtest
{
    public partial class BrowsePhotosPage : ContentPage
    {
        public ObservableCollection<ImageInfo> Images { get; private set; } = new();
        public string EventName { get; set; }

        public BrowsePhotosPage()
        {
            InitializeComponent();

            EventName = EventInfoManager.EventName;
            BindingContext = this;
            refreshView.IsRefreshing = true;
        }

        private async void ToggleLike(object sender, EventArgs e)
        {
            var imageInfo = ImageCarousel.CurrentItem as ImageInfo;
            if (imageInfo.IsLiked)
            {
                await Like();
            }
            else
            {
                await Unlike();
            }
        }

        private async Task Like()
        {
            var imageInfo = ImageCarousel.CurrentItem as ImageInfo;
            Images[ImageCarousel.Position].IsLiked = true;
            Images[ImageCarousel.Position].Likes += 1;
            var socketHttpHandler = new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                },
                EnableMultipleHttp2Connections = true
            };

            using var channel = GrpcChannel.ForAddress($"https://{Globals.IP_ADDRESS}:7244", new GrpcChannelOptions
            {
                HttpHandler = socketHttpHandler
            });
            var client = new Photo.PhotoClient(channel);
            var result = await client.LikePhotoInsecureAsync(new LikePhotoInsecureRequest
            {
                Email = "czorek@protonmail.com", PhotoId = new UUID { Value = imageInfo.Id.ToString() },
                EventId = new GrpcEvent.UUID { Value = imageInfo.Id.ToString() }
            });
        }

        private async Task Unlike()
        { 
            var imageInfo = ImageCarousel.CurrentItem as ImageInfo;
            Images[ImageCarousel.Position].IsLiked = false;
            Images[ImageCarousel.Position].Likes -= 1;
            
            var socketHttpHandler = new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                },
                EnableMultipleHttp2Connections = true
            };

            using var channel = GrpcChannel.ForAddress($"https://{Globals.IP_ADDRESS}:7244", new GrpcChannelOptions
            {
                HttpHandler = socketHttpHandler
            });
            var client = new Photo.PhotoClient(channel);
            var result = await client.UnLikePhotoInsecureAsync(new LikePhotoInsecureRequest
            {
                Email = "czorek@protonmail.com", PhotoId = new UUID { Value = imageInfo.Id.ToString() },
                EventId = new GrpcEvent.UUID { Value = imageInfo.Id.ToString() }
            });
        }

        private async void refreshView_Refreshing(object sender, EventArgs e)
        {
            foreach (var photoGuid in PhotoGuidManager.PhotoGuids)
            {
                var link =
                    $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png";
                var info = await GetLikesInfoAsync(photoGuid);
                Images.Add(new ImageInfo
                    { Id = photoGuid, Link = link, IsLiked = info.IsUserLike, Likes = info.Likes });
            }


            refreshView.IsRefreshing = false;
        }

        private async Task<PhotoDetails?> GetLikesInfoAsync(Guid photoId)
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

                using var channel = GrpcChannel.ForAddress($"https://{Globals.IP_ADDRESS}:7244", new GrpcChannelOptions
                {
                    HttpHandler = socketHttpHandler
                });
                var client = new Photo.PhotoClient(channel);
                var reply = await client.GetPhotoDetailsInsecureAsync(new PhotoDetailsInsecureRequest
                {
                    EventId = new GrpcEvent.UUID { Value = EventInfoManager.EventId.ToString() },
                    PhotoId = new GrpcPhoto.UUID { Value = photoId.ToString() }, Email = "czorek@protonmail.com"
                });
                return new PhotoDetails
                {
                    Likes = (int)reply.Likes,
                    IsUserLike = reply.IsUserLike,
                };
            }
            catch (RpcException ex)
            {
                await DisplayAlert("Error", "Server is unavailable: " + ex.Message, "OK");
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error", "Network error: " + ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
            }

            return null;
        }
    }
}

public class ImageInfo
{
    public Guid Id { get; set; }
    public string Link { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
}