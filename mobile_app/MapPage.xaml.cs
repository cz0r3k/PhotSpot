using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcEvent;
using Microsoft.Maui.Controls.Maps;
using util.PhotoEvent;
namespace QRtest;

public partial class MapPage : ContentPage
{
    private List<PhotoEventLocation> _eventLocations = new List<PhotoEventLocation>();
    public MapPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var resp = await GetLocations();
        if (resp != null) // czy jesli null to pass?
        {
            _eventLocations = resp.Event.Select(PhotoEventLocation.FromEventLocation).ToList();
            AddPinsToMap(_eventLocations);
        }
    }

    private async Task<LocationReply> GetLocations()
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
                var response = await client.GetEventLocalizationsAsync(new Empty());
                return response;
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
        return null; //czy to ok czy new LocationReply()?
    }

    private void AddPinsToMap(List<PhotoEventLocation> locations)
    {
        foreach (var location in locations)
        {
            var pin = new Pin
            {
                Label = location.Name,
                Type = PinType.Place,
                Location = new Location((double)location.Latitude, (double) location.Longitude),
            };
            map.Pins.Add(pin);
        }
    }
}