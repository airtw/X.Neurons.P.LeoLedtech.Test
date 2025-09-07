namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            svgImageBox1 = new DevExpress.XtraEditors.SvgImageBox();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            simpleLabelItem2 = new DevExpress.XtraLayout.SimpleLabelItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)svgImageBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem2).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.AllowCustomization = false;
            layoutControl1.Controls.Add(svgImageBox1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Margin = new Padding(2);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(1544, 388, 813, 831);
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(600, 370);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // svgImageBox1
            // 
            svgImageBox1.Location = new Point(12, 121);
            svgImageBox1.Name = "svgImageBox1";
            svgImageBox1.Size = new Size(576, 63);
            svgImageBox1.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            svgImageBox1.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("svgImageBox1.SvgImage");
            svgImageBox1.TabIndex = 4;
            svgImageBox1.Text = "svgImageBox1";
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { simpleLabelItem1, layoutControlItem1, emptySpaceItem1, emptySpaceItem2, simpleLabelItem2 });
            Root.Name = "Root";
            Root.Size = new Size(600, 370);
            Root.TextVisible = false;
            // 
            // simpleLabelItem1
            // 
            simpleLabelItem1.AppearanceItemCaption.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
            simpleLabelItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            simpleLabelItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            simpleLabelItem1.Location = new Point(0, 200);
            simpleLabelItem1.Name = "simpleLabelItem1";
            simpleLabelItem1.Size = new Size(580, 25);
            simpleLabelItem1.Text = "版本:v1.0.0";
            simpleLabelItem1.TextSize = new Size(238, 21);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = svgImageBox1;
            layoutControlItem1.Location = new Point(0, 109);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(580, 67);
            layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(0, 0);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(580, 109);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(0, 225);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(580, 125);
            // 
            // simpleLabelItem2
            // 
            simpleLabelItem2.AppearanceItemCaption.Font = new Font("微軟正黑體", 12F);
            simpleLabelItem2.AppearanceItemCaption.Options.UseFont = true;
            simpleLabelItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            simpleLabelItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            simpleLabelItem2.Location = new Point(0, 176);
            simpleLabelItem2.Name = "simpleLabelItem2";
            simpleLabelItem2.Size = new Size(580, 24);
            simpleLabelItem2.Text = "啟耀科技 - 生產測試 - 總電流測試";
            simpleLabelItem2.TextSize = new Size(238, 20);
            // 
            // About
            // 
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(6F, 12F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 370);
            Controls.Add(layoutControl1);
            Font = new Font("新細明體", 9F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MaximumSize = new Size(600, 400);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(600, 400);
            Name = "About";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "關於";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)svgImageBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem1;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem2;
        private DevExpress.XtraEditors.SvgImageBox svgImageBox1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}