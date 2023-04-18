using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int NumberOfMines { get; set; }
        public Cell[,] Cells { get; set; }
        public int Clicks = 0;

        public Board(Minesweeper minesweeper, int width, int height, int mines)
        {
            this.Minesweeper = minesweeper;
            this.Width = width;
            this.Height = height;
            this.NumberOfMines = mines;
            this.Cells = new Cell[width, height];
        }

        public void SetupBoard()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var c = new Cell
                    {
                        CellState = CellState.Closed,
                        CellType = CellType.Regular,
                        CellSize = 50,
                        Board = this,
                        XLoc = j,
                        YLoc = i,
                    };
                    c.SetupDesign();
                    c.MouseDown += Cell_MouseClick;
                    this.Cells[i, j] = c;
                    this.Minesweeper.Controls.Add(c);
                }
            }
        }

        public void SetupMines(int y, int x)
        {
            Random rnd = new Random();
            for (int i = 0; i < NumberOfMines; i++)
            {
                int row;
                int col;
                do
                {
                    row = rnd.Next(Height);
                    col = rnd.Next(Width);
                } while (Cells[row, col].CellType == CellType.Mine || Cells[row, col] == Cells[y, x]);

                this.Cells[row, col].CellType = CellType.Mine;
            }
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (Cell) sender;
            if (cell.CellState == CellState.Opened)
                return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    cell.OnClick();
                    IsTheGameWon();
                    break;
                case MouseButtons.Right:
                    cell.OnFlag();
                    break;
                default:
                    return;
            }
        }

        public void IsTheGameWon()
        {
            int openedCells = 0;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Cells[i, j].CellType == CellType.Regular && Cells[i, j].CellState == CellState.Opened)
                    {
                        openedCells++;
                    }
                }
            }

            if (openedCells == (Height * Width) - NumberOfMines)
            {
                MessageBox.Show("You just won!");
                Application.Exit();
            }
        }
    }
}
