using System.Windows.Forms;

namespace ImageProcessingActivity
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        int mode = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = 0;
            openFileDialog1.ShowDialog();
            // processed.Save(saveFileDialog1.FileName);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bitmap opened_file = new Bitmap(openFileDialog1.FileName);
            switch (mode)
            {
                case 0:
                    loaded = opened_file;
                    pictureBox1.Image = loaded;
                    break;
                case 1:
                    inputbg = opened_file;
                    pbBackground.Image = inputbg;
                    break;
                case 2:
                    inputfg = opened_file;
                    pbForeground.Image = inputfg;
                    break;
            }
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel((int)x, (int)y);
                    ave = (int)((pixel.R + pixel.G + pixel.B) / 3);
                    Color gray = Color.FromArgb(ave, ave, ave);
                    processed.SetPixel(x, y, gray);
                }
            pictureBox2.Image = processed;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel((int)x, (int)y);
                    Color res = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(x, y, res);
                }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap loadedOG = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    loadedOG.SetPixel(x, y, pixel);
                }
            }
            Color sample;
            Color gray;
            Byte graydata;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    sample = loaded.GetPixel((int)x, (int)y);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    loaded.SetPixel(x, y, gray);
                }
            }
            int[] histdata = new int[256];
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    sample = loaded.GetPixel(x, y);
                    histdata[sample.R]++;
                }
            }

            processed = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    processed.SetPixel(x, y, Color.White);
                }
            }
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, processed.Height - 1); y++)
                {
                    processed.SetPixel(x, (processed.Height - 1) - y, Color.Black);
                }
            }

            pictureBox2.Image = processed;

        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            // 0.393, 0.769, 0.189
            // 0.349, 0.686, 0.168s
            // 0.272, 0.534, 0.131
            Color pixel;
            int tr, tg, tb;
            for (int x = 0; x < loaded.Width; x++)
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel((int)x, (int)y);
                    tr = (int)Math.Min(255, 0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
                    tg = (int)Math.Min(255, 0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
                    tb = (int)Math.Min(255, 0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);
                    Color sepia = Color.FromArgb(tr, tg, tb);
                    processed.SetPixel(x, y, sepia);
                }
            pictureBox2.Image = processed;

        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }


        Bitmap inputbg, inputfg;

        private void btnLoadBg_Click(object sender, EventArgs e)
        {
            mode = 1;
            openFileDialog1.ShowDialog();
        }

        private void btnLoadFg_Click(object sender, EventArgs e)
        {
            mode = 2;
            openFileDialog1.ShowDialog();
        }
        private void chromaKey(Color chroma, int threshold)
        {
            int avg = (chroma.R + chroma.G + chroma.B) / 3;
            if (inputbg.Width != inputfg.Width || inputbg.Height != inputfg.Height)
                stretchBG();
            processed = new Bitmap(inputfg.Width, inputfg.Height);
            Color fp, bp;
            for (int x = 0; x < inputfg.Width; x++)
            {
                for (int y = 0; y < inputfg.Height; y++)
                {
                    fp = inputfg.GetPixel(x, y);
                    bp = inputbg.GetPixel(x, y);
                    int grey = (fp.R + fp.G + fp.B) / 3;
                    int subtractvalue = Math.Abs(grey - avg);
                    if (subtractvalue < threshold)
                        processed.SetPixel(x, y, bp);
                    else
                        processed.SetPixel(x, y, fp);
                }
            }
            pbOutput.Image = processed;
        }
        private void stretchBG()
        {
            processed = new Bitmap(inputbg.Width, inputbg.Height);
            Color px;
            for (int x = 0; x < inputbg.Width; x++)
            {
                for (int y = 0; y < inputbg.Height; y++)
                {
                    px = inputbg.GetPixel(x, y);
                    processed.SetPixel(x, y, px);
                }
            }
            inputbg = new Bitmap(inputfg.Width, inputfg.Height);
            for (int x = 0; x < inputfg.Width; x++)
            {
                for (int y = 0; y < inputfg.Height; y++)
                {
                    int dx = normal(inputfg.Width, processed.Width, x);
                    int dy = normal(inputfg.Height, processed.Height, y);
                    px = processed.GetPixel(dx, dy);
                    inputbg.SetPixel(x, y, px);
                }
            }
        }
        private int normal(int src, int dst, int px)
        {
            src -= 1;
            dst -= 1;
            double ratio = ((double)dst) / ((double)src);
            return Math.Min(
                dst,
                (int)Math.Round(ratio * px));
        }

        private void btnSubtractGS_Click(object sender, EventArgs e)
        {
            Color mygreen = Color.FromArgb(0, 0, 255);
            int threshold = 10;
            chromaKey(mygreen, threshold);
        }
    }
}
