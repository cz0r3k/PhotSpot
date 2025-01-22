using System.Collections.ObjectModel;

namespace QRtest
{
    public partial class BrowsePhotosPage : ContentPage
    {
        public ObservableCollection<string> Images { get; private set; } = new ObservableCollection<string>();
        public string EventName { get; set; }
        public BrowsePhotosPage()
        {
            InitializeComponent();

            EventName = EventInfoManager.EventName;
            BindingContext = this;
            LoadImages();
        }

        private async void LoadImages()
        {
            try
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;



                foreach (var photoGuid in PhotoGuidManager.PhotoGuids)
                {
                    string link = $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png";

                    Dispatcher.Dispatch(() =>
                    {
                        Images.Add(link);
                    });

                    await Task.Delay(200);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
    }
}
