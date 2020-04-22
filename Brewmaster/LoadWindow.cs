using System.Windows.Forms;

namespace BrewMaster
{
    public partial class LoadWindow : Form
    {
        public LoadWindow()
        {
            InitializeComponent();
        }
		public void ShowImmediately(IWin32Window owner) { Show(owner); Refresh(); }
	}
}
