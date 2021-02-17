
namespace PW_MusicManager
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SongDataGrid = new System.Windows.Forms.DataGridView();
            this.ImportButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SongDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // SongDataGrid
            // 
            this.SongDataGrid.AllowUserToAddRows = false;
            this.SongDataGrid.AllowUserToDeleteRows = false;
            this.SongDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SongDataGrid.Location = new System.Drawing.Point(12, 12);
            this.SongDataGrid.Name = "SongDataGrid";
            this.SongDataGrid.RowTemplate.Height = 25;
            this.SongDataGrid.Size = new System.Drawing.Size(544, 581);
            this.SongDataGrid.TabIndex = 0;
            this.SongDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.SongDataGrid_CellValueChanged);
            this.SongDataGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.SongDataGrid_CurrentCellDirtyStateChanged);
            this.SongDataGrid.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.SongDataGrid_DefaultValuesNeeded);
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(562, 12);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(84, 51);
            this.ImportButton.TabIndex = 1;
            this.ImportButton.Text = "Import WEMs";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 605);
            this.Controls.Add(this.ImportButton);
            this.Controls.Add(this.SongDataGrid);
            this.Name = "MainWindow";
            this.Text = "PW-MusicManager";
            ((System.ComponentModel.ISupportInitialize)(this.SongDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView SongDataGrid;
        private System.Windows.Forms.Button ImportButton;
    }
}

