﻿using System.Runtime.InteropServices;
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

    Button cfgbutton;

    public MainWindow()
    {
        InitializeComponent();

        BackColor = Color.Black;
        ForeColor = Color.YellowGreen;

        InitializeKeyboard();
        SetDefaultFont(Controls, Globals.Ourfont);

        // Remove the title bar and border
        FormBorderStyle = FormBorderStyle.None;

        // Make the form always stay on top
        TopMost = true;

        Load += new EventHandler(OnLoad);
        
        FormClosing += new FormClosingEventHandler(OnClosing);


        // Enable mouse dragging the form around
        MouseDown += new MouseEventHandler(Form_MouseDown);

    }


    private void OnLoad(object? sender, EventArgs e)
    {
        // Load the window location and size
        if (Properties.Settings.Default.WindowLocation != new Point(0, 0))
        {
            Location = Properties.Settings.Default.WindowLocation;
        }

        if (Properties.Settings.Default.WindowSize == new Size(0, 0))
            Properties.Settings.Default.WindowSize = new Size(270, 860);
        Size = Properties.Settings.Default.WindowSize;

        if (Properties.Settings.Default.WindowOpacity == 0)
            Properties.Settings.Default.WindowOpacity = 0.8;
        Opacity = Properties.Settings.Default.WindowOpacity;

        // Make the form always stay on top
        TopMost = true;
    }

    private void OnClosing(object? sender, FormClosingEventArgs e)
    {
        // Save the window location and size
        Properties.Settings.Default.WindowLocation = Location;
        Properties.Settings.Default.WindowSize = Size;
        Properties.Settings.Default.WindowOpacity = Opacity;

        Properties.Settings.Default.Save(); // Saves settings
    }


    private void Form_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // Release the current mouse capture
            ReleaseCapture();
            // Send the WM_NCLBUTTONDOWN message to the window
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
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
        using (ConfigureForm configForm = new ConfigureForm(keyboardLayout, Properties.Settings.Default.WindowOpacity))
        {
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                keyboardLayout = configForm.NewLayout; // Get the new layout
                Properties.Settings.Default.WindowOpacity = configForm.NewOpacity; // Set the new opacity
                Opacity = Properties.Settings.Default.WindowOpacity;

                OnClosing(null, null);


                Controls.Clear(); // Clear existing controls
                InitializeComponent(); // Reinitialize components
                InitializeKeyboard(); // Rebuild the keyboard
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
