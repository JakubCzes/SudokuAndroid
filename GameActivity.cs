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
        System.Timers.Timer timer;
        DateTime startTime;
        TextView textCzas;
        int[,] solvedBoard = new int[9, 9];
		int minutes, seconds, checkCount;
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

			string poziom = Intent.GetStringExtra("poziom") ?? "latwy";

			TextView trudnosc = FindViewById<TextView>(Resource.Id.textTrudnosc);
            textCzas = FindViewById<TextView>(Resource.Id.textCzas);
            trudnosc.Text = $"Trudność: \n{poziom}";
			GridLayout grid = FindViewById<GridLayout>(Resource.Id.sudokuGrid);
			Button buttonSprawdz = FindViewById<Button>(Resource.Id.buttonSprawdz);
			Button buttonPauza = FindViewById<Button>(Resource.Id.buttonPauza);

			switch (poziom)
			{
				case "latwy":
					GenerateSudoku(2, grid);
					checkCount = 5;
					break;
				case "sredni":
					GenerateSudoku(30, grid);
					checkCount = 3;
					break;
				case "trudny":
					GenerateSudoku(40, grid);
					checkCount = 1;
					break;
				default:
					GenerateSudoku(20, grid);
					break;
			}

            int elapsedSeconds = Intent.GetIntExtra("elapsedSeconds", 0);
            startTime = DateTime.Now - TimeSpan.FromSeconds(elapsedSeconds);

            TimeSpan elapsed = DateTime.Now - startTime;
            minutes = (int)elapsed.TotalMinutes;
            seconds = elapsed.Seconds;
            textCzas.Text = $"Czas: \n{minutes:D2}:{seconds:D2}";

            StartTimer();

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
								cell.Clickable = false; // Zablokuj edytowalność po poprawnym wpisaniu
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
							// Pusta komórka lub niepoprawna wartość
							cell.Background = CreateCellBackground(row, col, Color.IndianRed);
							cell.Tag = Color.IndianRed.ToArgb();
							errors++;
						}
					}
				}

				if (errors == 0)
				{
					Toast.MakeText(this, $"Gratulacje! Wszystkie liczby są poprawne! Czas: {minutes:D2}:{seconds:D2}", ToastLength.Long).Show();
					Finish();
				}
				else
				{
					if (checkCount > 0)
					{
						checkCount--;
						Toast.MakeText(this, $"{errors} niepoprawnych komórek. \nPozostało prób: {checkCount}", ToastLength.Long).Show();
					}
					else
					{
						Toast.MakeText(this, $"Przegrałeś! {errors} niepoprawnych komórek", ToastLength.Long).Show();
						Finish();
					}
				}

				
			};

			buttonPauza.Click += (sender, e) =>
			{
                timer.Stop();
                TimeSpan elapsed = DateTime.Now - startTime;
                var intent = new Intent(this, typeof(PauseMenu));
                intent.PutExtra("elapsedSeconds", (int)elapsed.TotalSeconds);
                StartActivity(intent);
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

			// LayerDrawable + insets = sztuczna grubość krawędzi
			var layer = new LayerDrawable(new Drawable[] { shape });
			layer.SetLayerInset(0, left, top, right, bottom);

			return layer;
		}
		private void GenerateSudoku(int hide, GridLayout grid)
		{
			int[,] board = new int[9, 9];
			Random rand = new Random();

			// Generowanie pełnej planszy
			if (GenerateCompleteSudoku(board, rand))
			{
				Array.Copy(board, solvedBoard, board.Length);
				// Ukrywanie cyfr (możesz zmienić liczbę)
				HideDigits(board, hide, rand);
				FillGridFromBoard(board, hide, grid);
			}
		}
		private bool GenerateCompleteSudoku(int[,] board, Random rand)
		{
			return SolveSudoku(board, rand);  // Używamy rozwiązywania Sudoku, by wygenerować pełne rozwiązanie
		}

		// Funkcja rozwiązująca Sudoku za pomocą rekurencji
		private bool SolveSudoku(int[,] board, Random rand)
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (board[row, col] == 0) // Szukamy pustej komórki
					{
						List<int> possibleNumbers = Enumerable.Range(1, 9).ToList();
						ShuffleList(possibleNumbers, rand);  // Tasujemy liczby, aby były losowe

						foreach (int num in possibleNumbers)
						{
							if (IsValid(board, row, col, num))  // Jeśli liczba jest poprawna
							{
								board[row, col] = num;
								if (SolveSudoku(board, rand))  // Rekurencyjnie rozwiązujemy
								{
									return true;
								}
								board[row, col] = 0; // Jeśli nie udało się rozwiązać, cofnij
							}
						}
						return false; // Jeśli nie ma możliwego rozwiązania
					}
				}
			}
			return true; // Zakończone, jeśli wszystkie komórki zostały wypełnione
		}

		// Funkcja sprawdzająca, czy liczba jest dozwolona w danej komórce
		private bool IsValid(int[,] board, int row, int col, int num)
		{
			// Sprawdzamy, czy liczba nie występuje w tym wierszu
			for (int i = 0; i < 9; i++)
			{
				if (board[row, i] == num || board[i, col] == num)
					return false;
			}

			// Sprawdzamy, czy liczba nie występuje w tej siatce 3x3
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
					board[row, col] = 0;  // Usuwamy cyfrę
				}
			}
		}

		private void FillGridFromBoard(int[,] board, int count, GridLayout grid)
		{
			int size = 112; // wielkość komórki w px (dostosuj do ekranu)
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

					// Stylizacja obramowań
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
						selectedCell.Tag = Color.LightGray.ToArgb();
					}
				};
			}
		}
        private void StartTimer()
        {
            timer = new System.Timers.Timer(1000); // 1 sekunda
            timer.Elapsed += (s, e) =>
            {
                RunOnUiThread(() =>
                {
                    TimeSpan elapsed = DateTime.Now - startTime;
                    minutes = (int)elapsed.TotalMinutes;
                    seconds = elapsed.Seconds;
					textCzas.Text = $"Czas: \n{minutes:D2}:{seconds:D2}";
                });
            };
            timer.Start();
        }
    }
}