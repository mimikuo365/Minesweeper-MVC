using System;
namespace Minesweeper.Models
{
	public class Game
	{
        public Cell[,] Board { get; set; }

        public bool IsFinished { get; set; }
	}
}

