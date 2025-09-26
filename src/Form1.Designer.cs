namespace KBAudioPlayer
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.peakMeterCtrl1 = new Ernzo.WinForms.Controls.PeakMeterCtrl();
            this.playTime = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.volumeSlider = new ColorSlider.ColorSlider();
            this.speakerButton = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.picSlider = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnToBottom = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnToTop = new System.Windows.Forms.Button();
            this.popupNotifier1 = new Tulpep.NotificationWindow.PopupNotifier();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.scrollbar1 = new Deveck.Ui.Controls.Scrollbar.CustomScrollbar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.lstSongs = new KBAudioPlayer.ListViewWithoutScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSlider)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "현재곡";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // peakMeterCtrl1
            // 
            this.peakMeterCtrl1.BandsCount = 19;
            this.peakMeterCtrl1.ColorHigh = System.Drawing.Color.Red;
            this.peakMeterCtrl1.ColorHighBack = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.peakMeterCtrl1.ColorMedium = System.Drawing.Color.Yellow;
            this.peakMeterCtrl1.ColorMediumBack = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(150)))));
            this.peakMeterCtrl1.ColorNormal = System.Drawing.Color.Green;
            this.peakMeterCtrl1.ColorNormalBack = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(150)))));
            this.peakMeterCtrl1.FalloffColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.peakMeterCtrl1.GridColor = System.Drawing.Color.Gainsboro;
            this.peakMeterCtrl1.LEDCount = 20;
            this.peakMeterCtrl1.Location = new System.Drawing.Point(84, 6);
            this.peakMeterCtrl1.Name = "peakMeterCtrl1";
            this.peakMeterCtrl1.Size = new System.Drawing.Size(198, 67);
            this.peakMeterCtrl1.TabIndex = 10;
            // 
            // playTime
            // 
            this.playTime.Tick += new System.EventHandler(this.playTime_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "시작";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(247, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "끝";
            // 
            // volumeSlider
            // 
            this.volumeSlider.BackColor = System.Drawing.Color.Transparent;
            this.volumeSlider.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.volumeSlider.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.volumeSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.volumeSlider.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.volumeSlider.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.volumeSlider.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.volumeSlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.volumeSlider.ForeColor = System.Drawing.Color.White;
            this.volumeSlider.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.volumeSlider.Location = new System.Drawing.Point(177, 114);
            this.volumeSlider.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.volumeSlider.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.volumeSlider.Name = "volumeSlider";
            this.volumeSlider.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.volumeSlider.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.volumeSlider.ShowDivisionsText = true;
            this.volumeSlider.ShowSmallScale = false;
            this.volumeSlider.Size = new System.Drawing.Size(87, 25);
            this.volumeSlider.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.volumeSlider.TabIndex = 21;
            this.volumeSlider.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.volumeSlider.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.volumeSlider.ThumbRoundRectSize = new System.Drawing.Size(13, 13);
            this.volumeSlider.ThumbSize = new System.Drawing.Size(13, 13);
            this.volumeSlider.TickAdd = 0F;
            this.volumeSlider.TickColor = System.Drawing.Color.White;
            this.volumeSlider.TickDivide = 0F;
            this.volumeSlider.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // speakerButton
            // 
            this.speakerButton.FlatAppearance.BorderSize = 0;
            this.speakerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.speakerButton.Image = global::KBAudioPlayer.Properties.Resources._62789_speaker_medium_volume_icon;
            this.speakerButton.Location = new System.Drawing.Point(151, 115);
            this.speakerButton.Name = "speakerButton";
            this.speakerButton.Size = new System.Drawing.Size(27, 23);
            this.speakerButton.TabIndex = 20;
            this.speakerButton.TabStop = false;
            this.speakerButton.Tag = "cb.png";
            this.speakerButton.UseVisualStyleBackColor = true;
            this.speakerButton.Click += new System.EventHandler(this.speakerButton_Click);
            // 
            // button7
            // 
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Image = global::KBAudioPlayer.Properties.Resources.control_double_left_icon;
            this.button7.Location = new System.Drawing.Point(8, 116);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(18, 18);
            this.button7.TabIndex = 18;
            this.button7.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::KBAudioPlayer.Properties.Resources.control_double_right_icon;
            this.btnNext.Location = new System.Drawing.Point(102, 116);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(18, 18);
            this.btnNext.TabIndex = 17;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(148, 409);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(30, 23);
            this.btnDelete.TabIndex = 16;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::KBAudioPlayer.Properties.Resources.Music_Folder_icon;
            this.pictureBox2.Location = new System.Drawing.Point(8, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(65, 65);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // picSlider
            // 
            this.picSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picSlider.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.picSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSlider.Location = new System.Drawing.Point(43, 99);
            this.picSlider.Name = "picSlider";
            this.picSlider.Size = new System.Drawing.Size(200, 10);
            this.picSlider.TabIndex = 11;
            this.picSlider.TabStop = false;
            this.picSlider.Paint += new System.Windows.Forms.PaintEventHandler(this.picSlider_Paint);
            this.picSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSlider_MouseDown);
            this.picSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picSlider_MouseMove);
            this.picSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picSlider_MouseUp);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = global::KBAudioPlayer.Properties.Resources.text_plus_icon;
            this.button3.Location = new System.Drawing.Point(242, 409);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 23);
            this.button3.TabIndex = 7;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnStop
            // 
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::KBAudioPlayer.Properties.Resources.control_stop_icon;
            this.btnStop.Location = new System.Drawing.Point(70, 116);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(18, 18);
            this.btnStop.TabIndex = 6;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::KBAudioPlayer.Properties.Resources.control_right_icon;
            this.button1.Location = new System.Drawing.Point(40, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(18, 18);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnToBottom
            // 
            this.btnToBottom.FlatAppearance.BorderSize = 0;
            this.btnToBottom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToBottom.Image = global::KBAudioPlayer.Properties.Resources.arrow_stop_270_icon;
            this.btnToBottom.Location = new System.Drawing.Point(112, 409);
            this.btnToBottom.Name = "btnToBottom";
            this.btnToBottom.Size = new System.Drawing.Size(30, 23);
            this.btnToBottom.TabIndex = 4;
            this.btnToBottom.UseVisualStyleBackColor = true;
            this.btnToBottom.Click += new System.EventHandler(this.btnToBottom_Click);
            // 
            // btnDown
            // 
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Image = global::KBAudioPlayer.Properties.Resources.arrow_skip_270_icon;
            this.btnDown.Location = new System.Drawing.Point(76, 409);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(30, 23);
            this.btnDown.TabIndex = 3;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Image = global::KBAudioPlayer.Properties.Resources.arrow_skip_090_icon;
            this.btnUp.Location = new System.Drawing.Point(40, 409);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(30, 23);
            this.btnUp.TabIndex = 2;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnToTop
            // 
            this.btnToTop.FlatAppearance.BorderSize = 0;
            this.btnToTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToTop.Image = global::KBAudioPlayer.Properties.Resources.arrow_stop_090_icon;
            this.btnToTop.Location = new System.Drawing.Point(4, 409);
            this.btnToTop.Name = "btnToTop";
            this.btnToTop.Size = new System.Drawing.Size(30, 23);
            this.btnToTop.TabIndex = 1;
            this.btnToTop.UseVisualStyleBackColor = true;
            this.btnToTop.Click += new System.EventHandler(this.btnToTop_Click);
            // 
            // popupNotifier1
            // 
            this.popupNotifier1.BodyColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.popupNotifier1.ContentFont = new System.Drawing.Font("Segoe UI", 12F);
            this.popupNotifier1.ContentText = null;
            this.popupNotifier1.GradientPower = 300;
            this.popupNotifier1.HeaderHeight = 20;
            this.popupNotifier1.Image = null;
            this.popupNotifier1.IsRightToLeft = false;
            this.popupNotifier1.OptionsMenu = null;
            this.popupNotifier1.Size = new System.Drawing.Size(600, 100);
            this.popupNotifier1.TitleFont = new System.Drawing.Font("Segoe UI", 12F);
            this.popupNotifier1.TitleText = null;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(265, 233);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lstSongs);
            this.tabPage1.Controls.Add(this.scrollbar1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(265, 233);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // scrollbar1
            // 
            this.scrollbar1.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollbar1.LargeChange = 10;
            this.scrollbar1.Location = new System.Drawing.Point(248, -12);
            this.scrollbar1.Maximum = 99;
            this.scrollbar1.Minimum = 0;
            this.scrollbar1.Name = "scrollbar1";
            this.scrollbar1.Size = new System.Drawing.Size(21, 256);
            this.scrollbar1.SmallChange = 1;
            this.scrollbar1.TabIndex = 0;
            this.scrollbar1.ThumbStyle = Deveck.Ui.Controls.Scrollbar.CustomScrollbar.ThumbStyleEnum.Auto;
            this.scrollbar1.Value = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(4, 144);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(273, 259);
            this.tabControl1.TabIndex = 22;
            // 
            // lstSongs
            // 
            this.lstSongs.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lstSongs.AllowDrop = true;
            this.lstSongs.AutoArrange = false;
            this.lstSongs.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.lstSongs.Dock = System.Windows.Forms.DockStyle.Left;
            this.lstSongs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lstSongs.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lstSongs.FullRowSelect = true;
            this.lstSongs.GridLines = true;
            this.lstSongs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstSongs.HideSelection = false;
            this.lstSongs.Location = new System.Drawing.Point(3, 3);
            this.lstSongs.Name = "lstSongs";
            this.lstSongs.ShowItemToolTips = true;
            this.lstSongs.Size = new System.Drawing.Size(239, 227);
            this.lstSongs.TabIndex = 19;
            this.lstSongs.UseCompatibleStateImageBehavior = false;
            this.lstSongs.View = System.Windows.Forms.View.Details;
            this.lstSongs.VScrollbar = null;
            
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 441);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.speakerButton);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.picSlider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnToBottom);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnToTop);
            this.Controls.Add(this.peakMeterCtrl1);
            this.Controls.Add(this.volumeSlider);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 800);
            this.MinimumSize = new System.Drawing.Size(300, 460);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "KBAudio 플레이어";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSlider)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnToTop;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnToBottom;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Timer timer1;
        private Ernzo.WinForms.Controls.PeakMeterCtrl peakMeterCtrl1;
        private System.Windows.Forms.Timer playTime;
        private System.Windows.Forms.PictureBox picSlider;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button speakerButton;
        private ColorSlider.ColorSlider volumeSlider;
        private Tulpep.NotificationWindow.PopupNotifier popupNotifier1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        //private System.Windows.Forms.ListView lstSongs;
        private ListViewWithoutScrollBar lstSongs;
        private System.Windows.Forms.TabControl tabControl1;
        //private Deveck.Ui.Controls.CustomListView lstSongs;
        private Deveck.Ui.Controls.Scrollbar.CustomScrollbar scrollbar1;
    }
}

