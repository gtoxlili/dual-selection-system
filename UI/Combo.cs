using System.Windows.Forms;

namespace UI
{
    public class Combo : ComboBox
    {
        public Combo()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            MouseWheel += (o, args) => ((HandledMouseEventArgs)args).Handled = !((ComboBox)o).Focused;
        }
    }
}