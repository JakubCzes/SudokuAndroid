using Android.Content;

namespace SudokuMobile;

[Activity(Label = "PlayActivity")]
public class PlayActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.play_activity);

		Button buttonLatwy = FindViewById<Button>(Resource.Id.buttonLatwy);
		Button buttonSredni = FindViewById<Button>(Resource.Id.buttonSredni);
		Button buttonTrudny = FindViewById<Button>(Resource.Id.buttonTrudny);
		Button buttonWroc = FindViewById<Button>(Resource.Id.buttonWroc);

		buttonLatwy.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra("poziom", "latwy");
			StartActivity(intent);
		};

		buttonSredni.Click += (sender, e) =>
		{
			var intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra("poziom", "sredni");
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