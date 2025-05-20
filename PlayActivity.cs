using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace SudokuMobile;

[Activity(Label = "PlayActivity", ScreenOrientation = ScreenOrientation.Portrait)]
public class PlayActivity : Activity
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
		SetContentView(Resource.Layout.play_activity);

		Button buttonLatwy = FindViewById<Button>(Resource.Id.buttonLatwy);
		Button buttonSredni = FindViewById<Button>(Resource.Id.buttonSredni);
		Button buttonTrudny = FindViewById<Button>(Resource.Id.buttonTrudny);
		Button buttonWroc = FindViewById<Button>(Resource.Id.buttonWroc);

		buttonLatwy.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra("poziom", "³atwy");
			StartActivity(intent);
		};

		buttonSredni.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra("poziom", "œredni");
			StartActivity(intent);
		};

		buttonTrudny.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra("poziom", "trudny");
			StartActivity(intent);
		};

		buttonWroc.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
		};
	}
}