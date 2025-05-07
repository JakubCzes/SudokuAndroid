using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Telecom;
using Android.Widget;

namespace SudokuMobile
{
	[Activity(Label = "GameActivity")]
	public class GameActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.game_activity);

			GridLayout grid = FindViewById<GridLayout>(Resource.Id.sudokuGrid);
			EditText selectedCell = null;
			Button button1 = FindViewById<Button>(Resource.Id.button1);

			int size = 110; // wielkoœæ komórki w px (dostosuj do ekranu)
			int previousRow = 0;
			int previousCol = 0;

			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					EditText cell = new EditText(this)
					{
						Text = "1"
					};

					cell.SetWidth(size);
					cell.SetHeight(size);
					cell.Gravity = Android.Views.GravityFlags.Center;
					cell.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
					cell.Focusable = false;
					cell.FocusableInTouchMode = false;
					cell.Clickable = true;
					cell.LongClickable = false;
					cell.InputType = Android.Text.InputTypes.Null;

					// Stylizacja obramowañ
					cell.Background = CreateCellBackground(row, col, Color.White);

					GridLayout.LayoutParams p = new GridLayout.LayoutParams();
					p.RowSpec = GridLayout.InvokeSpec(row);
					p.ColumnSpec = GridLayout.InvokeSpec(col);
					cell.LayoutParameters = p;

					int currentRow = row;
					int currentCol = col;
					
					cell.Click += (s, e) =>
					{
						if (selectedCell != null)
							// Initialize previousRow and previousCol to default values to avoid the CS0165 error.
							
							selectedCell.Background = CreateCellBackground(previousRow, previousCol, Color.White);
						previousRow = currentRow;
						previousCol = currentCol;
						selectedCell = (EditText)s;
						selectedCell.Background = CreateCellBackground(currentRow, currentCol, Color.LightBlue);
					};

					grid.AddView(cell);
				}
			}

			button1.Click += (s, e) =>
			{
				if (selectedCell != null)
					selectedCell.Text = "3";
			};
		}

		private Drawable CreateCellBackground(int row, int col, Color backgroundColor)
		{
			int strokeThin = 2;
			int strokeThick = 6;

			int left = (col % 3 == 0) ? strokeThick : strokeThin;
			int top = (row % 3 == 0) ? strokeThick : strokeThin;
			int right = (col == 8) ? strokeThick : strokeThin;
			int bottom = (row == 8) ? strokeThick : strokeThin;

			var shape = new GradientDrawable();
			shape.SetColor(backgroundColor);

			// LayerDrawable + insets = sztuczna gruboœæ krawêdzi
			var layer = new LayerDrawable(new Drawable[] { shape });
			layer.SetLayerInset(0, left, top, right, bottom);

			return layer;
		}
	}
}