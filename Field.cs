using System;

namespace Game2048 {
	public class Field {
		// Types
		public delegate void CellUnionHandler( ushort cellValue );

		// Fields
		private readonly Cell[,] cells = new Cell[rowCount, colCount];
		private CellUnionHandler cellUnionEvent = default;
		private Action cellMoveEvent = default;

		// Constants
		private const int rowCount = 4;
		private const int colCount = 4;
		private const string rowSeperator = "|----|----|----|----|";

		// Properties
		public event CellUnionHandler CellUnionEvent {
			add {
				cellUnionEvent -= value;
				cellUnionEvent += value;
			}
			remove {
				cellUnionEvent -= value;
			}
		}

		public event Action CellMoveEvent {
			add {
				cellMoveEvent -= value;
				cellMoveEvent += value;
			}
			remove {
				cellMoveEvent -= value;
			}
		}

		public Field() {
			for( int row = 0; row < rowCount; ++row ) {
				for( int col = 0; col < colCount; ++col ) {
					cells[row, col] = new Cell();
				}
			}
		}

		// Methods
		public void DrawField() {
			for( int row = 0; row < rowCount; ++row ) {
				Console.WriteLine( rowSeperator );
				for( int col = 0; col < colCount; ++col ) {
					Console.Write( "|" );
					cells[row, col].DrawCell();
				}
				Console.WriteLine( "|" );
			}
			Console.WriteLine( rowSeperator );
		}

		public void Clear() {
			for( int row = 0; row < rowCount; ++row ) {
				for( int col = 0; col < colCount; ++col ) {
					cells[row, col].Clear();
				}
			}
		}

		public void PlaceNewValue() {
			int randRow = Utils.RandomRange( 0, rowCount );
			int randCol = Utils.RandomRange( 0, colCount );
			for( int row = 0; row < rowCount; ++row ) {
				var newRow = ( row + randRow ) % rowCount;
				for( int col = 0; col < colCount; ++col ) {
					var newCol = ( col + randCol ) % colCount;
					var cell = cells[newRow, newCol];
					if( cell.IsEmpty ) {
						var rand = Utils.RandomRange( 0, 100 );
						cell.SetValue( (ushort) ( rand < 20 ? 4 : 2 ) );
						cell.NewFlag = true;
						return;
					}
				}
			}
		}

		public void TryUniteLeft() {
			for( int row = 0; row < rowCount; ++row ) {
				MoveRowLeft( row );
				var prevCellValue = cells[row, 0].Value;
				for( int col = 1; col < colCount; ++col ) {
					var cell = cells[row, col];
					if( prevCellValue == cell.Value && prevCellValue > 0 ) {
						var unionCell = cells[row, col - 1];
						unionCell.Unite( cell );
						cell.Clear();
						++col;
						cellUnionEvent?.Invoke( unionCell.Value );
					}
					if( col < colCount ) {
						prevCellValue = cells[row, col].Value;
					}
				}
				MoveRowLeft( row );
			}
		}

		public void TryUniteRight() {
			for( int row = 0; row < rowCount; ++row ) {
				MoveRowRight( row );
				var prevCellValue = cells[row, colCount - 1].Value;
				for( int col = colCount - 2; col >= 0; --col ) {
					var cell = cells[row, col];
					if( prevCellValue == cell.Value && prevCellValue > 0 ) {
						var unionCell = cells[row, col + 1];
						unionCell.Unite( cell );
						--col;
						cellUnionEvent?.Invoke( unionCell.Value );
					}
					if( col >= 0 ) {
						prevCellValue = cells[row, col].Value;
					}
				}
				MoveRowRight( row );
			}
		}

		public void TryUniteDown() {
			for( int col = 0; col < colCount; ++col ) {
				MoveColumnDown( col );
				var prevCellValue = cells[rowCount - 1, col].Value;
				for( int row = rowCount - 2; row >= 0; --row ) {
					var cell = cells[row, col];
					if( prevCellValue == cell.Value && prevCellValue > 0 ) {
						var unionCell = cells[row + 1, col];
						unionCell.Unite( cell );
						--row;
						cellUnionEvent?.Invoke( unionCell.Value );
					}
					if( row >= 0 ) {
						prevCellValue = cells[row, col].Value;
					}
				}
				MoveColumnDown( col );
			}
		}

		public void TryUniteUp() {
			for( int col = 0; col < colCount; ++col ) {
				MoveColumnUp( col );
				var prevCellValue = cells[0, col].Value;
				for( int row = 1; row < rowCount; ++row ) {
					var cell = cells[row, col];
					if( prevCellValue == cell.Value && prevCellValue > 0 ) {
						var unionCell = cells[row - 1, col];
						unionCell.Unite( cell );
						++row;
						cellUnionEvent?.Invoke( unionCell.Value );
					}
					if( row < rowCount ) {
						prevCellValue = cell.Value;
					}
				}
				MoveColumnUp( col );
			}
		}

		private void MoveRowLeft( int row ) {
			int leftIndex = int.MinValue;
			for( int col = 0; col < colCount; ++col ) {
				var cell = cells[row, col];
				if( cell.IsEmpty && leftIndex < 0 ) {
					leftIndex = col;
				}
				if( leftIndex >= 0 && leftIndex < colCount && !cell.IsEmpty ) {
					var dstCell = cells[row, leftIndex];
					dstCell.Copy( cell );
					cell.Clear();
					++leftIndex;
					cellMoveEvent?.Invoke();
				}
			}
		}

		private void MoveRowRight( int row ) {
			int rightIndex = int.MinValue;
			for( int col = colCount - 1; col >= 0; --col ) {
				var cell = cells[row, col];
				if( cell.IsEmpty && rightIndex < 0 ) {
					rightIndex = col;
				}
				if( rightIndex >= 0 && rightIndex > 0 && !cell.IsEmpty ) {
					var dstCell = cells[row, rightIndex];
					dstCell.Copy( cell );
					cell.Clear();
					--rightIndex;
					cellMoveEvent?.Invoke();
				}
			}
		}

		private void MoveColumnDown( int col ) {
			int bottomIndex = int.MinValue;
			for( int row = rowCount - 1; row >= 0; --row ) {
				var cell = cells[row, col];
				if( cell.IsEmpty && bottomIndex < 0 ) {
					bottomIndex = row;
				}
				if( bottomIndex >= 0 && bottomIndex > 0 && !cell.IsEmpty ) {
					var dstCell = cells[bottomIndex, col];
					dstCell.Copy( cell );
					cell.Clear();
					--bottomIndex;
					cellMoveEvent?.Invoke();
				}
			}
		}

		private void MoveColumnUp( int col ) {
			int upperIndex = int.MinValue;
			for( int row = 0; row < rowCount; ++row ) {
				var cell = cells[row, col];
				if( cell.IsEmpty && upperIndex < 0 ) {
					upperIndex = row;
				}
				if( upperIndex >= 0 && upperIndex < rowCount && !cell.IsEmpty ) {
					var dstCell = cells[upperIndex, col];
					dstCell.Copy( cell );
					cell.Clear();
					++upperIndex;
					cellMoveEvent?.Invoke();
				}
			}
		}

		public bool CheckValidMove() {
			for( int row = 0; row < rowCount; ++row ) {
				for( int col = 0; col < colCount; ++col ) {
					if( cells[row, col] == 0 ) {
						return true;
					}
					if( ( col + 1 ) < colCount && cells[row, col].Value == cells[row, col + 1].Value ) {
						return true;
					}
					if( ( row + 1 ) < rowCount && cells[row, col].Value == cells[row + 1, col].Value ) {
						return true;
					}
				}
			}
			return false;
		}

		public void TestWin() {
			cells[0, 0].SetValue( 1024 );
			cells[0, 1].SetValue( 1024 );
		}
	}
}