using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Local
{
    //class可以看做文件夹 instance看做语言文件

    /// <summary>
    /// 语言种类，可以注册
    /// </summary>
    public class LocalizationClass
    {
        //类型别名
        public string Label { get; set; }
        public string Class { get; set; }
        public string Extends { get; set; }
        public string Locale { get; set; }
        //特化版本
        public string Special { get; set; }
    }
}
