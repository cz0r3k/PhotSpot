using System.IO;
namespace QRtest
{

    public partial class BrowsePage : ContentPage
    {
        public BrowsePage()
        {
            InitializeComponent();
            LoadImages();

        }

        private void LoadImages()
        {

            //var imageFiles = Directory.GetFiles(imagePath, "*.jpg");
            string[] imageFiles = { "http://192.168.0.206:10000/devstoreaccount1/swistaki/swistak3.jpg",
                                    "http://192.168.0.206:10000/devstoreaccount1/swistaki/swistak5.jpg",
                                    "http://192.168.0.206:10000/devstoreaccount1/swistaki/swistak6.jpg",
                                    "http://192.168.0.206:10000/devstoreaccount1/swistaki/swistak7.jpg"};

            try
            {
                foreach (var file in imageFiles)
                {
                    var image = new Image
                    {
                        Source = file,
                        Aspect = Aspect.AspectFill,
                        //HeightRequest = 200,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    ImageContainer.Children.Add(image);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");

            }
        }
    }
}