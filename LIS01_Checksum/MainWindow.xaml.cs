// This software is provided “AS IS” (see below).
// 
// Copyright @2023 Theron W. Genaux
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the “Software”),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
//  - The above copyright notice and this permission notice is required
//  to be included in all copies or substantial portions of the Software. 
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//  CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AsciiHelper;

namespace AstmChecksum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<EcodingItem> _encodingInfo;

        public MainWindow()
        {
            InitializeComponent();

            _encodingInfo = new ObservableCollection<EcodingItem>();

            int index = 0;
            int count = 0;

            EncodingInfo[] encodings = Encoding.GetEncodings();
            foreach (var ei in encodings)
            {
                _encodingInfo.Add(new EcodingItem(ei));

                if (ei.GetEncoding().BodyName == Encoding.UTF8.BodyName)
                {
                    index = count;
                }
                count++;
            }
            encodingComboBox.ItemsSource = _encodingInfo;
            encodingComboBox.SelectedIndex = index;
        }

        public void SetWindowTitle()
        {
            Version version;
            try
            {
                // Add published version
                // Requires reference to System.Deployment
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                // Add Assembly Version
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }

            mainWindow.Title = "LIS1-A / ASTM E1381 Frame Checksum Calculator v" + version;

            frameTextBox.Text = "<STX>2P|1|PID123456|||Genaux^Theron^W||16680515|M<CR><ETX>8D<CR><LF>";
        }


        private void characterMapButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("charmap.exe");
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowTitle();
        }

        private void updateHexbox()
        {
            string frameText = frameTextBox.Text;

            string processed = LIS1Helper.LIS1Helper.Unbeautify(frameText);

            EcodingItem item = (EcodingItem)encodingComboBox.SelectedItem;
            Encoding encoding = item.EncodingInfo.GetEncoding();

            byte[] frame = encoding.GetBytes(processed);

            string checksum = LIS1Helper.LIS1Helper.CalculateFrameChecksum(frame);
            checksumTextBox.Text = checksum;
            
            int pos = -1;
            if (((pos = Array.IndexOf(frame, B.ETX)) > -1) || ((pos = Array.IndexOf(frame, B.ETB)) > -1))
            {
                if (((pos + 5) == frame.Length) && (frame[frame.Length - 2] == B.CR) && (frame[frame.Length - 1] == B.LF))
                {
                    frame[frame.Length - 4] = (byte)checksum[0];
                    frame[frame.Length - 3] = (byte)checksum[1];
                }
                else
                {
                    byte[] newFrame = new byte[pos + 5]; // new array of the max size
                    Array.Copy(frame, newFrame, Math.Min(frame.Length, (pos + 5))); // copy frame into new array
                    frame = newFrame; // replace frame with new frame
                    frame[frame.Length - 4] = (byte)checksum[0];
                    frame[frame.Length - 3] = (byte)checksum[1];
                    frame[frame.Length - 2] = B.CR;
                    frame[frame.Length - 1] = B.LF;
                }
            }
            
            string hex = BitConverter.ToString(frame);
            hexBox.Text = hex;

            byte[] beautified = LIS1Helper.LIS1Helper.Beautify(frame);
            beautifiedBox.Text = encoding.GetString(beautified);
        }

        private void frameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateHexbox();
        }

        private void encodingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateHexbox();
        }
    }

    public class EcodingItem
    {
        public string CodePage { get { return EncodingInfo.GetEncoding().CodePage.ToString(); } }
        public string BodyName { get { return "  " + EncodingInfo.GetEncoding().BodyName; } }
        public string EncodingName { get { return "  " + EncodingInfo.GetEncoding().EncodingName; } }

        public EncodingInfo EncodingInfo { get; set; }

        public EcodingItem(EncodingInfo encodingInfo)
        {
            EncodingInfo = encodingInfo;
        }

    };
}
