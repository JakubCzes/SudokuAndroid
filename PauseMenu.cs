using Android.Content;
using Android.OS;
using Android.Views;

namespace SudokuMobile;

[Activity(Label = "PauseMenu")]
public class PauseMenu : Activity
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

        SetContentView(Resource.Layout.pause_menu);

        Button buttonResume = FindViewById<Button>(Resource.Id.buttonResume);
        Button buttonExit = FindViewById<Button>(Resource.Id.buttonExit);

        buttonResume.Click += (sender, e) =>
        {
            int elapsedSeconds = Intent.GetIntExtra("elapsedSeconds", 0);

            var resultIntent = new Intent();
			resultIntent.PutExtra("elapsedSeconds", elapsedSeconds);
			SetResult(Result.Ok, resultIntent);
			Finish(); 
        };

        buttonExit.Click += (sender, e) =>
        {
            var intent = new Intent(this, typeof(MainActivity));
            FinishAffinity();
            StartActivity(intent);
        };
    }
}