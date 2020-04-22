using System.Windows.Forms;

namespace BrewMaster.Modules.NumberHelper
{
	partial class NumberHelper
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.DecimalNumber = new System.Windows.Forms.TextBox();
			this.HexNumber = new System.Windows.Forms.TextBox();
			this.BinaryNumber = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(36, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "DEC:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "HEX:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(3, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "BIN:";
			// 
			// DecimalNumber
			// 
			this.DecimalNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.DecimalNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DecimalNumber.Location = new System.Drawing.Point(53, 0);
			this.DecimalNumber.Name = "DecimalNumber";
			this.DecimalNumber.Size = new System.Drawing.Size(100, 13);
			this.DecimalNumber.TabIndex = 3;
			this.DecimalNumber.Text = "0";
			// 
			// HexNumber
			// 
			this.HexNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.HexNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HexNumber.Location = new System.Drawing.Point(53, 16);
			this.HexNumber.Name = "HexNumber";
			this.HexNumber.Size = new System.Drawing.Size(100, 13);
			this.HexNumber.TabIndex = 4;
			this.HexNumber.Text = "00";
			// 
			// BinaryNumber
			// 
			this.BinaryNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.BinaryNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BinaryNumber.Location = new System.Drawing.Point(53, 32);
			this.BinaryNumber.Name = "BinaryNumber";
			this.BinaryNumber.Size = new System.Drawing.Size(100, 13);
			this.BinaryNumber.TabIndex = 5;
			this.BinaryNumber.Text = "00000000";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
			this.label4.Location = new System.Drawing.Point(42, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(13, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "$";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.SystemColors.WindowFrame;
			this.label5.Location = new System.Drawing.Point(40, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(15, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "%";
			// 
			// NumberHelper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.BinaryNumber);
			this.Controls.Add(this.HexNumber);
			this.Controls.Add(this.DecimalNumber);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.ForeColor = System.Drawing.SystemColors.WindowText;
			this.Name = "NumberHelper";
			this.Size = new System.Drawing.Size(592, 309);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Label label1;
		private Label label2;
		private Label label3;
		private TextBox DecimalNumber;
		private TextBox HexNumber;
		private TextBox BinaryNumber;
		private Label label4;
		private Label label5;
	}
}
