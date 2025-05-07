namespace SudokuMobile;

[Activity(Label = "PlayActivity")]
public class PlayActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.play_activity);
    }
}