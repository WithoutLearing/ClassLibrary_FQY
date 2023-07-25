using System.IO;
using System.Text;

namespace ClassLibrary_FQY
{
    /// <summary>
    /// 文件及文件夹操作类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 创建文件夹并返回路径
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="path">文件夹路径</param>
        /// <returns>返回所新建文件夹的路径</returns>
        public static string CreateFolder(string folderName, string path)
        {
            StringBuilder strdir = new StringBuilder();
            strdir.Append(path);
            strdir.Append(@"\");
            strdir.Append(folderName);
            //在指定路径创建新文件夹，并返回创建后的路径，如果文件夹已经存在，则直接返回路径
            Directory.CreateDirectory(strdir.ToString());
            return strdir.ToString();
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="name">指定文件名（包含路径）</param>
        /// <returns></returns>
        public static FileStream CreateFiles(string name)
        {
            //检索是否已经
            if (File.Exists(name))
            {
                return null;
            }
            else
            {
                FileStream fs = File.Create(name);
                return fs;
            }

        }
    }
}
