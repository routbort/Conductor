using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conductor.GUI
{

    public class RotatableLabel : System.Windows.Forms.Label
    {

        private double rotationAngle;
        private string text;

        public RotatableLabel()
        {
            rotationAngle = 0d;
            this.Size = new Size(105, 12);
        }

        public double RotationAngle
        {
            get
            {
                return rotationAngle;
            }
            set
            {
                rotationAngle = value;
                this.Invalidate();
            }
        }

        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                this.Invalidate();
            }
        }




        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics graphics = e.Graphics;
            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                float width = graphics.MeasureString(text, this.Font).Width;
                float height = graphics.MeasureString(text, this.Font).Height;
                double angle = (rotationAngle / 180) * Math.PI;
                //Rectangle layoutRectangle = e.ClipRectangle;
                graphics.TranslateTransform(
                (ClientRectangle.Width + (float)(height * Math.Sin(angle)) - (float)(width * Math.Cos(angle))) / 2,
                (ClientRectangle.Height - (float)(height * Math.Cos(angle)) - (float)(width * Math.Sin(angle))) / 2);
                graphics.RotateTransform((float)rotationAngle);
                graphics.DrawString(text, this.Font, textBrush, 0, 0);
                graphics.ResetTransform();
            }
            // base.OnPaint(e);
        }

    }
}