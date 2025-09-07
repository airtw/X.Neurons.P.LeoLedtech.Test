namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    partial class ServerConfig
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
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            btn_Save = new DevExpress.XtraEditors.SimpleButton();
            serverIP_value = new DevExpress.XtraEditors.TextEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
            emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)serverIP_value.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem4).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(btn_Save);
            layoutControl1.Controls.Add(serverIP_value);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(936, 259, 1106, 723);
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(602, 372);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // btn_Save
            // 
            btn_Save.Appearance.Font = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_Save.Appearance.Options.UseFont = true;
            btn_Save.AppearanceDisabled.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            btn_Save.AppearanceDisabled.Options.UseFont = true;
            btn_Save.AppearanceHovered.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            btn_Save.AppearanceHovered.Options.UseFont = true;
            btn_Save.AppearancePressed.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            btn_Save.AppearancePressed.Options.UseFont = true;
            btn_Save.Location = new Point(12, 218);
            btn_Save.Name = "btn_Save";
            btn_Save.Size = new Size(578, 26);
            btn_Save.StyleController = layoutControl1;
            btn_Save.TabIndex = 5;
            btn_Save.Text = "保存";
            btn_Save.Click += btn_Save_Click;
            // 
            // serverIP_value
            // 
            serverIP_value.Location = new Point(184, 164);
            serverIP_value.Name = "serverIP_value";
            serverIP_value.Properties.Appearance.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            serverIP_value.Properties.Appearance.Options.UseFont = true;
            serverIP_value.Size = new Size(406, 28);
            serverIP_value.StyleController = layoutControl1;
            serverIP_value.TabIndex = 4;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, emptySpaceItem1, emptySpaceItem2, emptySpaceItem3, simpleLabelItem1, emptySpaceItem4 });
            Root.Name = "Root";
            Root.Size = new Size(602, 372);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.AppearanceItemCaption.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            layoutControlItem1.AppearanceItemCaptionDisabled.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            layoutControlItem1.AppearanceItemCaptionDisabled.Options.UseFont = true;
            layoutControlItem1.Control = serverIP_value;
            layoutControlItem1.Location = new Point(0, 152);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(582, 32);
            layoutControlItem1.Text = "伺服器IP";
            layoutControlItem1.TextSize = new Size(160, 21);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = btn_Save;
            layoutControlItem2.Location = new Point(0, 206);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(582, 30);
            layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(0, 0);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(582, 78);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(0, 236);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(582, 116);
            // 
            // emptySpaceItem3
            // 
            emptySpaceItem3.Location = new Point(0, 184);
            emptySpaceItem3.Name = "emptySpaceItem3";
            emptySpaceItem3.Size = new Size(582, 22);
            // 
            // simpleLabelItem1
            // 
            simpleLabelItem1.AppearanceItemCaption.Font = new Font("微軟正黑體", 24F, FontStyle.Bold, GraphicsUnit.Point, 136);
            simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
            simpleLabelItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            simpleLabelItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            simpleLabelItem1.Location = new Point(0, 78);
            simpleLabelItem1.Name = "simpleLabelItem1";
            simpleLabelItem1.Size = new Size(582, 44);
            simpleLabelItem1.Text = "伺服器設定";
            simpleLabelItem1.TextSize = new Size(160, 40);
            // 
            // emptySpaceItem4
            // 
            emptySpaceItem4.Location = new Point(0, 122);
            emptySpaceItem4.Name = "emptySpaceItem4";
            emptySpaceItem4.Size = new Size(582, 30);
            // 
            // ServerConfig
            // 
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(6F, 12F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(602, 372);
            Controls.Add(layoutControl1);
            Font = new Font("新細明體", 9F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "ServerConfig";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "伺服器設定";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)serverIP_value.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)simpleLabelItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem4).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit serverIP_value;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
    }
}