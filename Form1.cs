using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Solver
{
    public partial class Form1 : Form
    {
        Cell[,] cells = new Cell[9, 9];
        public Form1()
        {
            InitializeComponent();
            PrepareGui();
        }
        void PrepareGui()
        {
            Font font = new Font(new FontFamily("Arial"),Cell.cellSize,
                FontStyle.Regular,GraphicsUnit.Pixel);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = new Cell(i, j);
                    board.Controls.Add(cell);
                    cells[i, j] = cell;
                }
            }
           /* var btnSolve = new Button();
            btnSolve.
            btnSolve.Text = "Solve";
            this.Controls.Add(btnSolve);*/
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(openFileDialog1.FileName);
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        char value = lines[i][j];
                        if (!char.IsDigit(value))
                            throw new Exception("value not valid");
                        var cell = cells[i, j];
                        cell.Mode = value == '0' ? Cell.CellMode.Empty : Cell.CellMode.Edited;
                        cell.Value = int.Parse(value.ToString());
                    }
                }
                Cell.active = null;
            }
            CheckBoard();
        }

        void Solve()
        {
            List<Cell> empty = new List<Cell>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (cells[i, j].Value == 0)
                    {
                        empty.Add(cells[i, j]);

                    }
                }
            }

            int idx = 0;
            while (idx<empty.Count)
            {
                if (idx < 0)
                    break;
                Cell cell = empty[idx];
                if (cell.Value == 0)
                {
                    ComputePoss(cell);
                }
                if (cell.PossIdx >= cell.Possible.Count)
                {
                    idx--;
                    cell.Value = 0;
                    cell.Mode = Cell.CellMode.Empty;
                    continue;
                }
                cell.Value = cell.Possible[cell.PossIdx];
                cell.Mode = Cell.CellMode.Computed;
                cell.PossIdx++;
                idx++;
            }
            Invalidate();
            //CheckBoard();
            if(idx<0)
                MessageBox.Show("Puzzle is Unsolvable");
            else
                MessageBox.Show("Puzzle is Solved");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Solve();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        public void ComputePoss(Cell cell)
        {
            if (cell.Value != 0)
            {
                cell.Possible = null;
                return;
            }
            cell.Possible = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            cell.PossIdx = 0;
            for (int k = 0; k < 9; k++)
            {
                Cell other = cells[cell.I, k];
                if (other == cell)
                    continue;
                if (cell.Possible.Contains(other.Value))
                    cell.Possible.Remove(other.Value);
            }

            for (int k = 0; k < 9; k++)
            {
                Cell other = cells[k, cell.J];
                if (other == cell)
                    continue;
                if (cell.Possible.Contains(other.Value))
                    cell.Possible.Remove(other.Value);
            }

            int i0 = cell.I - cell.I % 3;
            int j0 = cell.J - cell.J % 3;
            for (int ii = i0; ii < i0+3; ii++)
            {
                for (int jj = j0; jj < j0+3; jj++)
                {
                    Cell other = cells[ii, jj];
                    if (other == cell)
                        continue;
                    if (cell.Possible.Contains(other.Value))
                        cell.Possible.Remove(other.Value);
                }
            }
        }

        public void CheckBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = cells[i, j];
                    if (cell.State == Cell.CellState.Selected)
                        continue;
                    CheckCell(cell);
                }
            }
        }

        public void CheckCell(Cell cell)
        {
            if (cell.Value == 0)
            {
                cell.State = Cell.CellState.Normal;
                return;
            }
            for (int k = 0; k < 9; k++)
            {
                Cell other = cells[cell.I, k];
                if (other == cell)
                    continue;
                if (cell.Value == other.Value)
                {
                    cell.State = Cell.CellState.Error;
                    return;
                }
            }

            for (int k = 0; k < 9; k++)
            {
                Cell other = cells[k, cell.J];
                if (other == cell)
                    continue;
                if (cell.Value == other.Value)
                {
                    cell.State = Cell.CellState.Error;
                    return;
                }
            }

            int i0 = cell.I - cell.I % 3;
            int j0 = cell.J - cell.J % 3;
            for (int ii = i0; ii < i0 + 3; ii++)
            {
                for (int jj = j0; jj < j0 + 3; jj++)
                {
                    Cell other = cells[ii, jj];
                    if (other == cell)
                        continue;
                    if (cell.Value == other.Value)
                    {
                        cell.State = Cell.CellState.Error;
                        return;
                    }
                }
            }
            cell.State = Cell.CellState.Normal;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = cells[i, j];
                    if (cell.Mode == Cell.CellMode.Edited)
                        continue;
                    cell.Value = 0;
                    cell.State = Cell.CellState.Normal;
                    cell.Mode = Cell.CellMode.Empty;
                }
            }

            button2.Enabled = false;
            button1.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string txt = "";
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                        txt += cells[i,j].Mode == Cell.CellMode.Edited ? cells[i, j].Value:0;
                    txt += "\n";
                }
                File.WriteAllText(saveFileDialog1.FileName, txt);
            }
        }
    }
}
