using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Telecom;
using Android.Views;

namespace SudokuMobile
{
	[Activity(Label = "GameActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class GameActivity : Activity
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

			SetContentView(Resource.Layout.game_activity);
		}
    }
}