using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace SudokuMobile
{
    [Activity(Label = "@string/app_name", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
			{
				Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
			}
			Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
			SystemUiFlags.ImmersiveSticky
			| SystemUiFlags.LayoutStable
			| SystemUiFlags.LayoutHideNavigation
			| SystemUiFlags.LayoutFullscreen
			| SystemUiFlags.HideNavigation
			| SystemUiFlags.Fullscreen
			);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

            Button buttonPlay = FindViewById<Button>(Resource.Id.buttonPlay); // Your button ID

            buttonPlay.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(PlayActivity));
                StartActivity(intent);
            };
        }
    }
}