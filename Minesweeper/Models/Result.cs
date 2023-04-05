namespace Minesweeper.Models;

public class Result
{
    public List<string> ImpactedCells { get; set; }

    public List<string> CorrespondingMines { get; set; }

    public List<string> SelectedMines { get; set; }

    public bool Status { get; set; }

    public string CurrentCellValue { get; set; }

}

