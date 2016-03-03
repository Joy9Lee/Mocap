using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetWork;

namespace collector
{
    public partial class Profiles_Form : Form
    {
        UdpNetWork netWork;
        private CheckBox[] checkBoxs;
        public Profiles_Form(UdpNetWork netWork1)
        {
            InitializeComponent();
            netWork = netWork1;
            checkBoxs = new CheckBox[4] { cBox_EMG1, cBox_EMG2, cBox_EMG3, cBox_EMG4 };
            cBox_Node.Items.AddRange(netWork.nodes.ToArray());
            cBox_Node.SelectedIndexChanged += new EventHandler(cBox_Node_SelectedIndexChanged);
            cBox_Node.SelectedIndex = 0;//设置第一项为默认选择项。

        }

        private void cBox_Node_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 获取被选中项目
            Node node = cBox_Node.SelectedItem as Node;
            // 执行操作
            text_ID.Text = node.profiles.ID.ToString();
            text_mRate.Text = node.profiles.MOTION_RATE.ToString();
            text_eRate.Text = node.profiles.EMG_RATE.ToString();

            text_Acc_Factor_x.Text = node.profiles.ACC_FACTOR[0].ToString();
            text_Acc_Factor_y.Text = node.profiles.ACC_FACTOR[1].ToString();
            text_Acc_Factor_z.Text = node.profiles.ACC_FACTOR[2].ToString();

            text_Acc_Bias_x.Text = node.profiles.ACC_BIAS[0].ToString();
            text_Acc_Bias_y.Text = node.profiles.ACC_BIAS[1].ToString();
            text_Acc_Bias_z.Text = node.profiles.ACC_BIAS[2].ToString();

            text_Meg_Factor_x.Text = node.profiles.MEG_FACTOR[0].ToString();
            text_Meg_Factor_y.Text = node.profiles.MEG_FACTOR[1].ToString();
            text_Meg_Factor_z.Text = node.profiles.MEG_FACTOR[2].ToString();

            text_Meg_Bias_x.Text = node.profiles.MEG_BIAS[0].ToString();
            text_Meg_Bias_y.Text = node.profiles.MEG_BIAS[1].ToString();
            text_Meg_Bias_z.Text = node.profiles.MEG_BIAS[2].ToString();

            text_Gyo_Factor_x.Text = node.profiles.GYO_FACTOR[0].ToString();
            text_Gyo_Factor_y.Text = node.profiles.GYO_FACTOR[1].ToString();
            text_Gyo_Factor_z.Text = node.profiles.GYO_FACTOR[2].ToString();

            text_Gyo_Bias_x.Text = node.profiles.GYO_BIAS[0].ToString();
            text_Gyo_Bias_y.Text = node.profiles.GYO_BIAS[1].ToString();
            text_Gyo_Bias_z.Text = node.profiles.GYO_BIAS[2].ToString();

            if (node.profiles.MOTION_EN == 1)
            {
                cBox_Motion.CheckState = CheckState.Checked;
            }
            else
            {
                cBox_Motion.CheckState = CheckState.Unchecked;
            }

            for (int i = 0; i < 4; i++)
            {
                if ((node.profiles.EMG_CHL & (0x01<<(7 - i))) != 0)
                {
                    checkBoxs[i].CheckState = CheckState.Checked;
                }
                else
                {
                    checkBoxs[i].CheckState = CheckState.Unchecked;
                }
            }

        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            Node node =  netWork.nodes[cBox_Node.SelectedIndex];
            node.profiles.ID = byte.Parse(text_ID.Text);
            node.profiles.MOTION_RATE = short.Parse(text_mRate.Text);
            node.profiles.EMG_RATE = short.Parse(text_eRate.Text);

            node.profiles.ACC_FACTOR[0] = float.Parse(text_Acc_Factor_x.Text);
            node.profiles.ACC_FACTOR[1] = float.Parse(text_Acc_Factor_y.Text);
            node.profiles.ACC_FACTOR[2] = float.Parse(text_Acc_Factor_z.Text); 
            node.profiles.ACC_BIAS[0] = float.Parse(text_Acc_Bias_x.Text); 
            node.profiles.ACC_BIAS[1] = float.Parse(text_Acc_Bias_y.Text); 
            node.profiles.ACC_BIAS[2] = float.Parse(text_Acc_Bias_x.Text);
            node.profiles.MEG_FACTOR[0] = float.Parse(text_Meg_Factor_x.Text);
            node.profiles.MEG_FACTOR[1] = float.Parse(text_Meg_Factor_y.Text);
            node.profiles.MEG_FACTOR[2] = float.Parse(text_Meg_Factor_z.Text);
            node.profiles.MEG_BIAS[0] = float.Parse(text_Meg_Bias_x.Text);
            node.profiles.MEG_BIAS[1] = float.Parse(text_Meg_Bias_y.Text);
            node.profiles.MEG_BIAS[2] = float.Parse(text_Meg_Bias_z.Text);
            node.profiles.GYO_FACTOR[0] = float.Parse(text_Gyo_Factor_x.Text);
            node.profiles.GYO_FACTOR[1] = float.Parse(text_Gyo_Factor_y.Text);
            node.profiles.GYO_FACTOR[2] = float.Parse(text_Gyo_Factor_z.Text);
            node.profiles.GYO_BIAS[0] = float.Parse(text_Gyo_Bias_x.Text);
            node.profiles.GYO_BIAS[1] = float.Parse(text_Gyo_Bias_y.Text);
            node.profiles.GYO_BIAS[2] = float.Parse(text_Gyo_Bias_z.Text); 


            if ( cBox_Motion.CheckState == CheckState.Checked)
            {
               
                node.profiles.MOTION_EN = 1;
            }
            else
            {
                node.profiles.MOTION_EN = 0;
            }
            node.profiles.EMG_CHL = 0x00;
            for (int i = 0; i < 4; i++)
            {
                if (checkBoxs[i].CheckState == CheckState.Checked)
                {
                    node.profiles.EMG_CHL |= (byte)(0x01 <<(7 - i));
                }
                else
                {
                    node.profiles.EMG_CHL &= (byte)(~(0x01 << (7 - i) ));
                }
            }

            node.SetProfiles();
            this.Dispose();
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
