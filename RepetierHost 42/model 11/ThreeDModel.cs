using System;//基本类和基类，这些类定义常用的值和引用数据类型、事件和事件处理程序、接口、属性和异常处理
using System.Collections.Generic;//定义泛型集合的接口和类，用户可以使用泛型集合来创建强类型集合，这种集合能提供比非泛型强类型集合更好的类型安全性和性能
using System.Linq;//支持使用语言集成查询 (LINQ) 的查询。这包括具有以下功能的类型：代表查询成为表达式树中的对象。
using System.Text;//包含用于字符编码和字符串操作的类型。还有一个子命名空间能让您使用正则表达式来处理文本。
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using OpenTK;

namespace RepetierHost.model
{   //三维向量类,用来表示位置，旋转度，规模等信息
    public class Coord3D {
        public float x=0,y=0,z=0;
        public Coord3D() { }
        public Coord3D(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
    //3D模型的抽象类，定义了是否被实现，位置，旋转度，规模等信息，要求子类有模型改变变量，实现clear和paint两个函数 
    public abstract class ThreeDModel
    {
        private bool selected = false;
        private Coord3D position = new Coord3D();
        private Coord3D rotation = new Coord3D();
        private Coord3D scale = new Coord3D(1,1,1);

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public Coord3D Position
        {
            get { return position; }
            set { position = value; }
        }
        public Coord3D Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Coord3D Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        //自从上次打印，模型是否被更改
        public virtual bool Changed
        {
            get { return false; }
        }
        public virtual void Clear() {}
        abstract public void Paint();
    }
}
