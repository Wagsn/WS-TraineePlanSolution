#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Text
* 项目描述 ：
* 类 名 称 ：ConsoleTable
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Core.Text
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/21 21:30:18
* 更新时间 ：2018/11/21 21:30:18
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using System;

namespace WS.Text
{
    /// <summary>
    /// 控制台表格
    /// </summary>
    public class ConsoleTable
    {
        public object[] Head { get; set; }

        public object[][] Body { get; set; }

        private object[][] mat2D;
        
        /// <summary>
        /// 在控制台上显示
        /// </summary>
        public void OutputToConsole()
        {
            Console.WriteLine(Output());
        }

        /// <summary>
        /// 输出成字符串
        /// </summary>
        /// <returns></returns>
        public string Output()
        {
            mat2D = new object[Body.Length + 1][];
            mat2D[0] = Head;
            for (int i = 1; i < mat2D.Length; i++)
            {
                mat2D[i] = Body[i - 1];
            }
            return Grid.ToGrid(mat2D);
        }
    }
}
