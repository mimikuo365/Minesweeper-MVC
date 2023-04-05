using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Models;
using Newtonsoft.Json;

namespace Minesweeper.Controllers;

public class HomeController : Controller
{
    public Cell[,] Board { get; set; }
    static public List<string> Mines = GenerateMine();
    public Result ClickedResult;

    const int NumOfColumn = 12;
    const int NumOfRow = 17;

    public HomeController()
    {
        this.Board = new Cell[NumOfRow, NumOfColumn];

        for (int i = 0; i < this.Board.GetLength(0); i++)
            for (int j = 0; j < this.Board.GetLength(1); j++)
            {
                this.Board[i, j] = new Cell()
                {
                    Value = "O",
                    IsMine = false,
                    IsOpened = false
                };
            }

        for (int i = 0; i < Mines.Count; i++)
        {
            int index = Int32.Parse(Mines[i]);
            this.Board[index / NumOfColumn, index % NumOfColumn].IsMine = true;
        }
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

    static public List<string> GenerateMine() {
        List<string> Mines = new List<string>();
        int counter = 40;

        while (counter > 0)
        {
            Random rnd = new Random();
            int mineRow = rnd.Next(NumOfRow);
            int mineCol = rnd.Next(NumOfColumn);
            string mineIndex = (mineRow * NumOfColumn + mineCol).ToString();

            if (!Mines.Contains(mineIndex))
            {
                counter--;
                Mines.Add(mineIndex);
            }
        }
        return Mines;
    }

    public string UpdateTable(string data, bool isFirst)
    {
        List<string> impactedCell = new List<string>();
        List<string> correspondingMines = new List<string>();
        int index = int.Parse(data);
        int row = index / NumOfColumn;
        int col = index % NumOfColumn;

        if (isFirst)
        {
            this.Board[row, col].IsMine = false;
            Mines.Remove(data);
        }

        ClickOnCell(index, ref impactedCell, ref correspondingMines);

        this.ClickedResult = new Result()
        {
            ImpactedCells = impactedCell,
            CurrentCellValue = this.Board[row, col].Value,
            Status = this.Board[row, col].IsMine,
            CorrespondingMines = correspondingMines,
            SelectedMines = Mines
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
                this.Board[neighRow, neighCol].IsMine)
                numOfNearbyMine++;
        }
        return numOfNearbyMine;
    }

    private void ClickOnCell(int index, ref List<string> impactedCell, ref List<string> correspondingMines)
    {
        int row = index / NumOfColumn;
        int col = index % NumOfColumn;
        int numOfNearbyMine = CheckNearbyForMine(row, col);

        if (!this.Board[row,col].IsMine)
        {
            this.Board[row, col].IsOpened = true;
            this.Board[row, col].Value = numOfNearbyMine.ToString();
            impactedCell.Add(index.ToString());
            correspondingMines.Add(this.Board[row, col].Value);
            if (numOfNearbyMine == 0)
            {
                int[,] surroundings = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
                for (int i = 0; i < surroundings.GetLength(0); i++)
                {
                    int neighRow = row + surroundings[i, 0];
                    int neighCol = col + surroundings[i, 1];
                    int newIndex = neighRow * NumOfColumn + neighCol;

                    if (neighRow < 0 || neighRow >= NumOfRow || neighCol < 0 || neighCol >= NumOfColumn ||
                        this.Board[neighRow, neighCol].IsOpened)
                        continue;
                    ClickOnCell(newIndex, ref impactedCell, ref correspondingMines);
                }
            }
        }
    }
}
