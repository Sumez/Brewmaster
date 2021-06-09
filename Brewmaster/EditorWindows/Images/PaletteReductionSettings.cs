using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Pipeline;

namespace Brewmaster.EditorWindows.Images
{
	public class PaletteReductionSettings : UserControl
	{
		private Panel ChrPipelinePanel;
		private GroupBox PaletteGroup;
		private readonly ImageRenderControl _image;
		private bool _loading;
		private List<PaletteEntries> PaletteEntries;
		private CheckBox _enablePaletteReduction;
		private ContextMenuStrip contextMenuStrip1;
		private System.ComponentModel.IContainer components;
		private FlowLayoutPanel PaletteContainer;
		private List<List<Color>> _palettes;

		public Pipeline.ChrPipelineSettings Pipeline { get; set; }
		public event Action PipelineChanged;

		public PaletteReductionSettings(Pipeline.ChrPipelineSettings pipeline, ImageRenderControl image)
		{
			_loading = true;
			Pipeline = pipeline;
			_image = image;
			InitializeComponent();
			RefreshLayout();
			_loading = false;
		}

		public void RefreshLayout()
		{
			//BorderStyle = BorderStyle.Fixed3D;

			var chrPipeline = Pipeline;
			if (chrPipeline == null) return;

			_enablePaletteReduction.Checked = chrPipeline.ReducePalette;
			//RefreshPalette();
		}

		private void SetPaletteAssignments()
		{
			var chrPipeline = Pipeline;
			if (chrPipeline == null) return;

			// TODO: Detect changes

			SuspendLayout();
			foreach (Control control in PaletteContainer.Controls) control.Dispose(); // Necessary?
			PaletteContainer.Controls.Clear();
			PaletteEntries = new List<PaletteEntries>();

			if (chrPipeline.ReducePalette)
			{
				if (chrPipeline.TilePalettes == null) AssignDefaultPalettes();
				foreach (var palette in chrPipeline.TilePalettes)
				{
					var entryEditor = new PaletteEntries { EnableEdit = true, Height = 30, Width = PaletteContainer.MaximumSize.Width };
					PaletteContainer.Controls.Add(entryEditor);
					PaletteEntries.Add(entryEditor);
				}
			}
			ResumeLayout(true);
			RegisterChange();
		}

		private void AssignDefaultPalettes()
		{
			_palettes = ChrPipeline.GetUniqueTilePalettes(_image.ImageSource);
			Pipeline.TilePalettes = new List<Dictionary<Color, int>>();

			foreach (var palette in _palettes)
			{
				var assignment = new Dictionary<Color, int>();
				for (var i = 0; i < palette.Count; i++)
				{
					assignment[palette[i]] = i;
				}

				Pipeline.TilePalettes.Add(assignment);
			}
		}

		private void RefreshPalette()
		{
			UpdatePreview();
			var chrPipeline = Pipeline;
			if (chrPipeline == null || chrPipeline.TilePalettes == null) return;

			var paletteColors = (int)Math.Pow(2, chrPipeline.BitDepth);

			var paletteIndex = 0;
			foreach (var tilePalette in chrPipeline.TilePalettes)
			{
				var palette = new List<Color?>(new Color?[tilePalette.Count == 0 ? 0 : tilePalette.Values.Max() + 1]);
				foreach (var assignment in tilePalette)
				{
					palette[assignment.Value] = assignment.Key;
				}

				/*foreach (var color in _paletteSource.OrderBy(c => c.A))
				{
					if (!palette.Contains(color)) palette.Add(color);
				}*/

				while (paletteColors > palette.Count) palette.Add(null);
				PaletteEntries[paletteIndex].SetEntries(palette, paletteColors);
				PaletteEntries[paletteIndex].OrderUpdated = () =>
				{
					for (var i = 0; i < palette.Count; i++)
					{
						if (palette[i].HasValue) tilePalette[palette[i].Value] = i;
					}
					RegisterChange();
					UpdatePreview();
				};

				paletteIndex++;
			}
		}

		private void UpdatePreview()
		{
			if (Pipeline == null || Pipeline.TilePalettes == null) { _image.SetPreviews(); }
			else _image.SetPreviews(ChrPipeline.GetReducedImage(Pipeline, _image.ImageSource));
		}


		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.ChrPipelinePanel = new System.Windows.Forms.Panel();
			this.PaletteGroup = new System.Windows.Forms.GroupBox();
			this.PaletteContainer = new System.Windows.Forms.FlowLayoutPanel();
			this._enablePaletteReduction = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ChrPipelinePanel.SuspendLayout();
			this.PaletteGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChrPipelinePanel
			// 
			this.ChrPipelinePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ChrPipelinePanel.AutoSize = true;
			this.ChrPipelinePanel.Controls.Add(this.PaletteGroup);
			this.ChrPipelinePanel.Location = new System.Drawing.Point(0, 0);
			this.ChrPipelinePanel.Name = "ChrPipelinePanel";
			this.ChrPipelinePanel.Size = new System.Drawing.Size(309, 65);
			this.ChrPipelinePanel.TabIndex = 3;
			// 
			// PaletteGroup
			// 
			this.PaletteGroup.AutoSize = true;
			this.PaletteGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.PaletteGroup.Controls.Add(this.PaletteContainer);
			this.PaletteGroup.Controls.Add(this._enablePaletteReduction);
			this.PaletteGroup.Dock = System.Windows.Forms.DockStyle.Top;
			this.PaletteGroup.Location = new System.Drawing.Point(0, 0);
			this.PaletteGroup.Name = "PaletteGroup";
			this.PaletteGroup.Size = new System.Drawing.Size(309, 59);
			this.PaletteGroup.TabIndex = 5;
			this.PaletteGroup.TabStop = false;
			this.PaletteGroup.Text = "Reduce palette per tile";
			// 
			// PaletteContainer
			// 
			this.PaletteContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PaletteContainer.AutoSize = true;
			this.PaletteContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.PaletteContainer.Location = new System.Drawing.Point(7, 40);
			this.PaletteContainer.MaximumSize = new System.Drawing.Size(287, 0);
			this.PaletteContainer.Name = "PaletteContainer";
			this.PaletteContainer.Size = new System.Drawing.Size(287, 0);
			this.PaletteContainer.TabIndex = 1;
			// 
			// _enablePaletteReduction
			// 
			this._enablePaletteReduction.AutoSize = true;
			this._enablePaletteReduction.Location = new System.Drawing.Point(7, 20);
			this._enablePaletteReduction.Name = "_enablePaletteReduction";
			this._enablePaletteReduction.Size = new System.Drawing.Size(141, 17);
			this._enablePaletteReduction.TabIndex = 0;
			this._enablePaletteReduction.Text = "Enable palette reduction";
			this._enablePaletteReduction.UseVisualStyleBackColor = true;
			this._enablePaletteReduction.CheckedChanged += new System.EventHandler(this._enablePaletteReduction_CheckedChanged);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// PaletteReductionSettings
			// 
			this.AutoSize = true;
			this.Controls.Add(this.ChrPipelinePanel);
			this.MinimumSize = new System.Drawing.Size(309, 0);
			this.Name = "PaletteReductionSettings";
			this.Size = new System.Drawing.Size(312, 68);
			this.ChrPipelinePanel.ResumeLayout(false);
			this.ChrPipelinePanel.PerformLayout();
			this.PaletteGroup.ResumeLayout(false);
			this.PaletteGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void _enablePaletteReduction_CheckedChanged(object sender, EventArgs e)
		{
			Pipeline.ReducePalette = _enablePaletteReduction.Checked;
			if (!_enablePaletteReduction.Checked)
			{
				//Height = 30;
				Pipeline.TilePalettes = null;
			}
			else
			{
				//Height = 300;
			}
			SetPaletteAssignments();
			RegisterChange();
			RefreshPalette();

			if (Parent != null) Parent.PerformLayout(); // Cheap workaround, but seems to be the easiest way to handle resized components
		}

		private void RegisterChange()
		{
			if (!_loading && PipelineChanged != null) PipelineChanged();
		}
	}
}
