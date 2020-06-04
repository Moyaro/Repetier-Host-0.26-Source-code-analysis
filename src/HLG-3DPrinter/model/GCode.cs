using System;//System 命名空间包含基本类和基类，这些类定义常用的值和引用数据类型、事件和事件处理程序、接口、属性和异常处理。
using System.IO;//System.IO 命名空间包含具有以下功能的类型：支持输入和输出，包括以同步或异步方式在流中读取和写入数据、压缩流中的数据、创建和使用独立存储区、
//将文件映射到应用程序的逻辑地址空间、将多个数据对象存储在一个容器中、使用匿名或命名管道进行通信、实现自定义日志记录，以及处理出入串行端口的数据流。
using System.Collections.Generic;//System.Collections.Generic 命名空间包含定义泛型集合的接口和类，用户可以使用泛型集合来创建强类型集合，
//这种集合能提供比非泛型强类型集合更好的类型安全性和性能
using System.Linq;//System.Linq 命名空间包含具有以下功能的类型：支持使用语言集成查询 (LINQ) 的查询。这包括具有以下功能的类型：代表查询成为表达式树中的对象。
//学习：https://docs.microsoft.com/zh-cn/dotnet/standard/using-linq    
using System.Text;//System.Text 命名空间包含用于字符编码和字符串操作的类型。还有一个子命名空间能让您使用正则表达式来处理文本。
using System.Globalization;//System.Globalization 命名空间包含定义区域性相关信息的类，这些信息包括语言，国家/地区，正在使用的日历，日期、货币和数字的格式模式，以及字符串的排序顺序。
//这些类对于编写全球化（国际化）应用程序很有用。而像 StringInfo 和 TextInfo 这样的类更是为我们提供了诸如代理项支持和文本元素处理等高级全球化功能
namespace RepetierHost.model
{
    ///以一种简单的数据结构存储gcode命令的完整数据
    ///这种结构可以被转换为二进制或ASCII形式，然后发送给reprap打印机。
    ///外部调用顺序：gcode命令分析，转换
	///public void Parse(String line)
    ///public String getAscii(bool inclLine,bool inclChecksum)，是否倾斜，是否满足校验
	///public byte[] getBinary(int version),这个参数有点怪
    public class GCode
    {
        public static NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;//当前区域的时间格式
        //提供用于对数字值进行格式设置和分析的区域性特定信息。后者：获取不依赖于区域性（固定）的 CultureInfo 对象。
        private ushort fields = 128;
        int n;
        private byte g = 0, m = 0, t = 0;
        private float x, y, z, e, f;
        private int s;
        private int p;
        private String text = null;

        public bool hasText { get { return (fields & 32768) != 0; } }
        public String Text
        {
            get { return text; }
            set { text = value; if (text.Length > 16) text = text.Substring(0, 16); fields |= 32768; }
        }

        //MNGTSPXYZEF? EFG MN P ST XYZ  11个数？gcode数据只由这几个数组成
        //G-CODE语言的命令通常是一个英文字母（A-Z）+数字的方式表示，在3D打印机的控制中，常用的字母包括G - 用来控制运动和位置, T - 控制工具
        //M - 一些辅助命令，X - x轴上的变化，Y - y轴上的变化，E - 挤出量，F - 打印头的速度。
        //https://www.cnblogs.com/mazhenyu/p/10711939.html
        public bool hasN { get { return (fields & 1) != 0; } }
        public int N
        {
            get { return n; }
            set { n = value; fields |= 1; }
        }
        public bool hasM {get { return (fields & 2) != 0; }}
        public byte M
        {
            get { return m; }
            set { m = value; fields |= 2; }
        }
        public bool hasG { get { return (fields & 4) != 0; } }
        public byte G
        {
            get { return g; }
            set { g = value; fields |= 4; }
        }
        public bool hasT { get { return (fields & 512) != 0; } }
        public byte T
        {
            get { return t; }
            set { t = value; fields |= 512; }
        }
        public bool hasS { get { return (fields & 1024) != 0; } }
        public int S
        {
            get { return s; }
            set { s = value; fields |= 1024; }
        }
        public bool hasP { get { return (fields & 2048) != 0; } }
        public int P
        {
            get { return p; }
            set { p = value; fields |= 2048; }
        }

        public bool hasX { get { return (fields & 8) != 0; } }
        public float X
        {
            get { return x; }
            set { x=value;fields|=8;}
        }
        public bool hasY { get { return (fields & 16) != 0; } }
        public float Y
        {
            get { return y; }
            set { y = value; fields |= 16; }
        }
        public bool hasZ { get { return (fields & 32) != 0; } }
        public float Z
        {
            get { return z; }
            set { z = value; fields |= 32; }
        }
        public bool hasE { get { return (fields & 64) != 0; } }
        public float E
        {
            get { return e; }
            set { e = value; fields |= 64; }
        }
        public bool hasF { get { return (fields & 256) != 0; } }
        public float F
        {
            get { return f; }
            set { f = value; fields |= 256; }
        }

        ///将gcode数据转换为二进制表示形式
        public byte[] getBinary(int version)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms,Encoding.ASCII);
            bw.Write(fields);
            if (hasN) bw.Write((ushort)(n & 65535));
            if (hasM) bw.Write(m);
            if (hasG) bw.Write(g);
            if (hasX) bw.Write(x);
            if (hasY) bw.Write(y);
            if (hasZ) bw.Write(z);
            if (hasE) bw.Write(e);
            if (hasF) bw.Write(f);
            if (hasT) bw.Write(t);
            if (hasS) bw.Write(s);
            if (hasP) bw.Write(p);
            if (hasText)
            {
                int i, len = text.Length;
                if (len > 16) len = 16;
                for (i = 0; i < len; i++)
                {
                    bw.Write((byte)text[i]);
                }
                for(;i<16;i++) bw.Write((byte)0);
            }
            //计算fletcher-16校验和
            int sum1 = 0, sum2 = 0;
            bw.Flush();
            ms.Flush();
            byte[] buf = ms.ToArray();
            foreach (byte c in buf)
            {
                sum1 = (sum1 + c) % 255;
                sum2 = (sum2 + sum1) % 255;
            }
            bw.Write((byte)sum1);
            bw.Write((byte)sum2);
            bw.Close();
            ms.Flush();
            return ms.ToArray();
        }
        //用将gcode数据转换为ASCII码形式
        public String getAscii(bool inclLine,bool inclChecksum)
        {
            StringBuilder s = new StringBuilder();
            //StringBuilder.Append向此实例追加指定对象的字符串表示形式
            if (inclLine && hasN)
            {
                s.Append("N");
                s.Append(n);
                s.Append(" ");
            }
            if (hasM)
            {
                s.Append("M");
                s.Append(m);
            }
            if (hasG)
            {
                s.Append("G");
                s.Append(g);
            }
            if (hasT)
            {
                if (hasM) s.Append(" ");
                s.Append("T");
                s.Append(t);
            }
            if (hasX)
            {
                s.Append(" X");
                s.Append(x.ToString(format));
            }
            if (hasY)
            {
                s.Append(" Y");
                s.Append(y.ToString(format));
            }
            if (hasZ)
            {
                s.Append(" Z");
                s.Append(z.ToString(format));
            }
            if (hasE)
            {
                s.Append(" E");
                s.Append(e.ToString(format));
            }
            if (hasF)
            {
                s.Append(" F");
                s.Append(f.ToString(format));
            }
            if (hasS)
            {
                s.Append(" S");
                s.Append(this.s);
            }
            if (hasP)
            {
                s.Append(" P");
                s.Append(p);
            }
            if (hasText)
            {
                s.Append(" ");
                s.Append(text);
            }
            if (inclChecksum)
            {
                int check = 0;
                foreach (char ch in s.ToString()) check ^= (ch & 0xff);//666
                check ^= 32;
                s.Append(" *");
                s.Append(check);
            }
            return s.ToString();
        }
        //对传进来的gcode命令进行语法分析
        public void Parse(String line)
        {
            fields = 128;
            int iv;
            float fv;
            if (line.IndexOf(';') >= 0) line = line.Substring(0, line.IndexOf(';')); // 删除评论？
            if (ExtractInt(line, "G", out iv)) G = (byte)iv;
            if (ExtractInt(line, "M", out iv)) M = (byte)iv;
            if (ExtractInt(line, "T", out iv)) T = (byte)iv;
            if (ExtractInt(line, "S", out iv)) S = iv;
            if (ExtractInt(line, "P", out iv)) P = iv;
            if (ExtractFloat(line, "X", out fv)) X = fv;
            if (ExtractFloat(line, "Y", out fv)) Y = fv;
            if (ExtractFloat(line, "Z", out fv)) Z = fv;
            if (ExtractFloat(line, "E", out fv)) E = fv;
            if (ExtractFloat(line, "F", out fv)) F = fv;
            if (hasM && (m == 23 || m == 28 || m == 29))
            {
                int pos = line.IndexOf('M') + 3;
                while (pos < line.Length && char.IsWhiteSpace(line[pos])) pos++;
                int end = pos;
                while (end < line.Length && !char.IsWhiteSpace(line[end])) end++;
                Text = line.Substring(pos, end - pos);
            }
        }
        //提取整数
        private bool ExtractInt(string s,string code,out int value) {
            value = 0;
            int p = s.IndexOf(code);
            if (p < 0) return false;
            p++;
            int end = p;
            while(end<s.Length && char.IsDigit(s[end])) end++;
            int.TryParse(s.Substring(p,end-p),out value);
            return true;
        }
        //提取浮点数
        private bool ExtractFloat(string s, string code, out float value)
        {
            value = 0;
            int p = s.IndexOf(code);
            if (p < 0) return false;
            p++;
            int end = p;
            while (end < s.Length && !char.IsWhiteSpace(s[end])) end++;

            float.TryParse(s.Substring(p, end - p), NumberStyles.Float, format, out value);
            return true;
        }
        //重写使其返回该对象的ASCII码
        //Object.ToString 是 .NET Framework 中的主要格式设置方法。 它将对象转换为其字符串表示形式，以便它适合于显示。
        public override string ToString()
        {
            return getAscii(true,true);
        }
    }
}
