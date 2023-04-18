using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public enum CellType
    {
        Regular, Mine, Flagged, FlaggedMine
    }

    public enum CellState
    {
        Opened, Closed
    }

    public class Cell : Button
    {
        public int XLoc { get; set; }
        public int YLoc { get; set; }
        public int CellSize { get; set; }
        public CellState CellState { get; set; }
        public CellType CellType { get; set; }
        public int NumMines { get; set; } = 0;
        public Board Board { get; set; }
        public List<Cell> CellsWithZeroMines = new List<Cell>();
        public List<Cell> DoneCells = new List<Cell>();

        public void SetupDesign()
        {
            this.BackColor = SystemColors.ButtonFace;
            this.Location = new Point(XLoc * CellSize, YLoc * CellSize);
            this.Size = new Size(CellSize, CellSize);
            this.UseVisualStyleBackColor = false;
            this.Font = new Font("Verdana", 15.75F, FontStyle.Bold);
        }

        public void OnFlag()
        {
            BackColor = Color.Black;
        }

        public void OnClick(bool recursiveCall = false)
        {
            if (Board.Clicks == 0)
            {
                Board.SetupMines(YLoc, XLoc);
                Board.Clicks = 1;
            }
            if (CellType == CellType.Regular)
            {
                CheckSurroundingCells(YLoc, XLoc);
                if (NumMines == 0)
                {
                    CellsWithZeroMines.Add(this);
                    do
                    {
                        OpenSurroundingCells(CellsWithZeroMines[0].YLoc, CellsWithZeroMines[0].XLoc);
                        DoneCells.Add(CellsWithZeroMines[0]);
                        CellsWithZeroMines.RemoveAt(0);
                    } while (CellsWithZeroMines.Count > 0);
                }
                CellState = CellState.Opened;
                BackColor = GetCellColour();
                NumMines = 0;
            }
            else if (CellType == CellType.Mine)
            {
                BackColor = Color.Black;
                MessageBox.Show("You just lost!");
                Application.Exit();
            }
        }

        public void CheckSurroundingCells(int y, int x)
        {
            for (int i = y - 1; i < y + 2; i++)
            {
                for (int j = x - 1; j < x + 2; j++)
                {
                    if (i >= 0 && i < Board.Width && j >= 0 && j < Board.Height)
                    {
                        if (Board.Cells[i, j].CellType == CellType.Mine)
                        {
                            NumMines++;
                        }
                    }
                }
            }
        }

        public void OpenSurroundingCells(int y, int x)
        {
            for (int i = y - 1; i < y + 2; i++)
            {
                for (int j = x - 1; j < x + 2; j++)
                {
                    if (i >= 0 && i < Board.Width && j >= 0 && j < Board.Height)
                    {
                        CheckSurroundingCells(i, j);
                        Board.Cells[i, j].CellState = CellState.Opened;
                        Board.Cells[i, j].BackColor = GetCellColour();
                        if (NumMines == 0 && !DoneCells.Contains(Board.Cells[i, j]) && !CellsWithZeroMines.Contains(Board.Cells[i, j]))
                        {
                            CellsWithZeroMines.Add(Board.Cells[i, j]);
                        }
                        NumMines = 0;
                    }
                }
            }
        }

        private Color GetCellColour()
        {
            switch (this.NumMines)
            {
                case 1:
                    return ColorTranslator.FromHtml("0x0000FE"); // 1
                case 2:
                    return ColorTranslator.FromHtml("0x186900"); // 2
                case 3:
                    return ColorTranslator.FromHtml("0xAE0107"); // 3
                case 4:
                    return ColorTranslator.FromHtml("0x000177"); // 4
                case 5:
                    return ColorTranslator.FromHtml("0x8D0107"); // 5
                case 6:
                    return ColorTranslator.FromHtml("0x007A7C"); // 6
                case 7:
                    return ColorTranslator.FromHtml("0x902E90"); // 7
                case 8:
                    return ColorTranslator.FromHtml("0x000000"); // 8+
                default:
                    return ColorTranslator.FromHtml("0xffffff");
            }
        }
    }
}
