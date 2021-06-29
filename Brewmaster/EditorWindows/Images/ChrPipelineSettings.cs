using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Pipeline;

namespace Brewmaster.EditorWindows.Images
{
	public class ChrPipelineSettings : UserControl
	{
		private ComboBox comboBox1;
		private ComboBox ChrPipelineOutput;
		private TextBox OutputFile;
		private Panel ChrPipelinePanel;
		private GroupBox PaletteGroup;
		private bool _loading;
		private PaletteEntries PaletteEntries;
		private CheckBox _enablePaletteAssignments;
		private readonly IEnumerable<Color> _paletteSource;
		private CheckBox _exportPalette;
		private TextBox PaletteOutputFile;
		private CheckBox _ignoreDuplicates;
		private TextBox rgbValue;
		private ContextMenuStrip contextMenuStrip1;
		private System.ComponentModel.IContainer components;
		private CheckBox _bigTiles;
		private List<Color?> _palette;

		public Pipeline.ChrPipelineSettings Pipeline { get; set; }
		public event Action PipelineChanged;

		public ChrPipelineSettings(Pipeline.ChrPipelineSettings pipeline, ImageRenderControl image)
		{
			_loading = true;
			Pipeline = pipeline;
			_paletteSource = image.Palette;
			InitializeComponent();
			RefreshLayout();
			_loading = false;

			PaletteEntries.OrderUpdated += () => { SetPaletteAssignments(); };
			PaletteEntries.MouseOverColor += (colors) => { PreviewPaletteColor(colors); };
			image.PaletteChanged += () =>
			{
				_loading = true;
				RefreshLayout();
				_loading = false;
			};
		}

		private void PreviewPaletteColor(Color?[] colors)
		{
			if (colors.Length == 0)
			{
				rgbValue.Text = "";
				return;
			}

			rgbValue.Text = string.Join(", ", colors.Select(GetColorHex));

		}

		private static string GetColorHex(Color? color)
		{
			if (!color.HasValue) return "0";
			var red = color.Value.R / 8;
			var green = color.Value.G / 8;
			var blue = color.Value.B / 8;

			return "$" + Convert.ToString(red | (green << 5) | (blue << 10), 16).ToUpper().PadLeft(4, '0');
		}

		public void RefreshLayout()
		{
			//BorderStyle = BorderStyle.Fixed3D;

			if (Pipeline == null) return;

			OutputFile.Text = Pipeline.ChrOutput;
			PaletteOutputFile.Text = Pipeline.PaletteOutput;
			ChrPipelineOutput.SelectedIndex = (int)Pipeline.PaletteType;
			_enablePaletteAssignments.Checked = Pipeline.PaletteAssignment.Count > 0;
			_ignoreDuplicates.Checked = Pipeline.DiscardRedundantTiles;
			_bigTiles.Checked = Pipeline.BigTiles;
			PaletteOutputFile.Enabled = _exportPalette.Checked = Pipeline.ExportPalette;
			RefreshPalette();
		}

		private void SetPaletteAssignments()
		{
			for (var i = 0; i < _palette.Count; i++)
			{
				if (_palette[i].HasValue) Pipeline.PaletteAssignment[_palette[i].Value] = i;
			}
			RegisterChange();
		}

		private void RefreshPalette()
		{
			if (Pipeline == null) return;
			
			var paletteColors = (int)Math.Pow(2, Pipeline.BitDepth);

			_palette = new List<Color?>(new Color?[Pipeline.PaletteAssignment.Count == 0 ? 0 : Pipeline.PaletteAssignment.Values.Max() + 1]);
			foreach (var assignment in Pipeline.PaletteAssignment)
			{
				_palette[assignment.Value] = assignment.Key;
			}

			foreach (var color in _paletteSource.OrderBy(c => c.A))
			{
				if (!_palette.Contains(color)) _palette.Add(color);
			}

			while (paletteColors > _palette.Count) _palette.Add(null);
			PaletteEntries.SetEntries(_palette, paletteColors);
		}


		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox ExportPaletteGroup;
			System.Windows.Forms.GroupBox ChrGroup;
			System.Windows.Forms.Label BitDepthLabel;
			this.PaletteOutputFile = new System.Windows.Forms.TextBox();
			this._exportPalette = new System.Windows.Forms.CheckBox();
			this._bigTiles = new System.Windows.Forms.CheckBox();
			this._ignoreDuplicates = new System.Windows.Forms.CheckBox();
			this.ChrPipelineOutput = new System.Windows.Forms.ComboBox();
			this.OutputFile = new System.Windows.Forms.TextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.ChrPipelinePanel = new System.Windows.Forms.Panel();
			this.PaletteGroup = new System.Windows.Forms.GroupBox();
			this.rgbValue = new System.Windows.Forms.TextBox();
			this.PaletteEntries = new Brewmaster.EditorWindows.Images.PaletteEntries();
			this._enablePaletteAssignments = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			ExportPaletteGroup = new System.Windows.Forms.GroupBox();
			ChrGroup = new System.Windows.Forms.GroupBox();
			BitDepthLabel = new System.Windows.Forms.Label();
			ExportPaletteGroup.SuspendLayout();
			ChrGroup.SuspendLayout();
			this.ChrPipelinePanel.SuspendLayout();
			this.PaletteGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// ExportPaletteGroup
			// 
			ExportPaletteGroup.Controls.Add(this.PaletteOutputFile);
			ExportPaletteGroup.Controls.Add(this._exportPalette);
			ExportPaletteGroup.Dock = System.Windows.Forms.DockStyle.Top;
			ExportPaletteGroup.Location = new System.Drawing.Point(0, 253);
			ExportPaletteGroup.Name = "ExportPaletteGroup";
			ExportPaletteGroup.Size = new System.Drawing.Size(342, 42);
			ExportPaletteGroup.TabIndex = 5;
			ExportPaletteGroup.TabStop = false;
			ExportPaletteGroup.Text = "Export palette";
			// 
			// PaletteOutputFile
			// 
			this.PaletteOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PaletteOutputFile.Location = new System.Drawing.Point(28, 16);
			this.PaletteOutputFile.Name = "PaletteOutputFile";
			this.PaletteOutputFile.Size = new System.Drawing.Size(308, 20);
			this.PaletteOutputFile.TabIndex = 1;
			this.PaletteOutputFile.TextChanged += new System.EventHandler(this.PaletteOutputFile_TextChanged);
			// 
			// _exportPalette
			// 
			this._exportPalette.AutoSize = true;
			this._exportPalette.Location = new System.Drawing.Point(7, 19);
			this._exportPalette.Name = "_exportPalette";
			this._exportPalette.Size = new System.Drawing.Size(15, 14);
			this._exportPalette.TabIndex = 0;
			this._exportPalette.UseVisualStyleBackColor = true;
			this._exportPalette.CheckedChanged += new System.EventHandler(this._exportPalette_CheckedChanged);
			// 
			// ChrGroup
			// 
			ChrGroup.Controls.Add(BitDepthLabel);
			ChrGroup.Controls.Add(this._bigTiles);
			ChrGroup.Controls.Add(this._ignoreDuplicates);
			ChrGroup.Controls.Add(this.ChrPipelineOutput);
			ChrGroup.Controls.Add(this.OutputFile);
			ChrGroup.Dock = System.Windows.Forms.DockStyle.Top;
			ChrGroup.Location = new System.Drawing.Point(0, 0);
			ChrGroup.Name = "ChrGroup";
			ChrGroup.Size = new System.Drawing.Size(342, 120);
			ChrGroup.TabIndex = 6;
			ChrGroup.TabStop = false;
			ChrGroup.Text = "Tile data (CHR)";
			// 
			// BitDepthLabel
			// 
			BitDepthLabel.AutoSize = true;
			BitDepthLabel.Location = new System.Drawing.Point(6, 48);
			BitDepthLabel.Name = "BitDepthLabel";
			BitDepthLabel.Size = new System.Drawing.Size(52, 13);
			BitDepthLabel.TabIndex = 4;
			BitDepthLabel.Text = "Bit depth:";
			// 
			// _bigTiles
			// 
			this._bigTiles.AutoSize = true;
			this._bigTiles.Location = new System.Drawing.Point(7, 96);
			this._bigTiles.Name = "_bigTiles";
			this._bigTiles.Size = new System.Drawing.Size(156, 17);
			this._bigTiles.TabIndex = 5;
			this._bigTiles.Text = "Store as 16x16 tiles (SNES)";
			this._bigTiles.UseVisualStyleBackColor = true;
			this._bigTiles.CheckedChanged += new System.EventHandler(this._bigTiles_CheckedChanged);
			// 
			// _ignoreDuplicates
			// 
			this._ignoreDuplicates.AutoSize = true;
			this._ignoreDuplicates.Location = new System.Drawing.Point(7, 73);
			this._ignoreDuplicates.Name = "_ignoreDuplicates";
			this._ignoreDuplicates.Size = new System.Drawing.Size(107, 17);
			this._ignoreDuplicates.TabIndex = 3;
			this._ignoreDuplicates.Text = "Ignore duplicates";
			this._ignoreDuplicates.UseVisualStyleBackColor = true;
			this._ignoreDuplicates.CheckedChanged += new System.EventHandler(this._ignoreDuplicates_CheckedChanged);
			// 
			// ChrPipelineOutput
			// 
			this.ChrPipelineOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ChrPipelineOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ChrPipelineOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ChrPipelineOutput.FormattingEnabled = true;
			this.ChrPipelineOutput.Items.AddRange(new object[] {
            "2bpp",
            "3bpp",
            "4bpp",
            "8bpp"});
			this.ChrPipelineOutput.Location = new System.Drawing.Point(78, 45);
			this.ChrPipelineOutput.MinimumSize = new System.Drawing.Size(100, 0);
			this.ChrPipelineOutput.Name = "ChrPipelineOutput";
			this.ChrPipelineOutput.Size = new System.Drawing.Size(258, 21);
			this.ChrPipelineOutput.TabIndex = 1;
			this.ChrPipelineOutput.SelectedIndexChanged += new System.EventHandler(this.ChrPipelineOutput_SelectedIndexChanged);
			// 
			// OutputFile
			// 
			this.OutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputFile.Location = new System.Drawing.Point(7, 19);
			this.OutputFile.MinimumSize = new System.Drawing.Size(150, 0);
			this.OutputFile.Name = "OutputFile";
			this.OutputFile.Size = new System.Drawing.Size(329, 20);
			this.OutputFile.TabIndex = 2;
			this.OutputFile.TextChanged += new System.EventHandler(this.OutputFile_TextChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(0, 0);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 0;
			// 
			// ChrPipelinePanel
			// 
			this.ChrPipelinePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ChrPipelinePanel.AutoSize = true;
			this.ChrPipelinePanel.Controls.Add(ExportPaletteGroup);
			this.ChrPipelinePanel.Controls.Add(this.PaletteGroup);
			this.ChrPipelinePanel.Controls.Add(ChrGroup);
			this.ChrPipelinePanel.Location = new System.Drawing.Point(0, 0);
			this.ChrPipelinePanel.Name = "ChrPipelinePanel";
			this.ChrPipelinePanel.Size = new System.Drawing.Size(342, 295);
			this.ChrPipelinePanel.TabIndex = 3;
			// 
			// PaletteGroup
			// 
			this.PaletteGroup.AutoSize = true;
			this.PaletteGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.PaletteGroup.Controls.Add(this.rgbValue);
			this.PaletteGroup.Controls.Add(this.PaletteEntries);
			this.PaletteGroup.Controls.Add(this._enablePaletteAssignments);
			this.PaletteGroup.Dock = System.Windows.Forms.DockStyle.Top;
			this.PaletteGroup.Location = new System.Drawing.Point(0, 120);
			this.PaletteGroup.MinimumSize = new System.Drawing.Size(300, 0);
			this.PaletteGroup.Name = "PaletteGroup";
			this.PaletteGroup.Size = new System.Drawing.Size(342, 133);
			this.PaletteGroup.TabIndex = 5;
			this.PaletteGroup.TabStop = false;
			this.PaletteGroup.Text = "Palette assignments";
			this.PaletteGroup.Enter += new System.EventHandler(this.PaletteGroup_Enter);
			// 
			// rgbValue
			// 
			this.rgbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rgbValue.Location = new System.Drawing.Point(9, 94);
			this.rgbValue.Name = "rgbValue";
			this.rgbValue.Size = new System.Drawing.Size(327, 20);
			this.rgbValue.TabIndex = 2;
			this.rgbValue.Visible = false;
			// 
			// PaletteEntries
			// 
			this.PaletteEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PaletteEntries.AutoSize = true;
			this.PaletteEntries.EnableEdit = false;
			this.PaletteEntries.Location = new System.Drawing.Point(7, 43);
			this.PaletteEntries.Name = "PaletteEntries";
			this.PaletteEntries.Size = new System.Drawing.Size(329, 26);
			this.PaletteEntries.TabIndex = 1;
			// 
			// _enablePaletteAssignments
			// 
			this._enablePaletteAssignments.AutoSize = true;
			this._enablePaletteAssignments.Location = new System.Drawing.Point(7, 20);
			this._enablePaletteAssignments.Name = "_enablePaletteAssignments";
			this._enablePaletteAssignments.Size = new System.Drawing.Size(160, 17);
			this._enablePaletteAssignments.TabIndex = 0;
			this._enablePaletteAssignments.Text = "Use manual (drag to reorder)";
			this._enablePaletteAssignments.UseVisualStyleBackColor = true;
			this._enablePaletteAssignments.CheckedChanged += new System.EventHandler(this._enablePaletteAssignments_CheckedChanged);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// ChrPipelineSettings
			// 
			this.AutoSize = true;
			this.Controls.Add(this.ChrPipelinePanel);
			this.Name = "ChrPipelineSettings";
			this.Size = new System.Drawing.Size(345, 298);
			ExportPaletteGroup.ResumeLayout(false);
			ExportPaletteGroup.PerformLayout();
			ChrGroup.ResumeLayout(false);
			ChrGroup.PerformLayout();
			this.ChrPipelinePanel.ResumeLayout(false);
			this.ChrPipelinePanel.PerformLayout();
			this.PaletteGroup.ResumeLayout(false);
			this.PaletteGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void ChrPipelineOutput_SelectedIndexChanged(object sender, EventArgs e)
		{
			Pipeline.PaletteType = (ChrOutputType)ChrPipelineOutput.SelectedIndex;
			RegisterChange();
			RefreshPalette();
		}

		private void OutputFile_TextChanged(object sender, EventArgs e)
		{
			if (Pipeline != null) Pipeline.ChrOutput = OutputFile.Text;

			RegisterChange();
		}

		private void _enablePaletteAssignments_CheckedChanged(object sender, EventArgs e)
		{
			PaletteEntries.EnableEdit = _enablePaletteAssignments.Checked;
			if (!_enablePaletteAssignments.Checked)
			{
				Pipeline.PaletteAssignment.Clear();
			}
			else
			{
				SetPaletteAssignments();
			}

			RegisterChange();
			RefreshPalette();
		}

		private void RegisterChange()
		{
			if (!_loading && PipelineChanged != null) PipelineChanged();
		}

		private void _ignoreDuplicates_CheckedChanged(object sender, EventArgs e)
		{
			Pipeline.DiscardRedundantTiles = _ignoreDuplicates.Checked;
			RegisterChange();
		}

		private void PaletteGroup_Enter(object sender, EventArgs e)
		{

		}

		private void _exportPalette_CheckedChanged(object sender, EventArgs e)
		{
			PaletteOutputFile.Enabled = Pipeline.ExportPalette = _exportPalette.Checked;
			RegisterChange();
		}

		private void PaletteOutputFile_TextChanged(object sender, EventArgs e)
		{
			Pipeline.PaletteOutput = PaletteOutputFile.Text;
			RegisterChange();
		}

		private void _bigTiles_CheckedChanged(object sender, EventArgs e)
		{
			_ignoreDuplicates.Enabled = !(Pipeline.BigTiles = _bigTiles.Checked);
			RegisterChange();
		}
	}
}
