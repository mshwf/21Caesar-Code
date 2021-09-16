using StegaXam.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class EncodePage : ContentPage
    {
        readonly SteganographyAlgorithm steganographyAlgorithm;
        public EncodePage()
        {
            InitializeComponent();
            steganographyAlgorithm = SteganographyAlgorithm.GetLastVersion();
        }

        public EncodePage(byte[] imageData) : this()
        {
            picker.LoadImage(imageData);
        }

        static string filename;
        private async void HideClick(object sender, EventArgs e)
        {
            try
            {
                string textToHide = entryMsg.Text;
                if (string.IsNullOrEmpty(textToHide))
                {
                    await DisplayAlert("What's your message?", "Type the message you wish to hide from enemies", "OK");
                    return;
                }
                if (picker.ImageData == null)
                {
                    await DisplayAlert("No image selected!", "Pick up an image to hide your message in.", "OK");
                    return;
                }

                string password = await DisplayPromptAsync("Password?", "Enter a password, or leave empty to embed the message in plain (not encrypted)");
                if (password == null) return;
                loader.IsRunning = true;
                var imageData = await Task.Run(() => steganographyAlgorithm.Encode(picker.ImageData, textToHide, password));
                loader.IsRunning = false;

                filename = DependencyService.Get<IPicture>().SavePictureToDisk("21Caesar", imageData);
                if (string.IsNullOrEmpty(filename))
                    await DisplayAlert("Oops!", "Image couldn't be saved.", "OK");
                else
                {
                    var pwdMesg = password != "" ? "they can't read the message without knowing the password, " : null;
                    await DisplayAlert("Your secret has been embeded successfully",
                        $"You can now share it with anybody, {pwdMesg}the image name: {filename}", "OK");
                    btnHide.IsVisible = false;
                    btnShare.IsVisible = true;
                }
            }
            catch (TooSmallImageException)
            {
                await DisplayAlert("Oops!", "The image is too small for the message, choose bigger one.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
        }

        private async void Share_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filename))
                await DisplayAlert("Oops!", "File not found, please try again.", "OK");
            Application.Current.Properties.TryGetValue(AppConstants.ShowShareNote, out object showShareNoteObj);
            if ((bool?)showShareNoteObj != false)
            {
                var dontRemind = await DisplayAlert("Important!", "To ensure the integrity of data, share via safe mediums, " +
                    "that don't manipulate images by reducing their quality. The safest option is using mail services, " +
                    "while using services like Facebook or WhatsApp will corrupt the encoded data.", "Don't remind me again", "OK");
                if (dontRemind)
                    Application.Current.Properties[AppConstants.ShowShareNote] = false;
            }
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Hello",
                File = new ShareFile(filename)
            });
        }

        private void picker_ImageDataChanged(object sender, EventArgs e)
        {
            btnShare.IsVisible = false;
            btnHide.IsVisible = true;
        }
    }
}