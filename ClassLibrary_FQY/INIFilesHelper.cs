using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ClassLibrary_FQY
{
    /// <summary>
    /// INI文件操作类
    /// </summary>
    public class INIFilesHelper
    {
        private string inipath;
        //声明API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 构造函数,初始化
        /// </summary>
        /// <param name="path"></param>
        public INIFilesHelper(string path)
        {
            inipath = path;//定义ini文件路径
        }

        #region 非静态方法
        /// <summary>
        /// 写入INI文件 
        /// </summary>
        /// <param name="Section">字段名称</param>
        /// <param name="Key">键名</param>
        /// <param name="Value">键值</param>
        /// <returns>是否写入成功</returns>
        public bool IniWriteValue(string Section, string Key, string Value)
        {
            return WritePrivateProfileString(Section, Key, Value, inipath);
        }

        /// <summary>
        ///  读出INI文件 
        /// </summary>
        /// <param name="Section">字段名称</param>
        /// <param name="Key">键名</param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "default", temp, 500, inipath);
            return temp.ToString();
        }

        /// <summary>
        /// 验证文件是否存在 
        /// </summary>
        /// <returns></returns>
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }

        #endregion

        #region 静态方法
        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="Section">字段名称</param>
        /// <param name="Key">键名</param>
        /// <param name="Value">键值</param>
        /// <param name="path">路径名称</param>
        /// <returns>是否写入成功</returns>
        public static bool IniWriteValue(string Section, string Key, string Value, string path)
        {
            return WritePrivateProfileString(Section, Key, Value, path);
        }

        /// <summary>
        /// 读出INI文件
        /// </summary>
        /// <param name="Section">字段名称</param>
        /// <param name="Key">键名</param>
        /// <param name="def">默认名称</param>
        /// <param name="path">路径名称</param>
        /// <returns></returns>
        public static string IniReadValue(string Section, string Key, string path, string def = "default")
        {
            StringBuilder temp = new StringBuilder(500);
            GetPrivateProfileString(Section, Key, def, temp, 500, path);
            return temp.ToString();
        }

        /// <summary>
        /// 验证文件是否存在 
        /// </summary>
        /// <returns></returns>
        public static bool ExistINIFile(string path)
        {
            return File.Exists(path);
        }
        #endregion


    }
}
