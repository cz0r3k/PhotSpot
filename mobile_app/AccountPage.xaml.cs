namespace QRtest;

public partial class AccountPage : ContentPage
{
    public string Username { get; set; }
    public string Email { get; set; }

    public AccountPage()
    {
        InitializeComponent();

        Username = Preferences.Get("Nickname", "");
        Email = Preferences.Get("Email", "");

        BindingContext = this;
    }
}
