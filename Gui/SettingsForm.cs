using System;
using System.Linq;
using System.Windows.Forms;
using EatShit.Settings;
using EatShit.Utilities;
using ff14bot.Managers;

namespace EatShit.Gui
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            comboBox1.DataSource = InventoryManager.FilledSlots.FoodItems().Select(x => x.Item.EnglishName + "(HQ: " + x.Item.IsHighQuality.ToString() + ")").ToList(); 
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            EatShitSettings.Instance.FoodName = comboBox1.SelectedValue.ToString();
            EatShitSettings.Instance.Save();
        }

		private void SettingsForm_Load(object sender, EventArgs e)
		{

		}
	}
}