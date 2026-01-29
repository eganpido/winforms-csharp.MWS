using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MWS.Controls
{
    [DesignerCategory("Code")]
    [DefaultProperty("BarColor")]
    public class ColoredProgressBar : ProgressBar
    {
        private Color barColor = Color.Red;

        [Category("Appearance")]
        [Description("The fill color of the progress bar.")]
        public Color BarColor
        {
            get { return barColor; }
            set { barColor = value; Invalidate(); }
        }

        public ColoredProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.Maximum = 100;
            this.Minimum = 0;
            this.Value = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            Graphics g = e.Graphics;

            // Draw background
            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(-1, -1);

            if (this.Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y,
                    (int)(rect.Width * ((double)this.Value / this.Maximum)), rect.Height);

                using (Brush brush = new SolidBrush(barColor))
                {
                    g.FillRectangle(brush, clip);
                }
            }

            // Draw percentage text
            string text = this.Value + "%";
            using (Font font = new Font("Open Sans", 10, FontStyle.Regular))
            {
                SizeF textSize = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.Black,
                    (rect.Width - textSize.Width) / 2,
                    (rect.Height - textSize.Height) / 2);
            }
        }
    }
}
