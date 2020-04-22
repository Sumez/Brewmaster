using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrewMaster.Pipeline;
using BrewMaster.ProjectModel;

namespace BrewMaster.EditorWindows.Images
{
	public class ImagePipelineSettings : UserControl
	{
		private ComboBox NewPipelineType;
		private FlowLayoutPanel NoPipelinePanel;
		private readonly AsmProjectFile _file;
		private bool _loading;
		private ContextMenuStrip contextMenuStrip1;
		private System.ComponentModel.IContainer components;

		public ChrPipelineSettings ChrPipelinePanel;
		public PaletteReductionSettings PaletteReductionPanel;
		public event Action PipelineChanged;

		public DataPipeline Pipeline { get; protected set; }

		public ChrPipeline PipelineAsChr
		{
			get { return Pipeline as ChrPipeline; }
		}

		private enum ImagePipelines
		{
			None, Chr, Palette, Custom
		}

		private readonly Dictionary<ImagePipelines, object> _imagePipelineChoices = new Dictionary<ImagePipelines, object>()
		{
			{ ImagePipelines.None, "Add an image data process..."},
			{ ImagePipelines.Chr, "Tileset converter (CHR)" },
			{ ImagePipelines.Palette, "Reduce palette"},
			{ ImagePipelines.Custom, "Custom..."}
		};

		public ImagePipelineSettings(AsmProjectFile file, ImageRenderControl image)
		{
			_loading = true;
			_file = file;
			InitializeComponent();

			NewPipelineType.Items.Clear();
			NewPipelineType.Items.AddRange(_imagePipelineChoices.Select(o => o.Value).ToArray());

			if (file.Pipeline != null) Pipeline = file.Pipeline.Clone(true);
			PaletteReductionPanel = new PaletteReductionSettings(PipelineAsChr, image) { Dock = DockStyle.Top };
			ChrPipelinePanel = new ChrPipelineSettings(PipelineAsChr, image) { Dock = DockStyle.Top };

			PaletteReductionPanel.PipelineChanged += RegisterChange;
			ChrPipelinePanel.PipelineChanged += RegisterChange;

			Controls.Add(ChrPipelinePanel);
			Controls.Add(PaletteReductionPanel);

			RefreshLayout();
			_loading = false;
		}

		private void RefreshLayout()
		{
			//BorderStyle = BorderStyle.Fixed3D;
			NoPipelinePanel.Visible = Pipeline == null;
			var chrPipeline = PipelineAsChr;
			PaletteReductionPanel.Visible = ChrPipelinePanel.Visible = chrPipeline != null;
			if (Pipeline == null)
			{
				NewPipelineType.SelectedIndex = _imagePipelineChoices.Keys.ToList().IndexOf(ImagePipelines.None);
			}
			else
			{
				PaletteReductionPanel.RefreshLayout();
				ChrPipelinePanel.RefreshLayout();
			}

		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Button AddPipelineButton;
			this.NewPipelineType = new System.Windows.Forms.ComboBox();
			this.NoPipelinePanel = new System.Windows.Forms.FlowLayoutPanel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			AddPipelineButton = new System.Windows.Forms.Button();
			this.NoPipelinePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// AddPipelineButton
			// 
			AddPipelineButton.Dock = System.Windows.Forms.DockStyle.Top;
			AddPipelineButton.Location = new System.Drawing.Point(3, 30);
			AddPipelineButton.MinimumSize = new System.Drawing.Size(150, 0);
			AddPipelineButton.Name = "AddPipelineButton";
			AddPipelineButton.Size = new System.Drawing.Size(150, 23);
			AddPipelineButton.TabIndex = 1;
			AddPipelineButton.Text = "Add processing pipeline";
			AddPipelineButton.UseVisualStyleBackColor = true;
			AddPipelineButton.Click += new System.EventHandler(this.AddPipelineButton_Click);
			// 
			// NewPipelineType
			// 
			this.NewPipelineType.Dock = System.Windows.Forms.DockStyle.Top;
			this.NewPipelineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.NewPipelineType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NewPipelineType.FormattingEnabled = true;
			this.NewPipelineType.Items.AddRange(new object[] {
            "Tileset converter (CHR)",
            "Custom..."});
			this.NewPipelineType.Location = new System.Drawing.Point(3, 3);
			this.NewPipelineType.MinimumSize = new System.Drawing.Size(150, 0);
			this.NewPipelineType.Name = "NewPipelineType";
			this.NewPipelineType.Size = new System.Drawing.Size(150, 21);
			this.NewPipelineType.TabIndex = 0;
			// 
			// NoPipelinePanel
			// 
			this.NoPipelinePanel.AutoScroll = true;
			this.NoPipelinePanel.Controls.Add(this.NewPipelineType);
			this.NoPipelinePanel.Controls.Add(AddPipelineButton);
			this.NoPipelinePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.NoPipelinePanel.Location = new System.Drawing.Point(0, 0);
			this.NoPipelinePanel.Name = "NoPipelinePanel";
			this.NoPipelinePanel.Size = new System.Drawing.Size(309, 58);
			this.NoPipelinePanel.TabIndex = 4;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// ImagePipelineSettings
			// 
			this.Controls.Add(this.NoPipelinePanel);
			this.Name = "ImagePipelineSettings";
			this.Size = new System.Drawing.Size(309, 546);
			this.NoPipelinePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private void AddPipelineButton_Click(object sender, EventArgs e)
		{
			var option = _imagePipelineChoices.Keys.ToArray()[NewPipelineType.SelectedIndex];
			switch (option)
			{
				case ImagePipelines.Chr:
					Pipeline = new ChrPipeline(_file, _file.GetRelativeDirectory() + @"/" + Path.GetFileNameWithoutExtension(_file.File.Name) + ".chr");
					ChrPipelinePanel.Pipeline = Pipeline as ChrPipeline;
					PaletteReductionPanel.Pipeline = Pipeline as ChrPipeline;
					RegisterChange();
					break;
				case ImagePipelines.Palette:
					RegisterChange();
					break;
			}
			RefreshLayout();
		}

		private void RegisterChange()
		{
			if (!_loading && PipelineChanged != null) PipelineChanged();
		}
	}
}
