using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PFSnip
{
    public partial class FormPreview : Form
    {
        #region Private Fields

        private Image baseImage = null;
        private Operation operation = Operation.None;
        private Color previewFrameColor;
        private Color previewShadowColor;
        private int prevX = int.MinValue;
        private int prevY = int.MinValue;
        private int windowIndex = -1;
        private (Rectangle, int)[] windows = new (Rectangle, int)[0];

        #endregion

        #region Public Methods

        public FormPreview()
        {
            InitializeComponent();

#if DEBUG
            TopMost = false;
#endif
        }

        public void DoOperate(Operation operation)
        {
            if (windowIndex < 0)
            {
                return;
            }

            Rectangle window = PointToClient(windows[windowIndex].Item1);

            try
            {
                switch (operation)
                {
                    case Operation.DrawClipboard:
                        DrawClipboard(window);
                        break;

                    case Operation.SaveFile:
                        SaveImage(window);
                        break;
                }
            }
            catch
            {
                throw;
            }
        }

        public void Initialize(Color previewShadowColor, byte previewShadowAlpha, Color previewFrameColor)
        {
            this.previewFrameColor = previewFrameColor;
            this.previewShadowColor = Color.FromArgb(previewShadowAlpha, previewShadowColor);
        }

        public void ShowPreview(Operation operation)
        {
            windows = CaptureEngine.CollectWindows(out Bitmap bitmap, out Rectangle totalScreenSize);
            windowIndex = -1;
            Image image = pictureBox.Image;
            pictureBox.Image = bitmap;
            baseImage = bitmap.Clone() as Image;
            image?.Dispose();

            if (!Visible)
            {
                Show();
            }

            Left = totalScreenSize.Left;
            Top = totalScreenSize.Top;
            Width = totalScreenSize.Width;
            Height = totalScreenSize.Height;
            this.operation = operation;
        }

        #endregion

        #region Private Methods

        private void DrawClipboard(Rectangle rectangle)
        {
            if (baseImage == null)
            {
                return;
            }

            try
            {
                using (Bitmap bitmap = GetSnippedImage(rectangle))
                {
                    Clipboard.SetImage(bitmap);
                }
            }
            catch
            {
                throw;
            }
        }

        private void EmphasizeRectangle(Rectangle rectangle)
        {
            if (baseImage == null)
            {
                return;
            }

            pictureBox.Image.Dispose();
            Bitmap bitmap = baseImage.Clone() as Bitmap;

            if (!rectangle.IsEmpty)
            {
                DrawingEngine.FillHollowRectangle(bitmap, rectangle, previewShadowColor, previewFrameColor);
            }

            pictureBox.Image = bitmap;
        }

        private Bitmap GetSnippedImage(Rectangle rectangle)
        {
            rectangle.Intersect(new Rectangle(0, 0, baseImage.Width, baseImage.Height));
            return (baseImage as Bitmap).Clone(rectangle, baseImage.PixelFormat);
        }

        private Rectangle PointToClient(Rectangle rectangle)
        {
            return new Rectangle(pictureBox.PointToClient(rectangle.Location), rectangle.Size);
        }

        private void SaveImage(Rectangle rectangle)
        {
            if (baseImage == null)
            {
                return;
            }

            string fileName = saveFileDialog.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                saveFileDialog.FileName = string.Empty;

                try
                {
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                }
                catch
                {
                }
            }

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (Bitmap bitmap = GetSnippedImage(rectangle))
            {
                bitmap.Save(saveFileDialog.FileName);
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        // Designer's Methods

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                DoOperate(operation);
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }

            Close();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.X == prevX) && (e.Y == prevY))
            {
                return;
            }

            prevX = e.X;
            prevY = e.Y;
            int candidate = -1;

            for (int i = 0; i < windows.Length; ++i)
            {
                (Rectangle, int) window = windows[i];
                Rectangle bounds = window.Item1;
                Point point = pictureBox.PointToScreen(e.Location);
                int x = point.X;
                int y = point.Y;

                if (x < bounds.Left) continue;
                if (y < bounds.Top) continue;
                if (x > bounds.Right) continue;
                if (y > bounds.Bottom) continue;

                if (candidate >= 0 && (window.Item2 > windows[candidate].Item2))
                {
                    continue;
                }

                candidate = i;
            }

            if ((candidate == -1) && (windowIndex != -1))
            {
                EmphasizeRectangle(Rectangle.Empty);
                windowIndex = -1;
            }
            else if ((candidate != -1) && (candidate != windowIndex))
            {
                EmphasizeRectangle(PointToClient(windows[candidate].Item1));
                windowIndex = candidate;
            }
        }
    }
}
