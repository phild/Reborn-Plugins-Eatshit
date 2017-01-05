using System;
using System.Linq;
using System.Windows.Forms;
using EatShit.Settings;
using EatShit.Utilities;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace EatShit.Gui
{
    public partial class EatShitSettingsForm : Form
    {
        public EatShitSettingsForm()
        {
            InitializeComponent();





	        comboBox1.DataSource =
		        InventoryManager.FilledSlots.FoodItems()
			        .Select(x => x.Item.IsHighQuality ? x.Item.EnglishName.ToString() + "- (HQ) - " + x.Item.ItemCount() + "x" : x.Item.EnglishName.ToString() + "- " + x.Item.ItemCount() + "x").ToList();
			//   x.Item.EnglishName.ToString() + "- (HQ) - " + x.Item.ItemCount() + "x"; 
		}

		private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
			// EatShitSettings.Instance.FoodName = comboBox1.SelectedValue.ToString();
			// EatShitSettings.Instance.Save();
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			var result = comboBox1.SelectedValue.ToString().Split('-');
			EatShitSettings.Instance.FoodName = result[0];
			EatShitSettings.Instance.Save();
		}
	}
}