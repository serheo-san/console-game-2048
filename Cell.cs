using System;
using System.Collections.Generic;

namespace Game2048 {
	public class Cell {
		// Fields
		private ushort cellValue = 0;
		public bool NewFlag = false;
		public bool UnionFlag = false;
		
		// Constants
		private const int cellSize = 4;
		private static readonly Dictionary<ushort, ConsoleColor> cellColors = new Dictionary<ushort, ConsoleColor>() {
			{2, ConsoleColor.White},
			{4, ConsoleColor.Cyan},
			{8, ConsoleColor.DarkGreen},
			{16, ConsoleColor.Green},
			{32, ConsoleColor.DarkYellow},
			{64, ConsoleColor.Yellow},
			{128, ConsoleColor.DarkRed},
			{256, ConsoleColor.Red},
			{512, ConsoleColor.DarkMagenta},
			{1024, ConsoleColor.Magenta},
			{2048, ConsoleColor.Blue},
		};


		// Properties
		public ushort Value => cellValue;
		
		public bool IsEmpty => cellValue == 0;

		// Methods
		public void SetValue( ushort value ) {
			cellValue = value;
		}

		public void Copy( Cell cell ) {
			cellValue = cell.Value;
			UnionFlag = cell.UnionFlag;
			NewFlag = cell.NewFlag;
		}

		public void Unite( Cell cell ) {
			cellValue = (ushort) ( cell.Value + cellValue);
			SetValue( cellValue );
			UnionFlag = true;
			cell.Clear();
		}

		public void Clear() {
			cellValue = 0;
			NewFlag = false;
			UnionFlag = false;
		}
		
		public void DrawCell( ) {
			var prevBackColor = Console.BackgroundColor;
			var prevColor = Console.ForegroundColor;
			if( NewFlag ) {
				Console.BackgroundColor = ConsoleColor.DarkRed;
				NewFlag = false;
			}
			if( UnionFlag ) {
				Console.BackgroundColor = ConsoleColor.DarkBlue;
				UnionFlag = false;
			}
			var strValue = cellValue > 0 ? cellValue.ToString() : string.Empty;
			var strValueLength = strValue.Length;
			if( strValueLength < cellSize ) {
				for( int i = 0; i < ( cellSize - strValueLength ); ++i ) {
					Console.Write( " " );
				}
			}
			if( !string.IsNullOrEmpty( strValue ) ) {
				Console.ForegroundColor = GetValueColor( cellValue );
				Console.Write( strValue );
			}
			Console.BackgroundColor = prevBackColor;
			Console.ForegroundColor = prevColor;
		}
		
		private ConsoleColor GetValueColor( ushort cellValue ) {
			if( cellColors.TryGetValue( cellValue, out var color ) ) {
				return color;
			}
			return ConsoleColor.White;
		}

		public static implicit operator ushort (Cell cell) {
			return cell.cellValue;
		}
	}
}