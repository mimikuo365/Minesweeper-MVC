using System;
namespace Minesweeper.Models
{
	public class Cell
	{
		public bool IsOpened { get; set; }

		public bool IsMine { get; set; }

		public string Value { get; set; }
    }
}

