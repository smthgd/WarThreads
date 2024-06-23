namespace WarThreads
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.Gun = new System.Windows.Forms.PictureBox();
            this.panelGameSreen = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.Gun)).BeginInit();
            this.SuspendLayout();
            // 
            // Gun
            // 
            this.Gun.Image = global::WarThreads.Properties.Resources.SpaceFighter;
            this.Gun.Location = new System.Drawing.Point(656, 920);
            this.Gun.Name = "Gun";
            this.Gun.Size = new System.Drawing.Size(40, 40);
            this.Gun.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Gun.TabIndex = 0;
            this.Gun.TabStop = false;
            // 
            // panelGameSreen
            // 
            this.panelGameSreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGameSreen.Location = new System.Drawing.Point(0, 0);
            this.panelGameSreen.Name = "panelGameSreen";
            this.panelGameSreen.Size = new System.Drawing.Size(1267, 519);
            this.panelGameSreen.TabIndex = 1;
            this.panelGameSreen.Tag = "GameSreen";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1267, 519);
            this.Controls.Add(this.panelGameSreen);
            this.Controls.Add(this.Gun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "War Threads";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.Gun)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Gun;
        private System.Windows.Forms.Panel panelGameSreen;
    }
}

