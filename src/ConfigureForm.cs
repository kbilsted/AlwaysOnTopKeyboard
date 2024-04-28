namespace AlwaysOnTopKeyboard;

public partial class ConfigureForm : Form
{
    public string NewLayout { get; private set; } 
    public double NewOpacity { get; private set; }

    public ConfigureForm(string currentLayout, double currentOpacity)
    {
        InitializeComponent();
        Label labelKeyboard = new Label
        {
            Text = "Keyboard Layout:",
            Dock = DockStyle.Top,
            Height = 20,
        };
        TextBox textBox = new TextBox
        {
            Multiline = true,
            Text = currentLayout,
            Dock = DockStyle.Top,
            Height = 120,
        };


        // Opacity TextBox and Label
        Label labelOpacity = new Label
        {
            Text = "Opacity (0.0 - 1.0):",
            Dock = DockStyle.Top,
            Height = 20
        };
        TextBox textBoxOpacity = new TextBox
        {
            Text = currentOpacity.ToString(),
            Dock = DockStyle.Top,
            Height = 20
        };

        Button btnApply = new Button
        {
            Text = "Apply",
            Dock = DockStyle.Top,
            Height = 30
        };

        btnApply.Click += (sender, e) =>
        {
            NewLayout = textBox.Text;
            if (double.TryParse(textBoxOpacity.Text, out double opacity))
            {
                NewOpacity = opacity;
            }
            else
            {
                MessageBox.Show("Invalid opacity value. Please enter a number between 0.0 and 1.0.");
                return;
            }
            this.DialogResult = DialogResult.OK; // Sets the dialog result and closes the form
        };

        Controls.Add(btnApply);
        Controls.Add(new Label() { Height = 20, Dock = DockStyle.Top, });

        Controls.Add(textBoxOpacity);
        Controls.Add(labelOpacity);
        Controls.Add(new Label() { Height = 20, Dock = DockStyle.Top, }); 

        Controls.Add(textBox);
        Controls.Add(labelKeyboard);
        Controls.Add(new Label() { Height = 20, Dock = DockStyle.Top, });

        ClientSize = new Size(300, 280);


        // Set the default font for all controls
        MainWindow.SetDefaultFont(Controls, Globals.Ourfont);

        TopMost = true; // Make the form always stay on top
    }

}