using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Telecom;
using Android.Widget;
using System.Reflection.Emit;

namespace SudokuMobile
{
	[Activity(Label = "GameActivity")]
	public class GameActivity : Activity
    {
		int[,] solvedBoard = new int[9, 9];
		protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.game_activity);

			string poziom = Intent.GetStringExtra("poziom") ?? "latwy";

			TextView trudnosc = FindViewById<TextView>(Resource.Id.textTrudnosc);
			trudnosc.Text = $"Trudnoœæ: \n{poziom}";
			GridLayout grid = FindViewById<GridLayout>(Resource.Id.sudokuGrid);
			Button buttonSprawdz = FindViewById<Button>(Resource.Id.buttonSprawdz);

			switch (poziom)
			{
				case "latwy":
					GenerateSudoku(2, grid);
					break;
				case "sredni":
					GenerateSudoku(30, grid);
					break;
				case "trudny":
					GenerateSudoku(40, grid);
					break;
				default:
					GenerateSudoku(20, grid);
					break;
			}

			buttonSprawdz.Click += (s, e) =>
			{
				int errors = 0;
				for (int i = 0; i < 81; i++)
				{
					var view = grid.GetChildAt(i);
					if (view is EditText cell && cell.Clickable)  // tylko edytowalne komórki
					{
						int row = i / 9;
						int col = i % 9;
						string text = cell.Text.Trim();

						if (int.TryParse(text, out int userValue))
						{
							if (userValue == solvedBoard[row, col])
							{
								cell.Background = CreateCellBackground(row, col, Color.LightGreen);
								cell.Clickable = false; // Zablokuj edytowalnoœæ po poprawnym wpisaniu
								cell.Tag = Color.LightGreen.ToArgb();
							}
							else
							{
								cell.Background = CreateCellBackground(row, col, Color.IndianRed);
								cell.Tag = Color.IndianRed.ToArgb();
								errors++;
							}
						}
						else
						{
							// Pusta komórka lub niepoprawna wartoœæ
							cell.Background = CreateCellBackground(row, col, Color.IndianRed);
							cell.Tag = Color.IndianRed.ToArgb();
							errors++;
						}
					}
				}

				if (errors == 0)
				{
					Toast.MakeText(this, "Gratulacje! Wszystkie liczby s¹ poprawne!", ToastLength.Long).Show();
				}
				else
				{
					Toast.MakeText(this, $"B³¹d: {errors} niepoprawnych komórek", ToastLength.Long).Show();
				}
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
		private void GenerateSudoku(int hide, GridLayout grid)
		{
			int[,] board = new int[9, 9];
			Random rand = new Random();

			// Generowanie pe³nej planszy
			if (GenerateCompleteSudoku(board, rand))
			{
				Array.Copy(board, solvedBoard, board.Length);
				// Ukrywanie cyfr (mo¿esz zmieniæ liczbê)
				HideDigits(board, hide, rand);
				FillGridFromBoard(board, hide, grid);
			}
		}
		private bool GenerateCompleteSudoku(int[,] board, Random rand)
		{
			return SolveSudoku(board, rand);  // U¿ywamy rozwi¹zywania Sudoku, by wygenerowaæ pe³ne rozwi¹zanie
		}

		// Funkcja rozwi¹zuj¹ca Sudoku za pomoc¹ rekurencji
		private bool SolveSudoku(int[,] board, Random rand)
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (board[row, col] == 0) // Szukamy pustej komórki
					{
						List<int> possibleNumbers = Enumerable.Range(1, 9).ToList();
						ShuffleList(possibleNumbers, rand);  // Tasujemy liczby, aby by³y losowe

						foreach (int num in possibleNumbers)
						{
							if (IsValid(board, row, col, num))  // Jeœli liczba jest poprawna
							{
								board[row, col] = num;
								if (SolveSudoku(board, rand))  // Rekurencyjnie rozwi¹zujemy
								{
									return true;
								}
								board[row, col] = 0; // Jeœli nie uda³o siê rozwi¹zaæ, cofnij
							}
						}
						return false; // Jeœli nie ma mo¿liwego rozwi¹zania
					}
				}
			}
			return true; // Zakoñczone, jeœli wszystkie komórki zosta³y wype³nione
		}

		// Funkcja sprawdzaj¹ca, czy liczba jest dozwolona w danej komórce
		private bool IsValid(int[,] board, int row, int col, int num)
		{
			// Sprawdzamy, czy liczba nie wystêpuje w tym wierszu
			for (int i = 0; i < 9; i++)
			{
				if (board[row, i] == num || board[i, col] == num)
					return false;
			}

			// Sprawdzamy, czy liczba nie wystêpuje w tej siatce 3x3
			int startRow = row - row % 3;
			int startCol = col - col % 3;
			for (int i = startRow; i < startRow + 3; i++)
			{
				for (int j = startCol; j < startCol + 3; j++)
				{
					if (board[i, j] == num)
						return false;
				}
			}

			return true;
		}

		// Funkcja do losowego tasowania listy
		private void ShuffleList(List<int> list, Random rand)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rand.Next(n + 1);
				int value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		// Funkcja do ukrywania 40 cyfr na planszy
		private void HideDigits(int[,] board, int digitsToHide, Random rand)
		{
			List<int> positions = new List<int>();

			// Losowe usuwanie cyfr z planszy
			while (positions.Count < digitsToHide)
			{
				int row = rand.Next(9);
				int col = rand.Next(9);
				int pos = row * 9 + col;

				if (!positions.Contains(pos) && board[row, col] != 0)
				{
					positions.Add(pos);
					board[row, col] = 0;  // Usuwamy cyfrê
				}
			}
		}

		private void FillGridFromBoard(int[,] board, int count, GridLayout grid)
		{
			int size = 112; // wielkoœæ komórki w px (dostosuj do ekranu)
			int previousRow = 0;
			int previousCol = 0;
			int childIndex = 0;
			EditText selectedCell = null;
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					EditText cell = new EditText(this);

					cell.SetWidth(size);
					cell.SetHeight(size);
					cell.Gravity = Android.Views.GravityFlags.Center;
					cell.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
					cell.Focusable = false;
					cell.FocusableInTouchMode = false;
					cell.Clickable = true;
					cell.LongClickable = false;
					cell.InputType = Android.Text.InputTypes.Null;

					// Stylizacja obramowañ
					Color baseColor = (board[row, col] != 0) ? Color.White : Color.LightGray;
					cell.Background = CreateCellBackground(row, col, baseColor);
					cell.Tag = baseColor.ToArgb();

					GridLayout.LayoutParams p = new GridLayout.LayoutParams();
					p.RowSpec = GridLayout.InvokeSpec(row);
					p.ColumnSpec = GridLayout.InvokeSpec(col);
					cell.LayoutParameters = p;

					int currentRow = row;
					int currentCol = col;

					cell.Click += (s, e) =>
					{
						if (selectedCell != null && selectedCell.Tag is Java.Lang.Integer javaColor)
						{
							var color = new Color(javaColor.IntValue());
							selectedCell.Background = CreateCellBackground(previousRow, previousCol, color);
						}
						previousRow = currentRow;
						previousCol = currentCol;
						selectedCell = (EditText)s;
						selectedCell.Background = CreateCellBackground(currentRow, currentCol, Color.LightBlue);
					};

					grid.AddView(cell);
					if (board[row, col] != 0)
					{
						cell.Text = board[row, col].ToString();
						cell.Clickable = false;
					}
					else
					{
						cell.Text = "";
						cell.Background = CreateCellBackground(row, col, Color.LightGray);
					}

					childIndex++;
				}
			}
			Button button1 = FindViewById<Button>(Resource.Id.button1);
			Button button2 = FindViewById<Button>(Resource.Id.button2);
			Button button3 = FindViewById<Button>(Resource.Id.button3);
			Button button4 = FindViewById<Button>(Resource.Id.button4);
			Button button5 = FindViewById<Button>(Resource.Id.button5);
			Button button6 = FindViewById<Button>(Resource.Id.button6);
			Button button7 = FindViewById<Button>(Resource.Id.button7);
			Button button8 = FindViewById<Button>(Resource.Id.button8);
			Button button9 = FindViewById<Button>(Resource.Id.button9);

			foreach (var button in new[] { button1, button2, button3, button4, button5, button6, button7, button8, button9 })
			{
				button.Click += (s, e) =>
				{
					if (selectedCell != null)
					{
						selectedCell.Text = ((Button)s).Text;
					}
				};
			}
		}
	}
}