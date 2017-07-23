using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeyValueLibary;

namespace TestDemo
{
    public partial class KeyValueTest :Form
    {
        public KeyValueTest()
        {
            InitializeComponent();
        }

        private void button1_Click( object sender, EventArgs e )
        {
            kve["sdfsd"] = new KeyValueEx
            {
                 {"123",""}
            };
            ;
        }
        KeyValueEx kve = new KeyValueEx();
        private void button2_Click( object sender, EventArgs e )
        {
          bool b=  kve.JsonStringToKeyValueEx( textBox1.Text );
          MessageBox.Show( string.Format( "转换{0}", b ) );
            
        }

        private void button3_Click( object sender, EventArgs e )
        {
            textBox2.Text = kve.ToJsonString();
        }

        private void button4_Click( object sender, EventArgs e )
        {
            bool b = kve.XmlStringToKeyValueEx( textBox3.Text, "List" );
          MessageBox.Show( string.Format( "转换{0}", b ) );
        }

        private void button5_Click( object sender, EventArgs e )
        {
            textBox4.Text = kve.ToXmlString();
        }

        private void menuButton1_Click( object sender, EventArgs e )
        {
            ( new KeyValueTest() ).Show();
        }

        private void menuButton2_Click( object sender, EventArgs e )
        {
            ( new KeyValueTest() ).Show();
        }

        private void Form1_KeyPress( object sender, KeyPressEventArgs e )
        {
            if( e.KeyChar == 13 )
            {
                textBox3.Text += "form enter press " + Environment.NewLine;
            }
        }

        private void textBox3_KeyPress( object sender, KeyPressEventArgs e )
        {

            
        }

        private void textBox3_KeyDown( object sender, KeyEventArgs e )
        {
            if( e.KeyCode ==  Keys.Enter )
            {
                textBox3.Text += "textBox3 enter press " + Environment.NewLine;
            }
        }

        private void textBox5_KeyDown( object sender, KeyEventArgs e )
        {
            MessageBox.Show( e.KeyCode + " down" );
        }

        private void textBox5_KeyPress( object sender, KeyPressEventArgs e )
        {
            //MessageBox.Show( e.KeyChar + " press" );
        }
    }
}
