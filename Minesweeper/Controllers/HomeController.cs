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

        if (this.Minesweeper.Board[row, col].IsOpened == false)
        {
            ClickOnCell(row, col);
        }
        return this.Minesweeper.Board[row, col].Value == "0" ? "" : this.Minesweeper.Board[row, col].Value;
    }

    private void ClickOnCell(int row, int col)
    {
        this.Minesweeper.Board[row, col].IsOpened = true;
        int numOfnearbyMine = CheckNearbyForMine(row, col);
        if (numOfnearbyMine == 0)
        {
            //TODO: Create a function to recursively open nearby not-mine cell
            //OpenNearbyCell();
        }
        this.Minesweeper.Board[row, col].Value = numOfnearbyMine.ToString();
    }

    private int CheckNearbyForMine(int row, int col)
    {
        int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        int numOfnearbyMine = 0;
        for (int i = 0; i < surroundings.GetLength(0); i++)
        {
            int neighRow = row + surroundings[i, 0];
            int neighCol = col + surroundings[i, 1];

            if (neighRow >= 0 && neighRow < NumOfRow &&
                neighCol >= 0 && neighCol < NumOfColumn &&
                this.Minesweeper.Board[neighRow, neighCol].IsMine)
                numOfnearbyMine++;
        }
        return numOfnearbyMine;
    }

    private void OpenNearbyCell(int row, int col)
    {
        this.Minesweeper.Board[row, col].IsOpened = true;

        int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        for (int i = 0; i < surroundings.GetLength(0); i++)
        {
            int neighRow = row + surroundings[i, 0];
            int neighCol = col + surroundings[i, 1];

            if (neighRow >= 0 && neighRow < NumOfRow &&
                neighCol >= 0 && neighCol < NumOfColumn &&
                !this.Minesweeper.Board[neighRow, neighCol].IsMine)
                OpenNearbyCell(neighRow, neighCol);
        }
    }
}
