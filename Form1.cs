using XboxDXTConverter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace XboxDXTConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            aspectRatioComboBox.SelectedIndex = 0; // Default to "None"

            // pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            // Use Invoke to update the PictureBox on the UI thread
            
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Adjust this to your needs
            aspectRatioComboBox.SelectedIndexChanged += AspectRatioComboBox_SelectedIndexChanged;

            // groupBox1.Controls.Add(pictureBox1);

            // Subscribe to the Resize event of the form
            Resize += Form1_Resize;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Adjust the size and aspect ratio of the PictureBox based on the form's size
            UpdatePictureBoxSize();
            CenterPictureBox();
        }
        private void AspectRatioComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the PictureBox's aspect ratio based on the selected item in the ComboBox
            UpdatePictureBoxSize();
            CenterPictureBox();
        }


        private void UpdatePictureBoxSize()
        {
            int groupBoxWidth = groupBox1.ClientSize.Width;
            int groupBoxHeight = groupBox1.ClientSize.Height;

            // Calculate the aspect ratio based on the selected item in the ComboBox
            double aspectRatio;
            if (aspectRatioComboBox.SelectedIndex == 0) // None
            {
                aspectRatio = 0; // Use Invoke to update the PictureBox on the UI thread
                Invoke(new Action(() =>
                { 
                    pictureBox1.Dock = DockStyle.Fill; // Undock the PictureBox
                }));
                return;
            }
            else if (aspectRatioComboBox.SelectedIndex == 1) // 4:3
                aspectRatio = 4.0 / 3.0;
            else if (aspectRatioComboBox.SelectedIndex == 2) // 16:9
                aspectRatio = 16.0 / 9.0;
            else if (aspectRatioComboBox.SelectedIndex == 3) // Scale Image
            {
                aspectRatio = 0; 
                if (pictureBox1.Image != null)
                { 
                    // Use Invoke to update the PictureBox on the UI thread
                    Invoke(new Action(() =>
                    {
                        // Set the PictureBox's size to match the size of the image
                        pictureBox1.Size = pictureBox1.Image.Size;
                        pictureBox1.Dock = DockStyle.None; // Undock the PictureBox
                    })); 

                } else
                {
                    // Use Invoke to update the PictureBox on the UI thread
                    Invoke(new Action(() =>
                    {
                        // Set the PictureBox's size to match the size of the image
                        pictureBox1.Size = pictureBox1.BackgroundImage.Size;
                        pictureBox1.Dock = DockStyle.None; // Dock the PictureBox within the GroupBox 
                    }));
                }
                return;
            } 
            else
                aspectRatio = 0;

            // When an aspect ratio is selected, undock the PictureBox and calculate its size to maintain the aspect ratio
           // pictureBox1.Dock = DockStyle.None;
            // Use Invoke to update the PictureBox on the UI thread
            Invoke(new Action(() =>
            { 
                pictureBox1.Dock = DockStyle.None; // Undock the PictureBox
            }));


            // Calculate the maximum size that fits within the GroupBox's boundaries
            int maxWidth = groupBoxWidth;
            int maxHeight = groupBoxHeight;

            // Calculate the size that maintains the selected aspect ratio
            if (aspectRatio > 0)
            {
                if ((double)maxHeight / aspectRatio <= maxWidth)
                {
                    maxWidth = (int)(maxHeight * aspectRatio);
                }
                else
                {
                    maxHeight = (int)(maxWidth / aspectRatio);
                }
            }

            // Use Invoke to update the PictureBox on the UI thread
            Invoke(new Action(() =>
            {
                // Set the size of the PictureBox
                pictureBox1.Size = new Size(maxWidth, maxHeight);
            }));
        }

        private void CenterPictureBox()
        {
            int panelWidth = groupBox1.ClientSize.Width;
            int panelHeight = groupBox1.ClientSize.Height;

            int pictureBoxWidth = pictureBox1.Width;
            int pictureBoxHeight = pictureBox1.Height;

            int x = (panelWidth - pictureBoxWidth) / 2;
            int y = (panelHeight - pictureBoxHeight) / 2;

            // Use Invoke to update the PictureBox on the UI thread
            Invoke(new Action(() =>
            {
                pictureBox1.Location = new Point(x, y);
            }));
        }


        // Create our Debugger //
        FrmDebug frmDebug = new FrmDebug();
        string imageName = "";
        string tmpLocation = Environment.CurrentDirectory;


        private void button1_Click(object sender, EventArgs e)
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set properties and options for the dialog to allow image files
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif;*.tiff";
            openFileDialog.Title = "Open Image File";

            // Show the dialog and check if the user clicked "OK"
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file name and work with the image
                string selectedFileName = openFileDialog.FileName;
                textBox1.Text = selectedFileName;                 
                // Use Path.GetFileName to get just the file name without the path
                imageName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                // Example: Load and display the selected image
                // You can replace this with your own image-processing logic
                try
                {
                    // Load the image using a suitable library (e.g., System.Drawing for Windows Forms)
                    System.Drawing.Image image = System.Drawing.Image.FromFile(selectedFileName);
                     
                    // Use Invoke to update the PictureBox on the UI thread
                    Invoke(new Action(() =>
                    {
                        pictureBox1.Image = image;
                        pictureBox1.BackgroundImage = null;
                    })); 

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
          
           //  frmDebug.Show();
            //Console.WriteLine(textBox1.Text);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }


        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
            {
                Console.WriteLine("No image file was selected to convert to the standard Xbox DXT format.");
                MessageBox.Show("No image file was selected to convert to the standard Xbox DXT format.", "Xbox Development Kit", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return;
            }
            // crunch_x64.exe -file cdx0.tga -fileformat dds -dxt3
            // crunch_x64.exe - file cdx1.tga - fileformat dds - a8r8g8b8
            //  FrmDebug frmDebug = new FrmDebug();

            toolStripProgressBar1.Visible = true;
            // Clean up the temporary directory if needed
            // Specify the path to the executable you want to run
            string executablePath = Environment.CurrentDirectory + @"\bin\crunch_x64.exe";

            string dxt3 = ""; 

            if (rbDXT3.Checked == true)
            {
                dxt3 = "-dxt3"; // USE DXT3
            }
            else
            {
                dxt3 = "-a8r8g8b8"; // USE ARGB 
            }

            string quote = "\""; // " //  
            string arguments = $"-file {quote + textBox1.Text + quote} -fileformat dds {dxt3}"; 

            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath, 
                Arguments = arguments,
                UseShellExecute = false, // Set to false to redirect input/output
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true // Set to true to hide the process window
            };

            // Create a new process and start it
            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            // Optionally, you can read the output/error streams of the process
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit(); // Wait for the process to finish

            // Display the output and error, if any
            Console.WriteLine("Output:\n" + output);
            Console.WriteLine("Error:\n" + error);

            // Optionally, you can check the exit code of the process
            int exitCode = process.ExitCode;
            Console.WriteLine("Exit Code: " + exitCode);

            // Close the process
            process.Close();

            toolStripProgressBar1.Visible = false;



            // Let's try moving our completed file
            try
            {

                // tmpLocation
                if (!File.Exists(Environment.CurrentDirectory + @"\" + imageName +".dds"))
                {
                    // The file did not create, there for something failed.
                    MessageBox.Show("Something went wrong. Please open the debugger for more information.");
                    return;
                }

                if (!Directory.Exists(Environment.CurrentDirectory + @"\exported"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\exported");
                }

                File.Move(Environment.CurrentDirectory + @"\" + imageName + ".dds", Environment.CurrentDirectory + @"\exported\" + imageName + ".dds");

                button3.PerformClick();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "XDK Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            { 
                toolStripProgressBar1.Visible = false; 
            }

        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            frmDebug.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(Environment.CurrentDirectory + @"\bin"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\bin");
                }

                if (!File.Exists(Environment.CurrentDirectory + @"\bin\crunch_x64.exe"))
                {
                    File.WriteAllBytes(Environment.CurrentDirectory + @"\bin\crunch_x64.exe", Resources.crunch_x64);
                }

                Console.WriteLine("Loading... Done.");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "XDK Error", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
            }  
        }
         

        private void button3_Click(object sender, EventArgs e)
        {
            try
            { 
                if (!Directory.Exists(Environment.CurrentDirectory + @"\exported"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\exported");
                }

                Process.Start("explorer.exe ", Environment.CurrentDirectory + @"\exported");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Xbox Development Kit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();  
            aboutBox1.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Use Invoke to update the PictureBox on the UI thread
            Invoke(new Action(() =>
            {
                textBox1.Clear();
                pictureBox1.BackgroundImage = XboxDXTConverter.Properties.Resources.logow1;
                pictureBox1.Image = null;
            }));
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // Update the PictureBox's aspect ratio based on the selected item in the ComboBox
            UpdatePictureBoxSize();
            CenterPictureBox();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
