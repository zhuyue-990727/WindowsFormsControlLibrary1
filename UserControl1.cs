using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;

namespace WindowsFormsControlLibrary1
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdg = new OpenFileDialog();
            if (ofdg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string file = ofdg.FileName;//得到选择bai的文件的完整路径du                
                textBox1.Text = System.IO.File.ReadAllText(file, Encoding.UTF8);//把读出来的数据显示在zhitextbox中            
            }
            }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
namespace GrammerAnalyzer
{
    class Grammer
    {
        StringCollection LeftPartRuleTemp = new StringCollection();
        StringCollection RightPartRuleTemp = new StringCollection();
        StringCollection LeftPartRule = new StringCollection();
        StringCollection RightPartRule = new StringCollection();
        ArrayList VT = new ArrayList();
        ArrayList VN = new ArrayList();
        bool[,] FirstVtTable;
        bool[,] LastVtTable;
        char[,] PreRelationTable;
        StringCollection ProcessStr = new StringCollection();
        Stack stack = new Stack();


        /*构造函数，参数为规则数组，构造函数将规则分为两部分，分别存于LeftPartRuleTemp和RightPartRuleTemp中*/
        public Grammer(String[] GrammerRules)
        {
            foreach (String ruleTemp in GrammerRules)
            {
                if (!ruleTemp.Contains("->")) return;
                else
                    ruleTemp.Replace("->", ">");
                String[] temp = ruleTemp.Split('>');
                LeftPartRule.Add(temp[0]);
                RightPartRule.Add(temp[1]);
            }
        }

        /*测试是否为算符优先文法，若是返回true，否返回false*/
        private bool TestGrammer()
        {
            foreach (String tempRule in RightPartRuleTemp)
            {
                for (int i = 0; i <= tempRule.Length - 2; i++)
                {
                    if (tempRule[i] >= 'A' && tempRule[i] <= 'Z' && tempRule[i + 1] >= 'A' && tempRule[i + 1] <= 'Z') return false;
                }

            }
            return true;
        }

        /*构造语法规则集合（去除|符号）*/
        private void AddToGrammerRules(string LeftPart, string RightPart)
        {
            String partTemp = String.Empty;
            for (int i = 0; i < RightPart.Length - 1; i++)
            {
                char vt = RightPart[i];
                if (vt != '|')
                    partTemp += vt.ToString();
                else
                {
                    RightPartRule.Add(partTemp);
                    LeftPartRule.Add(LeftPart[0].ToString());
                    partTemp = String.Empty;
                }
                if (i == (RightPart.Length - 1))
                {
                    RightPartRule.Add(partTemp);
                    LeftPartRule.Add(LeftPart[0].ToString());
                    partTemp = String.Empty;
                }
            }
        }


        /*构造终结符集合和非终结符集合，输入的字符串应不包括|符号*/
        private void AddToVtVn(string LeftPart, string RightPart)
        {
            if (!VN.Contains(LeftPart[0]))
                VN.Add(LeftPart[0]);
            foreach (char vt in RightPart)
            {
                if ((vt < 'A' && vt > 'Z') && !VT.Contains(vt) && vt != '|')
                    VT.Add(vt);
            }
        }



        /*返回对于输入文法的判断*/
        public string RetJudgement()
        {
            string RetString = string.Empty;
            if (TestGrammer())
            {
                RetString += "这是一个算符优先文法！ \r\n";
                for (int i = 0; i <= LeftPartRuleTemp.Count - 1; i++)
                {
                    AddToGrammerRules(LeftPartRuleTemp[i].ToString(), RightPartRuleTemp[i].ToString());
                    AddToVtVn(LeftPartRuleTemp[i].ToString(), RightPartRuleTemp[i].ToString());
                }
                RetString += "非终结符为： \r\n";
                foreach (char vn in VN)
                    RetString += vn.ToString();
                RetString += "\r\n终结符为： \r\n";
                foreach (char vt in VT)
                    RetString += vt.ToString();
            }
            else RetString += "这不是一个算符优先文法";
            return RetString;
        }

        private void Insert(char Vn, char Vt, bool[,] TempArr)
        {
            int VnNum = VN.IndexOf(Vn);
            int VtNum = VT.IndexOf(Vt);
            if (!TempArr[VnNum, VtNum])
            {
                TempArr[VnNum, VtNum] = true;
                stack.Push(Vn.ToString() + Vt.ToString());
            }
        }


        private string FirstVt() //计算FIRSTVT集合
        {
            string strFirst = "以下为FIRSTVT集合： \r\n";
            stack.Clear();
            FirstVtTable = new bool[VN.Count, VT.Count];
            for (int i = 0; i <= LeftPartRule.Count - 1; i++)
            {
                if (VT.Contains(RightPartRule[i][0]))
                {
                    Insert(LeftPartRule[i][0], RightPartRule[i][0],
                   FirstVtTable);
                }
                else if (RightPartRule[i].Length >= 2 &&
                VT.Contains(RightPartRule[i][1]))
                {
                    Insert(LeftPartRule[i][0], RightPartRule[i][1],
                   FirstVtTable);
                }
            }
            while (stack.Count >= 1)
            {
                string tempQa = stack.Pop().ToString();
                string tempQ = tempQa[0].ToString();
                string tempa = tempQa[1].ToString();
                for (int i = 0; i <= LeftPartRule.Count - 1; i++)
                {
                    if (RightPartRule[i][0].ToString() == tempQ)
                    {
                        Insert(LeftPartRule[i][0], tempa[0],
                       FirstVtTable);
                    }
                }
            }
            for (int i = 0; i <= VN.Count - 1; i++)
            {
                strFirst += "FIRSTVT(" + VN[i].ToString() + ") ={ ";
                for (int j = 0; j <= VT.Count - 1; j++)
                {
                    if (FirstVtTable[i, j] == true)
                        strFirst += VT[j].ToString() + " ";
                }
                strFirst += "} \r\n";
            }
            return strFirst;
        }
        private string LastVt()
        {
            string strLast = "以下为LASTVT集合： \r\n";
            stack.Clear();
            LastVtTable = new bool[VN.Count, VT.Count];
            for (int i = 0; i <= LeftPartRule.Count - 1; i++)
            {
                int j = RightPartRule[i].Length;
                if (VT.Contains(RightPartRule[i][j - 1]))
                {
                    Insert(LeftPartRule[i][0], RightPartRule[i][j - 1],
                  LastVtTable);
                }
                else if (RightPartRule[i].Length >= 2 &&
                VT.Contains(RightPartRule[i][j - 2]))
                {
                    Insert(LeftPartRule[i][0], RightPartRule[i][j - 2],
                  LastVtTable);
                }
            }
            while (stack.Count >= 1)
            {
                string tempQa = stack.Pop().ToString();
                string tempQ = tempQa[0].ToString();
                string tempa = tempQa[1].ToString();
                for (int i = 0; i <= LeftPartRule.Count - 1; i++)
                {
                    int k = RightPartRule[i].Length;
                    if (RightPartRule[i][k - 1].ToString() == tempQ)
                    {
                        Insert(LeftPartRule[i][0], tempa[0], LastVtTable);
                    }
                }
            }
            for (int i = 0; i <= VN.Count - 1; i++)
            {
                strLast += "LASTVT(" + VN[i].ToString() + ") ={ ";
                for (int j = 0; j <= VT.Count - 1; j++)
                {
                    if (LastVtTable[i, j] == true)
                        strLast += VT[j].ToString() + " ";
                }
                strLast += "} \r\n";
            }
            return strLast;
        } //计算LASTVT集合
        public string RetFirstLastVt()
        {
            return FirstVt() + "\r\n\r\n" + LastVt();
        }
        private void RetPreRelationTable()
        {
            PreRelationTable = new char[VT.Count, VT.Count];
            foreach (string tempRule in RightPartRule)
            {
                for (int i = 0; i <= tempRule.Length - 2; i++)
                {
                    if (VT.Contains(tempRule[i]) && VT.Contains(tempRule[i
                   + 1]))
                    {
                        PreRelationTable[VT.IndexOf(tempRule[i]),
                      VT.IndexOf(tempRule[i + 1])] = '=';
                    }
                    if (i <= tempRule.Length - 3 &&
                    VT.Contains(tempRule[i]) && !VT.Contains(tempRule[i + 1]) &&
                    VT.Contains((tempRule[i + 2])))
                    {
                        PreRelationTable[VT.IndexOf(tempRule[i]),
                       VT.IndexOf(tempRule[i + 2])] = '=';
                    }
                    if (VT.Contains(tempRule[i]) && !VT.Contains(tempRule[i + 1]))
                    {
                        for (int j = 0; j <= VT.Count - 1; j++)
                        {
                            if (FirstVtTable[VN.IndexOf(tempRule[i + 1]),
                          j] == true)
                                PreRelationTable[VT.IndexOf(tempRule[i]), j] = '<';
                        }
                    }
                    if (!VT.Contains(tempRule[i]) && VT.Contains(tempRule[i + 1]))
                    {
                        for (int j = 0; j <= VT.Count - 1; j++)
                        {
                            if (LastVtTable[VN.IndexOf(tempRule[i]), j] == true)
                                PreRelationTable[j,
                                VT.IndexOf(tempRule[i + 1])] = '>';
                        }
                    }
                }
            }
        }
        public StringCollection DrawPreRelationTable()
        {
            RetPreRelationTable();
            StringCollection tableStr = new StringCollection();
            string tempstring = " \t";
            for (int i = 0; i <= VT.Count - 1; i++)
            {
                tempstring += VT[i].ToString() + "\t";
            }
            tableStr.Add(tempstring);
            for (int i = 0; i <= VT.Count - 1; i++)
            {
                tempstring = string.Empty;
                tempstring += VT[i].ToString() + "\t";
                for (int j = 0; j <= VT.Count - 1; j++)
                {
                    tempstring += PreRelationTable[i, j].ToString() + "\t";
                }
                tableStr.Add(tempstring);
            }
            return tableStr;
        }
        private bool AnalysisProcess(string InputSentence) 
        {
            return true;
        }
private string TraceBack(string s, int m, int n)
        {
            return s;
        }
 
public StringCollection RetAnalysisProcess(string InputSentence)
        {
            ProcessStr.Clear();
            if (AnalysisProcess(InputSentence))
                return ProcessStr;
            else
            {
                ProcessStr.Add("error");
                MessageBox.Show("在输入的句子中发现语法错误！","提示！");
            return ProcessStr;
            }
        }
    }
}