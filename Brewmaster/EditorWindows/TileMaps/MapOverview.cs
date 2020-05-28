using System;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapOverview : UserControl
	{
		public event Action MapSizeChanged;
		public MapOverview(TileMap map)
		{
			_map = map;
			InitializeComponent();
			_miniMap.Map = map;
			_miniMap.Invalidate();

			UpdateDataView();

			_decWidth.Click += (s, a) => EditDimensions(-1, 0);
			_incWidth.Click += (s, a) => EditDimensions(1, 0);
			_decHeight.Click += (s, a) => EditDimensions(0, -1);
			_incHeight.Click += (s, a) => EditDimensions(0, 1);
		}

		public Action<int, int> ActivateScreen
		{
			get { return _miniMap.ActivateScreen; }
			set { _miniMap.ActivateScreen = value; }
		}

		private void EditDimensions(int deltaX, int deltaY)
		{
			var width = Math.Max(1, _map.Width + deltaX);
			var height = Math.Max(1, _map.Height + deltaY);
			if (_map.Width == width && _map.Height == height) return;
			_map.Width = width;
			_map.Height = height;
			if (MapSizeChanged != null) MapSizeChanged();
			UpdateDataView();
			_miniMap.PerformLayout();
			_miniMap.Invalidate();
		}

		private void UpdateDataView()
		{
			_width.Text = _map.Width.ToString();
			_height.Text = _map.Height.ToString();
		}

		private Label _label1;
		private Label _label2;
		private Button _incWidth;
		private Button _decWidth;
		private Button _decHeight;
		private Button _incHeight;
		private Label _height;
		private Panel _mapPanel;
		private MiniMap _miniMap;
		private Label _width;
		private TileMap _map;

		private void InitializeComponent()
		{
			this._label1 = new System.Windows.Forms.Label();
			this._label2 = new System.Windows.Forms.Label();
			this._width = new System.Windows.Forms.Label();
			this._incWidth = new System.Windows.Forms.Button();
			this._decWidth = new System.Windows.Forms.Button();
			this._decHeight = new System.Windows.Forms.Button();
			this._incHeight = new System.Windows.Forms.Button();
			this._height = new System.Windows.Forms.Label();
			this._mapPanel = new System.Windows.Forms.Panel();
			this._miniMap = new Brewmaster.EditorWindows.TileMaps.MiniMap();
			this._mapPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this._label1.AutoSize = true;
			this._label1.Location = new System.Drawing.Point(3, 0);
			this._label1.Name = "_label1";
			this._label1.Size = new System.Drawing.Size(35, 13);
			this._label1.TabIndex = 0;
			this._label1.Text = "Width";
			// 
			// label2
			// 
			this._label2.AutoSize = true;
			this._label2.Location = new System.Drawing.Point(91, 0);
			this._label2.Name = "_label2";
			this._label2.Size = new System.Drawing.Size(38, 13);
			this._label2.TabIndex = 1;
			this._label2.Text = "Height";
			// 
			// _width
			// 
			this._width.AutoSize = true;
			this._width.Location = new System.Drawing.Point(47, 0);
			this._width.Name = "_width";
			this._width.Size = new System.Drawing.Size(19, 13);
			this._width.TabIndex = 2;
			this._width.Text = "20";
			// 
			// _incWidth
			// 
			this._incWidth.FlatAppearance.BorderSize = 0;
			this._incWidth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._incWidth.Image = global::Brewmaster.Properties.Resources.collapsed;
			this._incWidth.Location = new System.Drawing.Point(63, 0);
			this._incWidth.Name = "_incWidth";
			this._incWidth.Size = new System.Drawing.Size(12, 13);
			this._incWidth.TabIndex = 3;
			this._incWidth.UseVisualStyleBackColor = true;
			// 
			// _decWidth
			// 
			this._decWidth.FlatAppearance.BorderSize = 0;
			this._decWidth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._decWidth.Image = global::Brewmaster.Properties.Resources.left_arrow;
			this._decWidth.Location = new System.Drawing.Point(36, 0);
			this._decWidth.Name = "_decWidth";
			this._decWidth.Size = new System.Drawing.Size(12, 13);
			this._decWidth.TabIndex = 4;
			this._decWidth.UseVisualStyleBackColor = true;
			// 
			// _decHeight
			// 
			this._decHeight.FlatAppearance.BorderSize = 0;
			this._decHeight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._decHeight.Image = global::Brewmaster.Properties.Resources.left_arrow;
			this._decHeight.Location = new System.Drawing.Point(127, 0);
			this._decHeight.Name = "_decHeight";
			this._decHeight.Size = new System.Drawing.Size(12, 13);
			this._decHeight.TabIndex = 7;
			this._decHeight.UseVisualStyleBackColor = true;
			// 
			// _incHeight
			// 
			this._incHeight.FlatAppearance.BorderSize = 0;
			this._incHeight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._incHeight.Image = global::Brewmaster.Properties.Resources.collapsed;
			this._incHeight.Location = new System.Drawing.Point(154, 0);
			this._incHeight.Name = "_incHeight";
			this._incHeight.Size = new System.Drawing.Size(12, 13);
			this._incHeight.TabIndex = 6;
			this._incHeight.UseVisualStyleBackColor = true;
			// 
			// _height
			// 
			this._height.AutoSize = true;
			this._height.Location = new System.Drawing.Point(138, 0);
			this._height.Name = "_height";
			this._height.Size = new System.Drawing.Size(19, 13);
			this._height.TabIndex = 5;
			this._height.Text = "20";
			// 
			// panel1
			// 
			this._mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mapPanel.AutoScroll = true;
			this._mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._mapPanel.Controls.Add(this._miniMap);
			this._mapPanel.Location = new System.Drawing.Point(6, 16);
			this._mapPanel.Name = "_mapPanel";
			this._mapPanel.Size = new System.Drawing.Size(428, 285);
			this._mapPanel.TabIndex = 8;
			// 
			// miniMap1
			// 
			this._miniMap.Location = new System.Drawing.Point(0, 0);
			this._miniMap.Map = null;
			this._miniMap.Name = "_miniMap";
			this._miniMap.Size = new System.Drawing.Size(75, 23);
			this._miniMap.TabIndex = 0;
			this._miniMap.Text = "miniMap1";
			// 
			// MapOverview
			// 
			this.Controls.Add(this._mapPanel);
			this.Controls.Add(this._decHeight);
			this.Controls.Add(this._incHeight);
			this.Controls.Add(this._height);
			this.Controls.Add(this._decWidth);
			this.Controls.Add(this._incWidth);
			this.Controls.Add(this._width);
			this.Controls.Add(this._label2);
			this.Controls.Add(this._label1);
			this.Name = "MapOverview";
			this.Size = new System.Drawing.Size(437, 304);
			this._mapPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public void UpdateScreenImage(TileMapScreen screen)
		{
			for (var y = 0; y < _map.Screens.Count; y++)
			{
				var x = _map.Screens[y].IndexOf(screen);
				if (x >= 0) _miniMap.UpdateScreenImage(x, y);
			}
		}
	}
}
