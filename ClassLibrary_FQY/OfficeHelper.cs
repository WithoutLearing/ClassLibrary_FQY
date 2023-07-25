
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ClassLibrary_FQY
{
    /// <summary>
    /// Office使用类
    /// 需要引用microsoft.office.interop.excel
    /// 
    /// </summary>
    public class OfficeHelper
    {
        /// <summary>
        /// 当前应用的表格文件
        /// </summary>
        public static IWorkbook workbook;
        /// <summary>
        /// 当前sheet名称
        /// </summary>
        public static ISheet sheet;

        /// <summary>
        /// 将dataGridView导出为Excel
        /// </summary>
        /// <param name="myDGV">dataGridView名称</param>
        /// <param name="filename">导出路径</param>
        public static void ExportExcel(DataGridView myDGV, string filename)
        {
            string saveFilename = filename;
            if (saveFilename == null || saveFilename.IndexOf(":") < 0)
            {
                return;
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1

            //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }

            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应


            if (saveFilename != null)
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFilename);
                    //  MessageBox.Show("文件 " + filename + "保存成功", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    //  MessageBox.Show("导出文件时出错，文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();
        }
        /// <summary>
        /// 创建新的本地表格
        /// </summary>
        /// <param name="bookName">表名</param>
        /// <param name="sheetName">sheet名</param>
        /// <returns>返回创建好的文件流</returns>
        public static FileStream CreateLocalExcel(string bookName, string sheetName)
        {
            IWorkbook wb;
            if (Path.GetExtension(bookName) == ".xls")
            {
                wb = new HSSFWorkbook();
            }
            else
            {
                wb = new XSSFWorkbook();
            }

            ISheet st = wb.CreateSheet(sheetName);
            using (FileStream fs = File.OpenWrite(bookName))
            {
                wb.Write(fs);
                return fs;
            }
        }
        /// <summary>
        /// 创建新的本地表格
        /// </summary>
        /// <param name="bookName">表名</param>
        /// <returns>返回创建好的文件流</returns>
        public static FileStream CreateLocalExcel(string bookName)
        {
            if (File.Exists(bookName))
            {
                using (FileStream fs = File.OpenWrite(bookName))
                {
                    return fs;
                }
            }
            else
            {
                IWorkbook wb;
                if (Path.GetExtension(bookName) == ".xls")
                {
                    wb = new HSSFWorkbook();
                }
                else
                {
                    wb = new XSSFWorkbook();
                }
                ISheet st = wb.CreateSheet("sheet1");
                using (FileStream fs = File.OpenWrite(bookName))
                {
                    wb.Write(fs);
                    return fs;
                }
            }
           
        }
        /// <summary>
        /// 读取本地excel
        /// </summary>
        /// <param name="path">本地表格路径</param>
        /// <param name="index">sheet索引，从0开始</param>
        /// <returns>读取成功返回0</returns>
        public static int ReadLocalExcel(string path, int index)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (Path.GetExtension(path) == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = new XSSFWorkbook(fs);
                }
                //获取工作簿的sheet数量
                int sheetNum = workbook.NumberOfSheets;
                if (sheetNum > index)
                {
                    sheet = workbook.GetSheetAt(index);
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }

        }
        /// <summary>
        /// 获取所选sheet
        /// </summary>
        /// <param name="path">本地表格路径</param>
        /// <param name="name">要选取的sheet名称</param>
        /// <returns></returns>
        public static int GetSheet(string path, string name)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (Path.GetExtension(path) == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = new XSSFWorkbook(fs);
                }
                //获取工作簿的sheet数量
                int sheetNum = workbook.NumberOfSheets;
                if (sheetNum > 0)
                {
                    sheet = workbook.GetSheet(name);
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }
        /// <summary>
        /// 获取所选列的值
        /// </summary>
        /// <param name="sheet">所选的sheet名称</param>
        /// <param name="cellnum">列的索引，从0开始</param>
        /// <returns></returns>
        public static string[] GetCell(ISheet sheet, int cellnum)
        {
            ICell cell;
            string[] strCellsValue;
            //获取当前sheet的行数（有数据的行数）
            int rowsNum = sheet.PhysicalNumberOfRows;
            if (rowsNum > 0)
            {
                strCellsValue = new string[rowsNum - 1];
                //选中标题行
                IRow row = sheet.GetRow(0);
                //获取当前sheet的列数（有数据的列数）
                int CellsNum = row.PhysicalNumberOfCells;
                if (CellsNum > cellnum)
                {
                    for (int i = 1; i < rowsNum; i++)
                    {
                        cell = sheet.GetRow(i).GetCell(cellnum);
                        if (cell != null)
                        {
                            switch (cell.CellType)
                            {
                                case CellType.Unknown:
                                    break;
                                case CellType.Numeric:
                                    strCellsValue[i - 1] = cell.NumericCellValue.ToString();
                                    break;
                                case CellType.String:
                                    strCellsValue[i - 1] = cell.StringCellValue;
                                    break;
                                case CellType.Formula:
                                    break;
                                case CellType.Blank:
                                    strCellsValue[i - 1] = null;
                                    break;
                                case CellType.Boolean:
                                    break;
                                case CellType.Error:
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            strCellsValue[i - 1] = null;
                        }

                    }
                }
                return strCellsValue;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 向表格中插入数据
        /// </summary>
        /// <param name="bookName">表名</param>
        /// <param name="cellTitleVlaue">标题行写入值</param>
        /// <param name="cellDataVlaue">数据行写入值</param>
        public static void InsertDataLocalExcel(string bookName, List<string> cellTitleVlaue, List<string> cellDataVlaue)
        {
            FileStream fs = new FileStream(bookName, FileMode.Open, FileAccess.Read);
            if (Path.GetExtension(bookName) == ".xls")
            {
                workbook = new HSSFWorkbook(fs);
            }
            else
            {
                workbook = new XSSFWorkbook(fs);
            }
            //判断工作簿中是否有工作表
            if (workbook.NumberOfSheets > 0)
            {
                sheet = workbook.GetSheetAt(0);//选择工作簿中的第一张工作表                           
                //获取当前sheet的行数（有数据的行数）,判断选中的表中是否有数据
                if (sheet.PhysicalNumberOfRows > 0)
                {
                    IRow row = sheet.CreateRow(sheet.PhysicalNumberOfRows);//创建数据行
                    for (int i = 0; i < cellDataVlaue.Count; i++)
                    {
                        row.CreateCell(i).SetCellValue(cellDataVlaue[i]);

                    }
                }
                else
                {
                    IRow row = sheet.CreateRow(0);//创建标题行
                    for (int i = 0; i < cellTitleVlaue.Count; i++)
                    {
                        row.CreateCell(i).SetCellValue(cellTitleVlaue[i]);

                    }
                }

                fs = new FileStream(bookName, FileMode.Open, FileAccess.Write);//配置文件流为写入
                workbook.Write(fs);//向表中写入数据
                fs.Close();//关闭文件
                workbook.Close();//关闭工作簿
            }
        }
    }
}
