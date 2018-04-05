using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.GUI
{
    static public class FontHelper
    {

        public static Font GetFittingFont(Control host, Rectangle testRect, string testLabel, StringFormat format = null, string fontFamilyName = "Arial")
        {
            if (testLabel == "")
                return null;
            int size = 2;
            int size_step = 2;
            using (Graphics g = host.CreateGraphics())
            {
                do
                {
                    Font font = new Font(fontFamilyName, size, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF stringSize;
                    if (format != null)
                        stringSize = g.MeasureString(testLabel, font, testRect.Width, format);
                    else
                        stringSize = g.MeasureString(testLabel, font);
                    font.Dispose();
                    if (stringSize.Height > testRect.Height || stringSize.Width > testRect.Width) break;
                    size += size_step;
                } while (true);
                g.Dispose();
                int final_size = size - size_step;
                if (final_size < 1) final_size = 1;
                return new Font(fontFamilyName, final_size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        public static Font GetFittingFont(Control host, Rectangle testRect, int padding, List<string> testLabels, StringFormat format = null, string fontFamilyName = "Arial")
        {
            if (testLabels.Count == 0) return null;
            int size = 2;
            int size_step = 2;
            testRect.Width = testRect.Width - 2 * padding;
            testRect.Height = testRect.Height - 2 * padding;
            if (format == null)
                format = new StringFormat();
            //find widest label to start
            using (Graphics g = host.CreateGraphics())
            {
                Font font = new Font(fontFamilyName, 20, FontStyle.Regular, GraphicsUnit.Pixel);
                string maxLabel = "";
                float maxWidth = 0f;
                foreach (string testLabel in testLabels)
                {
                    SizeF stringSize = g.MeasureString(testLabel, font);
                    if (stringSize.Width > maxWidth)
                    {
                        maxWidth = stringSize.Width;
                        maxLabel = testLabel;
                    }
                }
                font.Dispose();
                do
                {
                    font = new Font(fontFamilyName, size, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF stringSize = g.MeasureString(maxLabel, font);
                    font.Dispose();
                    if (stringSize.Height > testRect.Height || stringSize.Width > testRect.Width) break;
                    size += size_step;
                } while (true);
                g.Dispose();
                int final_size = size - size_step;
                if (final_size < 1) final_size = 1;
                return new Font(fontFamilyName, final_size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

    }
}
