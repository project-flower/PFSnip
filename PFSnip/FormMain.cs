using PFSnip.Properties;
using System;
using System.Windows.Forms;

namespace PFSnip
{
    public partial class FormMain : Form
    {
        #region Private Fields

        private readonly FormPreview formPreview = new FormPreview();

        #endregion

        #region Public Methods

        public FormMain()
        {
            InitializeComponent();
            Settings settings = Settings.Default;
            formPreview.Initialize(settings.PreviewShadowColor, settings.PreviewShadowAlpha, settings.PreviewFrameColor);
            formPreview.VisibleChanged += formPreview_VisibleChanged;
        }

        #endregion

        #region Private Methods

        private void StartCapture()
        {
            int interval;
            decimal value = numericUpDownDelay.Value;

            if (value == 0)
            {
                interval = 1;
            }
            else
            {
                interval = (int)(value * 1000);
            }

            Hide();
            timer.Interval = interval;
            timer.Start();
        }

        #endregion

        #region Private Methods

        private Operation GetCurrentOperation()
        {
            if (radioButtonClipboard.Checked) return Operation.DrawClipboard;

            if (radioButtonFile.Checked) return Operation.SaveFile;

            return Operation.None;
        }

        #endregion

        // Designer's Methods

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            StartCapture();
        }

        private void buttonOneMore_Click(object sender, EventArgs e)
        {
            try
            {
                formPreview.DoOperate(GetCurrentOperation());
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void formPreview_VisibleChanged(object sender, EventArgs e)
        {
            if (formPreview.IsDisposed)
            {
                return;
            }

            if (!formPreview.Visible && !Visible)
            {
                Show();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            formPreview.ShowPreview(GetCurrentOperation());
        }
    }
}
