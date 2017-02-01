using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku_Solver
{
    public partial class Cell : Control
    {
        public enum CellMode
        {
            Edited,
            Computed,
            Empty
        }

        public enum CellState
        {
            Selected,
            Error,
            Normal
        }

        public bool Edited { get; set; }
        public static int cellSize = 40;

        static Font font = new Font(new FontFamily("Arial"), cellSize, 
            FontStyle.Regular, GraphicsUnit.Pixel);
        static Point offset = new Point(3, 0);

        int value = 0;
        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                Invalidate();
            }
        }

        CellMode mode = CellMode.Empty;
        public CellMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                Invalidate();
            }
        }

        CellState state = CellState.Normal;
        public CellState State
        {
            get { return state; }
            set
            {
                state = value;
                Invalidate();
            }
        }

        public int I { get; }
        public int J { get; }
        public Cell(int i, int j)
        {
            I = i;
            J = j;            
            Location = new Point(j * (cellSize + 1) + j / 3 * 5, i * (cellSize + 1) + i / 3 * 5);
            Size = new Size(cellSize, cellSize);
            State = CellState.Normal;
            Mode = CellMode.Empty;    
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            switch (state)
            {
                case CellState.Selected:
                    pe.Graphics.Clear(Color.DarkGreen);
                    if (value > 0)
                        pe.Graphics.DrawString(value.ToString(), font, Brushes.White, new RectangleF(offset, this.Size));
                    break;
                case CellState.Error:
                    pe.Graphics.Clear(Color.DarkRed);
                    if (value > 0)
                        pe.Graphics.DrawString(value.ToString(), font, Brushes.White, new RectangleF(offset, this.Size));
                    break;
                default:
                    break;
            }
            if (state != CellState.Normal)
                return;
            switch (Mode)
            {
                case CellMode.Edited:
                    pe.Graphics.Clear(Color.DarkBlue);
                    pe.Graphics.DrawString(value.ToString(), font, Brushes.White, new RectangleF(offset, this.Size));
                    break;
                case CellMode.Computed:
                    pe.Graphics.Clear(Color.LightBlue);
                    pe.Graphics.DrawString(value.ToString(), font, Brushes.Black, new RectangleF(offset, this.Size));
                    break;
                case CellMode.Empty:
                    pe.Graphics.Clear(Color.White);
                    break;                
                default:
                    break;
            }
            
        }
        public static Cell active = null;
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (active != null)
                ((Form1)FindForm()).CheckCell(active);
            State = CellState.Selected;
            active = this;
            active.Focus();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (char.IsDigit(e.KeyChar))
            {
                Value = int.Parse(e.KeyChar.ToString());
                Mode = Value == 0?  CellMode.Empty : CellMode.Edited;
                ((Form1)FindForm()).CheckBoard();
            }
        }
        public List<int> Possible = null;
        public int PossIdx = 0;
    }
}
