using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Models;

namespace Minesweeper.Controllers;

public class HomeController : Controller
{
    public Game Minesweeper;
    const int NumOfColumn = 12;
    const int NumOfRow = 17;

    public HomeController()
    {
        this.Minesweeper = new Game
        {
            Board = new Cell[NumOfRow, NumOfColumn],
            IsFinished = false
        };

        for (int i = 0; i < this.Minesweeper.Board.GetLength(0); i++)
            for(int j = 0; j < this.Minesweeper.Board.GetLength(1); j++)
            {
                this.Minesweeper.Board[i, j] = new Cell()
                {
                    Value = "O",
                    IsMine = false,
                    IsOpened = false
                };
            }
        for (int i = 0; i < this.Minesweeper.Board.GetLength(1); i++)
            this.Minesweeper.Board[1, i].IsMine = true;
    }

    public IActionResult Index()
    {
        return View(this);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    //TODO: Create a function to randomly generate mine in a game
    //public
    public string UpdateTable(string data)
    {
        int row = Int32.Parse(data) / NumOfColumn;
        int col = Int32.Parse(data) % NumOfColumn;

        ClickOnCell(row, col);
        return this.Minesweeper.Board[row, col].Value == "0" ? "" : this.Minesweeper.Board[row, col].Value;
    }

    private int CheckNearbyForMine(int row, int col)
    {
        Console.WriteLine(row + " " + col);
        int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        int numOfNearbyMine = 0;
        for (int i = 0; i < surroundings.GetLength(0); i++)
        {
            int neighRow = row + surroundings[i, 0];
            int neighCol = col + surroundings[i, 1];

            if (neighRow >= 0 && neighRow < NumOfRow &&
                neighCol >= 0 && neighCol < NumOfColumn &&
                this.Minesweeper.Board[neighRow, neighCol].IsMine)
                numOfNearbyMine++;
        }
        return numOfNearbyMine;
    }

    private void ClickOnCell(int row, int col)
    {
        if (row < 0 || row >= NumOfRow || col < 0 || col >= NumOfColumn ||
            this.Minesweeper.Board[row, col].IsOpened == true)
            return;

        int numOfNearbyMine = CheckNearbyForMine(row, col);
        this.Minesweeper.Board[row, col].IsOpened = true;
        this.Minesweeper.Board[row, col].Value = numOfNearbyMine.ToString();

        if (numOfNearbyMine == 0)
        {
            int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            for (int i = 0; i < surroundings.GetLength(0); i++)
            {
                int neighRow = row + surroundings[i, 0];
                int neighCol = col + surroundings[i, 1];
                ClickOnCell(neighRow, neighCol);
            }
        }
    }
}
