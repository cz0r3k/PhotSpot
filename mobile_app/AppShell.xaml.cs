namespace QRtest
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("TakePhotoPage", typeof(TakePhotoPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            //Routing.RegisterRoute("BrowsePage", typeof(BrowsePage));
            Routing.RegisterRoute("BrowsePhotosPage", typeof(BrowsePhotosPage));
            //CurrentItem.TabBarBackgroundColor = Color.FromArgb("#32a6a8");
            //CurrentItem.TabBarForegroundColor = Colors.White;
        }
    }
}
