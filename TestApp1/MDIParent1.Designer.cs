namespace TestApp1
{
    partial class MDIParent1
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.borderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borderWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borderRadiusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borderOpacityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activeBorederColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inactiveBorederColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captionHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.desktopBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageLayoutTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageLayoutCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageLayoutStretchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageLayoutZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newChildFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutMdiCascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutMdiVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutMdiHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 707);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Status";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.borderToolStripMenuItem,
            this.desktopBackgroundToolStripMenuItem,
            this.formsToolStripMenuItem,
            this.directionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // borderToolStripMenuItem
            // 
            this.borderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.borderWidthToolStripMenuItem,
            this.borderRadiusToolStripMenuItem,
            this.borderOpacityToolStripMenuItem,
            this.activeBorederColorToolStripMenuItem,
            this.inactiveBorederColorToolStripMenuItem,
            this.captionHeightToolStripMenuItem});
            this.borderToolStripMenuItem.Name = "borderToolStripMenuItem";
            this.borderToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.borderToolStripMenuItem.Text = "Border";
            // 
            // borderWidthToolStripMenuItem
            // 
            this.borderWidthToolStripMenuItem.Name = "borderWidthToolStripMenuItem";
            this.borderWidthToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.borderWidthToolStripMenuItem.Text = "Border Width";
            this.borderWidthToolStripMenuItem.Click += new System.EventHandler(this.borderWidthToolStripMenuItem_Click);
            // 
            // borderRadiusToolStripMenuItem
            // 
            this.borderRadiusToolStripMenuItem.Name = "borderRadiusToolStripMenuItem";
            this.borderRadiusToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.borderRadiusToolStripMenuItem.Text = "Border Radius";
            this.borderRadiusToolStripMenuItem.Click += new System.EventHandler(this.borderRadiusToolStripMenuItem_Click);
            // 
            // borderOpacityToolStripMenuItem
            // 
            this.borderOpacityToolStripMenuItem.Name = "borderOpacityToolStripMenuItem";
            this.borderOpacityToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.borderOpacityToolStripMenuItem.Text = "Border Opacity";
            this.borderOpacityToolStripMenuItem.Click += new System.EventHandler(this.borderOpacityToolStripMenuItem_Click);
            // 
            // activeBorederColorToolStripMenuItem
            // 
            this.activeBorederColorToolStripMenuItem.Name = "activeBorederColorToolStripMenuItem";
            this.activeBorederColorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.activeBorederColorToolStripMenuItem.Text = "Active Border Color";
            this.activeBorederColorToolStripMenuItem.Click += new System.EventHandler(this.activeBorederColorToolStripMenuItem_Click);
            // 
            // inactiveBorederColorToolStripMenuItem
            // 
            this.inactiveBorederColorToolStripMenuItem.Name = "inactiveBorederColorToolStripMenuItem";
            this.inactiveBorederColorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.inactiveBorederColorToolStripMenuItem.Text = "Inactive Border Color";
            this.inactiveBorederColorToolStripMenuItem.Click += new System.EventHandler(this.inactiveBorederColorToolStripMenuItem_Click);
            // 
            // captionHeightToolStripMenuItem
            // 
            this.captionHeightToolStripMenuItem.Name = "captionHeightToolStripMenuItem";
            this.captionHeightToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.captionHeightToolStripMenuItem.Text = "Caption Height";
            this.captionHeightToolStripMenuItem.Click += new System.EventHandler(this.CaptionHieghtToolStripMenuItem_Click);
            // 
            // desktopBackgroundToolStripMenuItem
            // 
            this.desktopBackgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageLayoutTileToolStripMenuItem,
            this.imageLayoutCenterToolStripMenuItem,
            this.imageLayoutStretchToolStripMenuItem,
            this.imageLayoutZoomToolStripMenuItem,
            this.removeBackgroundToolStripMenuItem});
            this.desktopBackgroundToolStripMenuItem.Name = "desktopBackgroundToolStripMenuItem";
            this.desktopBackgroundToolStripMenuItem.Size = new System.Drawing.Size(129, 20);
            this.desktopBackgroundToolStripMenuItem.Text = "Desktop Background";
            // 
            // imageLayoutTileToolStripMenuItem
            // 
            this.imageLayoutTileToolStripMenuItem.Name = "imageLayoutTileToolStripMenuItem";
            this.imageLayoutTileToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.imageLayoutTileToolStripMenuItem.Text = "Image Layout Tile";
            this.imageLayoutTileToolStripMenuItem.Click += new System.EventHandler(this.imageLayoutTileToolStripMenuItem_Click);
            // 
            // imageLayoutCenterToolStripMenuItem
            // 
            this.imageLayoutCenterToolStripMenuItem.Name = "imageLayoutCenterToolStripMenuItem";
            this.imageLayoutCenterToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.imageLayoutCenterToolStripMenuItem.Text = "Image Layout Center";
            this.imageLayoutCenterToolStripMenuItem.Click += new System.EventHandler(this.imageLayoutCenterToolStripMenuItem_Click);
            // 
            // imageLayoutStretchToolStripMenuItem
            // 
            this.imageLayoutStretchToolStripMenuItem.Name = "imageLayoutStretchToolStripMenuItem";
            this.imageLayoutStretchToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.imageLayoutStretchToolStripMenuItem.Text = "Image Layout Stretch";
            this.imageLayoutStretchToolStripMenuItem.Click += new System.EventHandler(this.imageLayoutStretchToolStripMenuItem_Click);
            // 
            // imageLayoutZoomToolStripMenuItem
            // 
            this.imageLayoutZoomToolStripMenuItem.Name = "imageLayoutZoomToolStripMenuItem";
            this.imageLayoutZoomToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.imageLayoutZoomToolStripMenuItem.Text = "Image Layout Zoom";
            this.imageLayoutZoomToolStripMenuItem.Click += new System.EventHandler(this.imageLayoutZoomToolStripMenuItem_Click);
            // 
            // removeBackgroundToolStripMenuItem
            // 
            this.removeBackgroundToolStripMenuItem.Name = "removeBackgroundToolStripMenuItem";
            this.removeBackgroundToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.removeBackgroundToolStripMenuItem.Text = "Remove Background";
            this.removeBackgroundToolStripMenuItem.Click += new System.EventHandler(this.removeBackgroundToolStripMenuItem_Click);
            // 
            // formsToolStripMenuItem
            // 
            this.formsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFormToolStripMenuItem,
            this.newChildFormToolStripMenuItem,
            this.layoutMdiCascadeToolStripMenuItem,
            this.layoutMdiVerticalToolStripMenuItem,
            this.layoutMdiHorizontalToolStripMenuItem});
            this.formsToolStripMenuItem.Name = "formsToolStripMenuItem";
            this.formsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.formsToolStripMenuItem.Text = "Forms";
            // 
            // newFormToolStripMenuItem
            // 
            this.newFormToolStripMenuItem.Name = "newFormToolStripMenuItem";
            this.newFormToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.newFormToolStripMenuItem.Text = "New Form";
            this.newFormToolStripMenuItem.Click += new System.EventHandler(this.newFormToolStripMenuItem_Click);
            // 
            // newChildFormToolStripMenuItem
            // 
            this.newChildFormToolStripMenuItem.Name = "newChildFormToolStripMenuItem";
            this.newChildFormToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.newChildFormToolStripMenuItem.Text = "New Child Form";
            this.newChildFormToolStripMenuItem.Click += new System.EventHandler(this.newChildFormToolStripMenuItem_Click);
            // 
            // layoutMdiCascadeToolStripMenuItem
            // 
            this.layoutMdiCascadeToolStripMenuItem.Name = "layoutMdiCascadeToolStripMenuItem";
            this.layoutMdiCascadeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.layoutMdiCascadeToolStripMenuItem.Text = "Layout Mdi Cascade";
            this.layoutMdiCascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // layoutMdiVerticalToolStripMenuItem
            // 
            this.layoutMdiVerticalToolStripMenuItem.Name = "layoutMdiVerticalToolStripMenuItem";
            this.layoutMdiVerticalToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.layoutMdiVerticalToolStripMenuItem.Text = "Layout Mdi Vertical";
            this.layoutMdiVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // layoutMdiHorizontalToolStripMenuItem
            // 
            this.layoutMdiHorizontalToolStripMenuItem.Name = "layoutMdiHorizontalToolStripMenuItem";
            this.layoutMdiHorizontalToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.layoutMdiHorizontalToolStripMenuItem.Text = "Layout Mdi Horizontal";
            this.layoutMdiHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // directionToolStripMenuItem
            // 
            this.directionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftToRightToolStripMenuItem,
            this.rightToLeftToolStripMenuItem});
            this.directionToolStripMenuItem.Name = "directionToolStripMenuItem";
            this.directionToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.directionToolStripMenuItem.Text = "Direction";
            // 
            // leftToRightToolStripMenuItem
            // 
            this.leftToRightToolStripMenuItem.Name = "leftToRightToolStripMenuItem";
            this.leftToRightToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.leftToRightToolStripMenuItem.Text = "Left to Right";
            this.leftToRightToolStripMenuItem.Click += new System.EventHandler(this.leftToRightToolStripMenuItem_Click);
            // 
            // rightToLeftToolStripMenuItem
            // 
            this.rightToLeftToolStripMenuItem.Name = "rightToLeftToolStripMenuItem";
            this.rightToLeftToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.rightToLeftToolStripMenuItem.Text = "Right to Left";
            this.rightToLeftToolStripMenuItem.Click += new System.EventHandler(this.rightToLeftToolStripMenuItem_Click);
            // 
            // MDIParent1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MDIParent1";
            this.Text = "MDIParent1";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem borderToolStripMenuItem;
        private ToolStripMenuItem borderWidthToolStripMenuItem;
        private ToolStripMenuItem borderRadiusToolStripMenuItem;
        private ToolStripMenuItem borderOpacityToolStripMenuItem;
        private ToolStripMenuItem desktopBackgroundToolStripMenuItem;
        private ToolStripMenuItem formsToolStripMenuItem;
        private ToolStripMenuItem directionToolStripMenuItem;
        private ToolStripMenuItem activeBorederColorToolStripMenuItem;
        private ToolStripMenuItem inactiveBorederColorToolStripMenuItem;
        private ToolStripMenuItem captionHeightToolStripMenuItem;
        private ToolStripMenuItem imageLayoutTileToolStripMenuItem;
        private ToolStripMenuItem imageLayoutCenterToolStripMenuItem;
        private ToolStripMenuItem imageLayoutStretchToolStripMenuItem;
        private ToolStripMenuItem imageLayoutZoomToolStripMenuItem;
        private ToolStripMenuItem removeBackgroundToolStripMenuItem;
        private ToolStripMenuItem newFormToolStripMenuItem;
        private ToolStripMenuItem newChildFormToolStripMenuItem;
        private ToolStripMenuItem layoutMdiCascadeToolStripMenuItem;
        private ToolStripMenuItem layoutMdiVerticalToolStripMenuItem;
        private ToolStripMenuItem layoutMdiHorizontalToolStripMenuItem;
        private ToolStripMenuItem leftToRightToolStripMenuItem;
        private ToolStripMenuItem rightToLeftToolStripMenuItem;
    }
}



