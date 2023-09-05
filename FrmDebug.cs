using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace XboxDXTConverter
{
    public partial class FrmDebug : Form
    {
        public FrmDebug()
        {
            InitializeComponent();
            Console.SetOut(new TextBoxWriter(textBox1));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        } 

        private class TextBoxWriter : TextWriter
        {
            private TextBox _textBox;
            private StringBuilder _buffer = new StringBuilder();
            private const int UpdateIntervalMilliseconds = 200; // Adjust the update interval as needed
            private System.Threading.Timer _updateTimer;

            public TextBoxWriter(TextBox textBox)
            {
                try
                {
                    _textBox = textBox;
                    _updateTimer = new System.Threading.Timer(UpdateTextBoxCallback, null, UpdateIntervalMilliseconds, Timeout.Infinite);

                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "XDK Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }               
            }

            private void UpdateTextBoxCallback(object state)
            {
                try
                {
                    string textToUpdate = null;

                    lock (_buffer)
                    {
                        textToUpdate = _buffer.ToString();
                        _buffer.Clear();
                    }

                    if (!string.IsNullOrEmpty(textToUpdate))
                    {
                        _textBox.Invoke(new Action(() =>
                        {
                            _textBox.AppendText(textToUpdate);
                            _textBox.SelectionStart = _textBox.TextLength;
                            _textBox.ScrollToCaret();
                        }));
                    }

                    // Restart the timer for the next update
                    _updateTimer.Change(UpdateIntervalMilliseconds, Timeout.Infinite);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "XDK Update Callback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
               
            }

            public override void Write(char value)
            {
                try
                {
                    lock (_buffer)
                    {
                        _buffer.Append(value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "XDK Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public override void Write(string value)
            {
                try
                {
                    lock (_buffer)
                    {
                        _buffer.Append(value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "XDK Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public override Encoding Encoding => Encoding.UTF8;

            protected override void Dispose(bool disposing)
            {
                try
                {
                    base.Dispose(disposing);
                    if (disposing)
                    {
                        _updateTimer.Dispose();
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message, "XDK Dispose Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void FrmDebug_Load(object sender, EventArgs e)
        {

        }
    }
}
