using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AlwaysOnTopKeyboard;

public partial class MainWindow : Form
{
    // P/Invoke declarations
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    const int WM_NCLBUTTONDOWN = 0xA1;
    const int HT_CAPTION = 0x2;

    private string keyboardLayout = Globals.RollerCoasterKeyboardLayout;

    double opacityLayout = 0.4;
    Button cfgbutton;

    public MainWindow()
    {
        InitializeComponent();

        Opacity = opacityLayout;
        BackColor = Color.Black;
        ForeColor = Color.YellowGreen;

        InitializeKeyboard();
        SetDefaultFont(Controls, Globals.Ourfont);

        // Remove the title bar and border
        FormBorderStyle = FormBorderStyle.None;

        // Enable mouse dragging the form around
        MouseDown += new MouseEventHandler(Form_MouseDown);

        // Make the form always stay on top
        TopMost = true;
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // Release the current mouse capture
            ReleaseCapture();
            // Send the WM_NCLBUTTONDOWN message to the window
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    private void InitializeKeyboard()
    {
        int x = 10, y = 10; // Starting position of the first key
        int keyWidth = 50, keyHeight = 50; // Width and height of keys
        int row = 0; // Row counter

        foreach (char key in keyboardLayout
            .SkipWhile(x => x == '\n' || x == '\r')
            .Where(c => c != '\r'))
        {
            if (key == '\n')
            {
                row++;
                x = 10;
                y = 10 + row * (keyHeight + 10); // Increment y position
                continue;
            }

            if (key == ' ')
            {
                x += (keyWidth / 2) + (10 / 2);
                continue;
            }

            Button button = new Button
            {
                Text = key.ToString(),
                Location = new Point(x, y),
                Size = new Size(keyWidth, keyHeight),
                ForeColor = Color.YellowGreen,
                FlatStyle = FlatStyle.Flat,
                Enabled = false

            };
            button.FlatAppearance.BorderSize = 2; // Set the border size
            button.Paint += new PaintEventHandler((sender, e) => Button_Paint(sender, e, Color.Green)); // Custom paint handler
            Controls.Add(button);

            x += keyWidth + 10; // Increment x position for next key
        }

        // cfg button
        cfgbutton = new Button
        {
            Text = "?",
            Location = new Point(x, y),
            Size = new Size(keyWidth, keyHeight),
            ForeColor = Color.Red,
            FlatStyle = FlatStyle.Flat,
        };
        cfgbutton.FlatAppearance.BorderSize = 2; // Set the border size
        cfgbutton.Click += configure_Click;
        Controls.Add(cfgbutton);
    }

    private void configure_Click(object? sender, EventArgs e)
    {
        using (ConfigureForm configForm = new ConfigureForm(keyboardLayout, opacityLayout))
        {
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                keyboardLayout = configForm.NewLayout; // Get the new layout
                this.opacityLayout = configForm.NewOpacity; // Set the new opacity
                this.Opacity = opacityLayout;
                this.Controls.Clear(); // Clear existing controls
                this.InitializeComponent(); // Reinitialize components
                this.InitializeKeyboard(); // Rebuild the keyboard
            }
        }
    }

    private void Button_Paint(object sender, PaintEventArgs e, Color borderColor)
    {
        Button button = sender as Button;

        // Draw border
        Pen pen = new Pen(borderColor, 2);
        Rectangle rectangle = new Rectangle(0, 0, button.Width - 1, button.Height - 1);
        e.Graphics.DrawRectangle(pen, rectangle);

        TextRenderer.DrawText(e.Graphics, button.Text, button.Font, rectangle, button.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    public static void SetDefaultFont(Control.ControlCollection controls, Font font)
    {
        foreach (Control control in controls)
        {
            control.Font = font; // Set font for each control
            if (control.HasChildren)
            {
                SetDefaultFont(control.Controls, font); // Recursively set font for child controls
            }
        }
    }
}
