using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Models;
using Newtonsoft.Json;

namespace Minesweeper.Controllers;

public class HomeController : Controller
{
    public Game Minesweeper;
    public Result ClickedResult;

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
        List<int> impactedCell = new List<int>();
        List<string> correspondingMines = new List<string>();
        int index = int.Parse(data);
        ClickOnCell(index, ref impactedCell, ref correspondingMines);

        this.ClickedResult = new Result()
        {
            ImpactedCells = impactedCell,
            CurrentCellValue = this.Minesweeper.Board[index / NumOfColumn, index % NumOfColumn].Value,
            Status = this.Minesweeper.Board[index / NumOfColumn, index % NumOfColumn].IsMine,
            CorrespondingMines = correspondingMines
        };
        return JsonConvert.SerializeObject(this.ClickedResult);
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

    private void ClickOnCell(int index, ref List<int> impactedCell, ref List<string> correspondingMines)
    {
        int row = index / NumOfColumn;
        int col = index % NumOfColumn;

        if (row < 0 || row >= NumOfRow || col < 0 || col >= NumOfColumn ||
            this.Minesweeper.Board[row, col].IsOpened == true)
            return;

        int numOfNearbyMine = CheckNearbyForMine(row, col);
        this.Minesweeper.Board[row, col].IsOpened = true;
        this.Minesweeper.Board[row, col].Value = numOfNearbyMine.ToString();
        impactedCell.Add(index);
        correspondingMines.Add(this.Minesweeper.Board[row, col].Value);

        if (numOfNearbyMine == 0)
        {
            int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            for (int i = 0; i < surroundings.GetLength(0); i++)
            {
                int neighRow = row + surroundings[i, 0];
                int neighCol = col + surroundings[i, 1];
                int newIndex = neighRow * NumOfColumn + neighCol;
                ClickOnCell(newIndex, ref impactedCell, ref correspondingMines);
            }
        }
    }
}
