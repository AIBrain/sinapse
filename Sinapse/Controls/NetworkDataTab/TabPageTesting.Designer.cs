namespace Sinapse.Controls.NetworkDataTab
{
    partial class TabPageTesting
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnRound = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Image = global::Sinapse.Properties.Resources.switchuser;
            this.btnCompare.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCompare.Location = new System.Drawing.Point(429, 372);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(80, 23);
            this.btnCompare.TabIndex = 29;
            this.btnCompare.Text = "Compare";
            this.btnCompare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Image = global::Sinapse.Properties.Resources.gear_22;
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnQuery.Location = new System.Drawing.Point(340, 372);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(83, 23);
            this.btnQuery.TabIndex = 30;
            this.btnQuery.Text = "Compute!";
            this.btnQuery.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnRound
            // 
            this.btnRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRound.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRound.Image = global::Sinapse.Properties.Resources.precminus;
            this.btnRound.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRound.Location = new System.Drawing.Point(498, 5);
            this.btnRound.Name = "btnRound";
            this.btnRound.Size = new System.Drawing.Size(87, 23);
            this.btnRound.TabIndex = 30;
            this.btnRound.Text = "Round results";
            this.btnRound.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRound.UseVisualStyleBackColor = true;
            this.btnRound.Click += new System.EventHandler(this.btnRound_Click);
            // 
            // TabPageTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnRound);
            this.Controls.Add(this.btnQuery);
            this.Name = "TabPageTesting";
            this.Controls.SetChildIndex(this.lbSetTitle, 0);
            this.Controls.SetChildIndex(this.btnQuery, 0);
            this.Controls.SetChildIndex(this.btnRound, 0);
            this.Controls.SetChildIndex(this.btnCompare, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnRound;
    }
}
