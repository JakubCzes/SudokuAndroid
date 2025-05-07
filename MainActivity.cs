using Android.Content;

namespace SudokuMobile
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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