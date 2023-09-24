using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Game2048 {
	public class Game {
		// Fields
		private readonly Field field = new Field();
		private ulong score = 0;
		private ulong bestScore = 0;
		private bool appendValueFlag = false;
		private bool winFlag = false;
		private Storage storage = default;
		private bool gameLoop = false;

		// Constants
		public const string storageFileName = "storage.json";

		// Methods
		public void StartGameLoop() {
			LoadStorage();
			field.CellUnionEvent += OnFieldCellUnited;
			field.CellMoveEvent += OnFieldCellMoved;
			StartNewGame();
			//field.TestWin();
			while( gameLoop ) {
				appendValueFlag = false;
				Draw();
				if( UpdateState() ) {
					continue;
				}
				Console.WriteLine( "Press 'Q' to quit game. 'R' to restart" );
				if( UpdateInput() ) {
					continue;
				}
				if( appendValueFlag ) {
					field.PlaceNewValue();
				}
			}
			field.CellUnionEvent -= OnFieldCellUnited;
			field.CellMoveEvent -= OnFieldCellMoved;
			SaveStorage();
		}

		private void Draw() {
			Console.Clear();
			Console.SetCursorPosition( 0, 0 );
			Console.WriteLine( $"Score='{score}'. Best score='{bestScore}'" );
			field.DrawField();
		}

		private bool UpdateInput() {
			var keyInfo = Console.ReadKey( true );
			if( keyInfo.Key == ConsoleKey.LeftArrow ) {
				field.TryUniteLeft();
			}
			if( keyInfo.Key == ConsoleKey.RightArrow ) {
				field.TryUniteRight();
			}
			if( keyInfo.Key == ConsoleKey.DownArrow ) {
				field.TryUniteDown();
			}
			if( keyInfo.Key == ConsoleKey.UpArrow ) {
				field.TryUniteUp();
			}
			if( keyInfo.Key == ConsoleKey.Q ) {
				if( ShowYesNoDialog( "Do you want to exit the game?" ) ) {
					ExitGame();
					return true;
				}
			}
			if( keyInfo.Key == ConsoleKey.R ) {
				if( ShowYesNoDialog( "Do you want to restart the game?" ) ) {
					StartNewGame();
					return true;
				}
			}
			return false;
		}

		private bool UpdateState() {
			if( !field.CheckValidMove() ) {
				if( ShowYesNoDialog( "You Lose! Do you want to play again?" ) ) {
					StartNewGame();
				}
				else {
					ExitGame();
				}
				return true;
			}
			if( winFlag ) {
				if( ShowYesNoDialog( "You Win! Do you want to play again?" ) ) {
					StartNewGame();
				}
				else {
					ExitGame();
				}
				return true;
			}
			return false;
		}

		private bool ShowYesNoDialog( string text ) {
			Console.WriteLine( $"{text} (Y/N)" );
			while( true ) {
				var keyInfo = Console.ReadKey( true );
				if( keyInfo.Key == ConsoleKey.Y ) {
					return true;
				}
				if( keyInfo.Key == ConsoleKey.N ) {
					return false;
				}
			}
		}

		private void StartNewGame() {
			field.Clear();
			field.PlaceNewValue();
			field.PlaceNewValue();
			score = 0;
			winFlag = false;
			gameLoop = true;
		}

		private void ExitGame() {
			gameLoop = false;
		}

		private void OnFieldCellMoved() {
			appendValueFlag = true;
		}

		private void OnFieldCellUnited( ushort cellValue ) {
			appendValueFlag = true;
			score += cellValue;
			if( score > bestScore ) {
				bestScore = score;
			}
			if( cellValue >= 2048 ) {
				winFlag = true;
			}
		}

		public bool LoadStorage() {
			string filePath = Path.Combine( Path.GetDirectoryName( Assembly.GetEntryAssembly().Location ), storageFileName );
			if( !File.Exists( filePath ) ) {
				return false;
			}
			var text = File.ReadAllText( filePath );
			if( string.IsNullOrEmpty( text ) ) {
				return false;
			}
			storage = JsonSerializer.Deserialize<Storage>( text );
			if( storage != null ) {
				bestScore = storage.bestScore;
			}
			return true;
		}

		public bool SaveStorage() {
			string filePath = Path.Combine( Path.GetDirectoryName( Assembly.GetEntryAssembly().Location ), storageFileName );
			if( storage == null ) {
				storage = new Storage();
			}
			storage.bestScore = bestScore;
			var text = JsonSerializer.Serialize( storage );
			File.WriteAllText( filePath, text );
			return true;
		}
	}
}