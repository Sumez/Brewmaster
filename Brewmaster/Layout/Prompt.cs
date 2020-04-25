using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Ide
{
	public static class Prompt
	{
		public static bool ShowDialog(string text, string caption)
		{
			using (var prompt = new Form
			{
				FormBorderStyle = FormBorderStyle.FixedDialog,
				Text = caption,
				StartPosition = FormStartPosition.CenterScreen,
				ShowInTaskbar = false,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowOnly,
				Height = 10,
				Width = 340
			}) {
				var textLabel = new Label
				{
					Padding = new Padding(10),
					Dock = DockStyle.Top,
					Text = text,
					AutoSize = true,
					MaximumSize = new Size(300, 600),
					MinimumSize = new Size(300, 10)
				};
				var confirmation = new Button
				{
					Text = "Yes",
					Dock = DockStyle.Right,
					DialogResult = DialogResult.Yes
				};
				var rejection = new Button
				{
					Text = "No",
					Dock = DockStyle.Right,
					DialogResult = DialogResult.No
				};
				var buttonContainer = new ContainerControl
				{
					Dock = DockStyle.Top,
					Height = confirmation.Height + 10,
					Padding = new Padding(10, 0, 10, 10)
				};
				confirmation.Click += (sender, e) => { prompt.Close(); };
				rejection.Click += (sender, e) => { prompt.Close(); };

				buttonContainer.Controls.Add(confirmation);
				buttonContainer.Controls.Add(rejection);
				prompt.Controls.Add(buttonContainer);
				prompt.Controls.Add(textLabel);

				return prompt.ShowDialog() == DialogResult.Yes;
			}
		}
	}
}
