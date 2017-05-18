using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serial_iwsk
{
    public partial class Form1 : Form
    {
        SerialPort port;
        bool polaczony = false;
        String[] porty;
        int[] predkosci = { 150, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };
        String terminator;
        public Form1()
        {
            InitializeComponent();
            comboBox2.DataSource = predkosci;
            comboBox2.SelectedItem = predkosci[6];
            comboBox3.SelectedIndex = 0; //none parity
            comboBox4.SelectedIndex = 1; //jeden znak stopu
            comboBox5.SelectedIndex = 1; //8bitów na znak
            comboBox6.SelectedIndex = 0; //Brak kontroli przepływu
            comboBox7.SelectedIndex = 1; //Standardowy terminator
            terminator = Environment.NewLine;
            wylistujPolaczenia();
        }

        private void wylistujPolaczenia()
        {
            porty = SerialPort.GetPortNames();
            foreach (string port in porty)
            {
                comboBox1.Items.Add(port);
                if (porty[0] != null)
                    comboBox1.SelectedItem = porty[0];
            }
        }

        private String wybierzTerminator()
        {
            return "";
        }
        private void polaczZPortem()
        {
            polaczony = true;
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            int predkosc = Int32.Parse(comboBox2.GetItemText(comboBox2.SelectedItem));
            Parity parzystosc = (Parity)Enum.Parse(typeof(Parity), comboBox3.SelectedIndex.ToString(), true);
            int liczba_bitow = Int32.Parse(comboBox5.GetItemText(comboBox5.SelectedItem));
            StopBits bity_stopu = (StopBits)Enum.Parse(typeof(StopBits), comboBox4.SelectedIndex.ToString(), true);
            Handshake kontrola = (Handshake)Enum.Parse(typeof(Handshake), comboBox6.SelectedIndex.ToString(), true);

            port = new SerialPort(selectedPort, predkosc, parzystosc, liczba_bitow, bity_stopu);
            port.Handshake = kontrola;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            switch (comboBox7.SelectedIndex)
            {
                case 0:
                    port.NewLine = "";
                    break;
                case 1:
                    port.NewLine = Environment.NewLine;
                    break;
                case 2:
                    port.NewLine = comboBox7.SelectedText.Remove(2);
                    break;

            }
            port.PinChanged += new SerialPinChangedEventHandler(pinChanged);
            port.Open();
            button1.Text = "Rozłącz";
            textBox1.AppendText(String.Format("Połączono z {0}! {1}", port.PortName, terminator));
        }

        private void rozlaczZPortem()
        {
            polaczony = false;
            port.Close();
            button1.Text = "Połącz";
            textBox1.AppendText(String.Format("Rozłączono z {0}! {1}", port.PortName, terminator));
        }

        private void pinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.WriteLine("ZMAINA");
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            indata = indata.Trim().Replace(Environment.NewLine, string.Empty);
            indata = indata.Trim().Replace("\r", string.Empty);
            indata = indata.Trim().Replace("\n", string.Empty);
            //if (indata.Equals("PING"))
            //ping_procedure();

            if (port.RtsEnable)
                label8.ForeColor = System.Drawing.Color.Green;
            else
                label8.ForeColor = System.Drawing.Color.Red;
            textBox1.Invoke(new Action(delegate ()
            {
                textBox1.AppendText(String.Format("<{0}>: {1}{2}", sp.PortName, indata, terminator));
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!polaczony)
            {
                polaczZPortem();
            }
            else
            {
                rozlaczZPortem();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                sendMessage();
            }
        }

        private void sendMessage()
        {
            port.WriteLine(textBox2.Text);
            textBox1.AppendText(String.Format("<Ja>: {0}{1}", textBox2.Text, terminator));
            textBox2.Clear();
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == 2)
            {
                ((ComboBox)sender).DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                ((ComboBox)sender).DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
    }
}
