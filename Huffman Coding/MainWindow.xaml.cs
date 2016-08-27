using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;


namespace Huffman_Coding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class node
        {
            public int weight;
            public node lchild;
            public node rchild;
           public int nodeid;
           static int Nodeid = 0;
            public node(int w=0,node l=null,node r=null)
            {
                lchild = l;
                rchild = r;
                nodeid = Nodeid;
                Nodeid++;
                weight = (l!=null && r!=null)?l.weight + r.weight : w;

            }
            
        }

        private void pdfs(node a )
        {
            Console.Write("(");
            if(a.lchild!=null)
            pdfs(a.lchild);
            if(a.rchild!=null)
            pdfs(a.rchild);
            Console.Write(a.weight);
            Console.Write(")");
        }

        private string dictToHead(Dictionary<char, int> source)
        {
            string dest = "";
            foreach(var item in source.Keys)
            {
                dest += item;
                dest += (char)source[item];
            }
            dest += "'''";
    
            return dest;
        }


        private string HuffConvert(string source)
        {


            Dictionary<char, int> dictFrequence = new Dictionary<char, int>();

            for (int i = 0; i < source.Length; i++)
            {
                if (dictFrequence.ContainsKey(source[i]))
                {
                    dictFrequence[source[i]]++;
                }else
                {
                    dictFrequence.Add(source[i], 1);
                }
            }
            
            Dictionary<node, KeyValuePair<char, int>> dictNodeToPair = new Dictionary<node, KeyValuePair<char, int>>();
            foreach (var item in dictFrequence)
            {
                dictNodeToPair.Add(new node(item.Value), item);
            }
            List<node> liCurrentProcessNode = new List<node>();

            foreach(var item in dictNodeToPair.Keys)
            {
                liCurrentProcessNode.Add(item);
            }


            while (liCurrentProcessNode.Count >= 2)
            {
                var dicSort = from objDic in liCurrentProcessNode orderby objDic.weight ascending select objDic;
                //var count = dicSort.Count();
                //var f1 = dicSort.ElementAt(0);
                //liCurrentProcessNode.Remove(f1);
                //var f2 = dicSort.ElementAt(1);
                //liCurrentProcessNode.Remove(f2);
                var f1 = dicSort.ElementAt(0);
                var f2 = dicSort.ElementAt(1);
                liCurrentProcessNode.Remove(f1);
                liCurrentProcessNode.Remove(f2);
                var f3 = new node(0, f1, f2);
                liCurrentProcessNode.Add(f3);
            }

            Stack<int> dfsInt = new Stack<int>();
            Stack<node> dfsHaff = new Stack<node>(); 
            dfsInt.Push(0);
            dfsHaff.Push(liCurrentProcessNode[0]);
            int curInt = 1;


            pdfs(dfsHaff.Peek());

            Dictionary<char, int> dictCharToInt = new Dictionary<char, int>();
            while(dfsHaff.Count > 0 )
            {
                node curnode = dfsHaff.Pop();
                if (curnode.lchild != null)
                {
                    var c = curnode.lchild;
                    curnode.lchild = null;
                    dfsHaff.Push(curnode);
                    dfsHaff.Push(c);
                    curInt = (curInt << 1) + 1;
                    continue;
                }
                else if(curnode.rchild!= null)
                {
                    var c = curnode.rchild;
                    curnode.rchild = null;
                    dfsHaff.Push(curnode);
                    dfsHaff.Push(c);
                    curInt = (curInt <<1);
                    continue;
                }
                else
                {
                    if(dictNodeToPair.ContainsKey(curnode))
                        dictCharToInt.Add(dictNodeToPair[curnode].Key,curInt);
                    curInt = curInt >> 1;
                }

            }

            foreach (var item in dictCharToInt)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }

            string dest = "";
            char curchar=(char)0;
            int bitCount = 0;
            foreach(var item in source)
            {
                int bit = dictCharToInt[item];
                for (int i = 31; i >=0; i--)
                {
                    if((bit & (1<<i))!= 0 )
                    {
                        for(int j =i-1;j>=0;j--)
                        {
                            if(bitCount>15)
                            {
                                bitCount = 0;
                                dest = dest + curchar;
                                curchar = '\0';
                            }
                            if((bit&(1<<j))!=0)
                            {
                                curchar = (char) (curchar | (char)(1 << (15 - bitCount)));

                            }

                            bitCount++;

                        }

                        break;
                    }
                }
            }
            dest += curchar;

            foreach (var item in dest)
            {
                Console.WriteLine((int)item);
            }

            Console.WriteLine("dictchartoInt");
            int turn = 0;
            foreach(var i in dictToHead(dictCharToInt))
            {
                if(turn==0)
                {
                    Console.Write(i);
                    turn = 1;
                }else
                {
                    turn = 0;
                    Console.Write((int)i);
                }
            }
            
            


            

            return (char)bitCount + dictToHead(dictCharToInt) + dest;
            
        }

        private string HuffRevert(string source)
        {
            string dest = "";
            int bitcount = (int)source[0];

            int headcount = source.IndexOf("'''");

            Dictionary<int, char> dictCodeToChar = new Dictionary<int, char>();

            for(int i =1;i<headcount;i+=2)
            {
                char curchar = source[i];
                int curcode = source[i + 1];
                dictCodeToChar.Add(curcode,curchar);
            }


            int keycode = 1;
            for(long i = (headcount+3)*16;i<source.Length*16 ;i++)
            {
                int charnum =(int) (i / 16);
                int charbit = 15 - (int)(i % 16);
                if(charnum +1 >=source.Length && (15- charbit) >= bitcount )
                {
                    if (dictCodeToChar.ContainsKey(keycode)) dest += dictCodeToChar[keycode];
                    break;
                }

                if (dictCodeToChar.ContainsKey(keycode))
                {
                  
                    dest += dictCodeToChar[keycode];
                    keycode = 1;
                }

                if ((source[charnum] & (1 << charbit)) != 0)

                    keycode = (keycode << 1) + 1;
                else
                    keycode = keycode << 1;

            }

            return dest;


        }

        private void btn_Convert_Click(object sender, RoutedEventArgs e)
        {
            string sSource = txt_Source.Text;
            string dest = HuffConvert(sSource);
            Console.WriteLine("converted line");
            foreach(var item in dest)
            {
                Console.WriteLine((int)item);
            }

            txt_Dest.Text =  dest;

            lbl_DestByte.Content = dest.Length;

        }

        private void btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "文本文件|*.*";
            if (dialog.ShowDialog() == false)
            {
                return;
            }
            string s = File.ReadAllText(dialog.FileName, Encoding.GetEncoding("GB18030"));
            lbl_SourceByte.Content = s.Length;
            txt_Source.Text = s;
        }



        private void btn_Revert_Click(object sender, RoutedEventArgs e)
        {
            txt_Source.Text = HuffRevert(txt_Dest.Text);
        }
    }
}
