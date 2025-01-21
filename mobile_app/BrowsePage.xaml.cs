using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRtest
{

    public partial class BrowsePage : ContentPage
    {
        public List<string> PhotoLinks { get; set; } = new List<string>();
        private int _currentIndex = 0;
        public BrowsePage()
        {
            InitializeComponent();
            LoadInitialImage();
            this.BindingContext = this;

        }

        private async void LoadInitialImage()
        {
            try
            {
                foreach (var photoGuid in PhotoGuidManager.PhotoGuids)
                {
                    string link = $"http://{Globals.IP_ADDRESS}:8080/insecure/plain/http://azurite:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png/@webp";
                    PhotoLinks.Add(link);
                }

                if (PhotoLinks.Count > 0)
                {
                    await LoadImageAtIndex(_currentIndex);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
        }

        private async Task LoadImageAtIndex(int index)
        {
            if (index >= 0 && index < PhotoLinks.Count)
            {
                var image = new Image
                {
                    Source = PhotoLinks[index],
                    Aspect = Aspect.AspectFill,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                await Task.Delay(200);
            }
        }

        private async void OnPreviousClicked(object sender, EventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                await LoadImageAtIndex(_currentIndex);
            }
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            if (_currentIndex < PhotoLinks.Count - 1)
            {
                _currentIndex++;
                await LoadImageAtIndex(_currentIndex);
            }
        }

        private void OnCarouselViewPositionChanged(object sender, PositionChangedEventArgs e)
        {
        }
    }

    //    private async void LoadImages()
    //    {
    //        //string[] imageFiles = { $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/events/swistak3.jpg",
    //        //                        $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/swistaki/swistak5.jpg",
    //        //                        $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/swistaki/swistak6.jpg",
    //        //                        $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/swistaki/swistak7.jpg"};
    //        LoadingIndicator.IsRunning = true;
    //        LoadingIndicator.IsVisible = true;
    //        try
    //        {
    //            var counter = 0;
    //            foreach (var photoGuid in PhotoGuidManager.PhotoGuids)
    //            {
    //                string link = $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png";

    //                //string link = $"http://{Globals.IP_ADDRESS}:8080/insecure/plain/http://azurite:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png/@webp";

    //                await DisplayAlert("link", $"http://{Globals.IP_ADDRESS}:10000/devstoreaccount1/photos/{EventInfoManager.EventId}/{photoGuid}.png", "OK");

    //                var image = new Image
    //                {
    //                    Source = link,
    //                    Aspect = Aspect.AspectFill,
    //                    //HeightRequest = 200,
    //                    Margin = new Thickness(0, 0, 0, 10)
    //                };

    //                ImageContainer.Children.Add(image);

    //                await Task.Delay(500);

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");

    //        }
    //        finally
    //        {
    //            LoadingIndicator.IsRunning = false;
    //            LoadingIndicator.IsVisible = false;
    //        }
    //    }
    //}
}