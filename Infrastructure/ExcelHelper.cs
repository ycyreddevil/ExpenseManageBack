using System;
using System.Web;
using System.Data;
using System.IO;
using System.Text;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.Util;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ExpenseManageBack.Infrastructure
{
    public class ExcelHelper
    {
        //public static DataTable Import(HttpRequest request)
        //{
        //    DataTable dt = null;
        //    HttpFileCollection httpFileCollection = request.Files;
        //    HttpPostedFile file = null;
        //    if (httpFileCollection.Count > 0)
        //    {
        //        file = httpFileCollection[0];
        //        //Excel读取
        //        dt = Import(file.InputStream, 0, 0);
        //    }
        //    return dt;
        //    //    return null;
        //}

        //public static DataTable Import(HttpRequest request, int headerIndex)
        //{
        //    DataTable dt = null;
        //    HttpFileCollection httpFileCollection = request.Files;
        //    HttpPostedFile file = null;
        //    if (httpFileCollection.Count > 0)
        //    {
        //        file = httpFileCollection[0];
        //        //Excel读取
        //        dt = Import(file.InputStream, 0, headerIndex);
        //    }
        //    return dt;
        //    //    return null;
        //}

        public static DataTable Import(Stream fileStream)
        {
            return Import(fileStream, "0", 0);
        }

        public static DataTable Import(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            XSSFWorkbook wbXssf = null;
            HSSFWorkbook wbHssf = null;
            ISheet sheet = null;
            DataTable dt = null;
            try
            {
                wbXssf = new XSSFWorkbook(ExcelFileStream);
                sheet = wbXssf.GetSheet(SheetName);
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Wrong Local header signature"))
                {
                    wbHssf = new HSSFWorkbook(ExcelFileStream);
                    sheet = wbHssf.GetSheet(SheetName);
                }
            }
            finally
            {
                dt = Import(sheet, HeaderRowIndex);
                wbXssf = null;
                wbHssf = null;
                sheet = null;
                ExcelFileStream.Close();
            }
            return dt;
        }

        public static DataTable Import(ISheet sheet, int HeaderRowIndex)
        {
            if (sheet == null)
                return null;

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(HeaderRowIndex);

            int index = 1;
            foreach (ICell cell in headerRow.Cells)
            {
                DataColumn column = new DataColumn(cell.ToString().Trim());
                if (!table.Columns.Contains(column.ColumnName))
                {
                    table.Columns.Add(column);
                }
                else
                {
                    column.ColumnName = column.ColumnName + index;
                    table.Columns.Add(column);

                    index++;
                }
            }

            int cellCount = table.Columns.Count;

            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                DataRow dataRow = table.NewRow();
                bool AllCellIsBlank = true;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell != null)
                    {
                        //读取Excel格式，根据格式读取数据类型
                        switch (cell.CellType)
                        {
                            case CellType.Blank: //空数据类型处理
                                dataRow[j] = "";
                                break;
                            case CellType.String: //字符串类型
                                dataRow[j] = cell.StringCellValue;
                                if (!string.IsNullOrEmpty(cell.StringCellValue))
                                {
                                    dataRow[j] = cell.StringCellValue.Trim();
                                    AllCellIsBlank = false;
                                }
                                else
                                    dataRow[j] = null;
                                break;
                            case CellType.Numeric: //数字类型                                   
                                                   //if (HSSFDateUtil.IsValidExcelDate(cell.NumericCellValue))
                                                   //{
                                                   //    dataRow[j] = cell.DateCellValue;
                                                   //}
                                                   //else
                                                   //{
                                                   //    dataRow[j] = cell.NumericCellValue;
                                                   //}
                                                   //short format = cell.CellStyle.DataFormat;
                                                   //if (format == 14 || format == 22 || format == 31 || format == 57
                                                   // || format == 58 || format == 178 || format == 179)//日期格式
                                                   //{
                                                   //    DateTime date = cell.DateCellValue;
                                                   //    dataRow[j] = date.ToString("yyy-MM-dd HH:mm:ss");
                                                   //}
                                                   //else if (format == 20 || format == 32)//时间格式
                                                   //{
                                                   //    DateTime date = cell.DateCellValue;
                                                   //    dataRow[j] = date.ToString("HH:mm:ss");
                                                   //}
                                                   //try
                                                   //{
                                                   //    dataRow[j] = cell.DateCellValue.ToString("yyy-MM-dd HH:mm:ss");
                                                   //}
                                                   //catch
                                                   //{
                                                   //    dataRow[j] = cell.NumericCellValue;
                                                   //}
                                                   //DateTime date;
                                                   //if (DateTime.TryParse(cell.ToString(), out date))
                                                   //{
                                                   //dataRow[j] = cell.DateCellValue.ToString("yyy-MM-dd HH:mm:ss");
                                                   //}
                                                   //else
                                                   //{
                                dataRow[j] = cell.NumericCellValue;
                                //}
                                AllCellIsBlank = false;
                                break;
                            case CellType.Formula:
                                //HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
                                //dataRow[j] = e.Evaluate(cell).StringValue;

                                //if (!string.IsNullOrEmpty(e.Evaluate(cell).StringValue))
                                //{
                                //    AllCellIsBlank = false;
                                //}
                                switch (row.GetCell(j).CachedFormulaResultType)
                                {
                                    case CellType.String:
                                        string strFORMULA = row.GetCell(j).StringCellValue;
                                        if (strFORMULA != null && strFORMULA.Length > 0)
                                        {
                                            dataRow[j] = strFORMULA.ToString();
                                        }
                                        else
                                        {
                                            dataRow[j] = null;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    //case HSSFCellType.ERROR:
                                    //    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                    //    break;
                                    default:
                                        dataRow[j] = "";
                                        break;
                                }
                                break;
                            default:
                                dataRow[j] = "";
                                break;
                        }
                    }
                }
                if (!AllCellIsBlank)//如果每个cell都是空值则不保存到dataTable
                {
                    table.Rows.Add(dataRow);
                }

            }

            return table;
        }

        public static DataTable Import(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            XSSFWorkbook wbXssf = null;
            HSSFWorkbook wbHssf = null;
            ISheet sheet = null;
            DataTable dt = null;
            try
            {
                wbXssf = new XSSFWorkbook(ExcelFileStream);
                sheet = wbXssf.GetSheetAt(SheetIndex);
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Wrong Local header signature"))
                {
                    wbHssf = new HSSFWorkbook(ExcelFileStream);
                    sheet = wbHssf.GetSheetAt(SheetIndex);
                }
            }
            finally
            {
                //CostSharingHelper.importCostSharing(sheet, HeaderRowIndex, true);
                dt = Import(sheet, HeaderRowIndex);
                //dt = CostSharingHelper.importCostSharing(dt, true);
                wbXssf = null;
                wbHssf = null;
                sheet = null;
                ExcelFileStream.Close();
            }

            return dt;
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="strFileName"></param>
        //public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, string[] chineseHeaders)
        //{

        //    HttpContext curContext = HttpContext.Current;

        //    // 设置编码和附件格式
        //    curContext.Response.ContentType = "application/vnd.ms-excel";
        //    curContext.Response.ContentEncoding = Encoding.UTF8;
        //    curContext.Response.Charset = "";
        //    curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

        //    curContext.Response.BinaryWrite(Export(dtSource, strHeaderText, chineseHeaders).GetBuffer());
        //    curContext.Response.End();
        //}

        //public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, string title)
        //{

        //    HttpContext curContext = HttpContext.Current;

        //    // 设置编码和附件格式
        //    curContext.Response.ContentType = "application/vnd.ms-excel";
        //    curContext.Response.ContentEncoding = Encoding.UTF8;
        //    curContext.Response.Charset = "";
        //    curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

        //    curContext.Response.BinaryWrite(Export(dtSource, strHeaderText, title).GetBuffer());
        //    curContext.Response.End();
        //}


        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <returns></returns>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText, string[] chineseHeaders)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "Yeli";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "yelioa"; //填加xls文件作者信息
                si.ApplicationName = strHeaderText; //填加xls文件创建程序信息
                si.LastAuthor = "yelioa"; //填加xls文件最后保存者信息
                si.Comments = "说明信息"; //填加xls文件作者信息
                si.Title = ""; //填加xls文件标题信息
                si.Subject = "";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            //dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }
                    #endregion

                    #region 列头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);
                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        //string[] chineseHeaders;
                        //if(strHeaderText=="部门积分信息")
                        //{
                        //    chineseHeaders =new string[] { "姓名", "部门", "月度积分", "季度积分", "年度积分", "总积分" };
                        //}
                        //else
                        // chineseHeaders = new string[] {"序号", "编号", "提交日期", "审批日期", "财务审批日期",
                        //    "提交人", "部门", "费用归属部门", "产品", "费用明细", "金额", "实报金额", "状态", "审批人", "抄送人", "备注", "审批意见", "审批结果"};

                        for (int i = 0; i < dtSource.Columns.Count; i++)
                        {
                            int colWidth = sheet.GetColumnWidth(i) * 2;
                            if (colWidth < 255 * 256)
                            {
                                sheet.SetColumnWidth(i, colWidth < 3000 ? 3000 : colWidth);
                            }
                            else
                            {
                                sheet.SetColumnWidth(i, 6000);
                            }
                            DataColumn column = dtSource.Columns[i];
                            if (chineseHeaders.Length == 0 || chineseHeaders.Length != dtSource.Columns.Count)
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            else
                                headerRow.CreateCell(column.Ordinal).SetCellValue(chineseHeaders[i]);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 255);

                        }
                        // headerRow.Dispose();
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion

                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

                #region 填充内容
                foreach (DataColumn column in dtSource.Columns)
                {

                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            if ("".Equals(drValue))
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }
                            DateTime date = Convert.ToDateTime(drValue);
                            drValue = date.ToString("yyyy-MM-dd HH:mm:ss");
                            //DateTime dateV;
                            //DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(drValue);

                            //newCell.SetCellType(HSSFCellType.FORMULA);
                            //newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Dispose();

                return ms;
            }

        }


        public static MemoryStream Export(DataTable dtSource, string strHeaderText, string title)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "Yeli";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "yelioa"; //填加xls文件作者信息
                si.ApplicationName = title; //填加xls文件创建程序信息
                si.LastAuthor = "yelioa"; //填加xls文件最后保存者信息
                si.Comments = "说明信息"; //填加xls文件作者信息
                si.Title = title; //填加xls文件标题信息
                si.Subject = "";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            //dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }
                    #endregion

                    #region 列头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);
                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        //string[] chineseHeaders;
                        //if(strHeaderText=="部门积分信息")
                        //{
                        //    chineseHeaders =new string[] { "姓名", "部门", "月度积分", "季度积分", "年度积分", "总积分" };
                        //}
                        //else
                        // chineseHeaders = new string[] {"序号", "编号", "提交日期", "审批日期", "财务审批日期",
                        //    "提交人", "部门", "费用归属部门", "产品", "费用明细", "金额", "实报金额", "状态", "审批人", "抄送人", "备注", "审批意见", "审批结果"};

                        for (int i = 0; i < dtSource.Columns.Count; i++)
                        {
                            int colWidth = sheet.GetColumnWidth(i) * 2;
                            if (colWidth < 255 * 256)
                            {
                                sheet.SetColumnWidth(i, colWidth < 3000 ? 3000 : colWidth);
                            }
                            else
                            {
                                sheet.SetColumnWidth(i, 6000);
                            }
                            DataColumn column = dtSource.Columns[i];
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 255);

                        }
                        // headerRow.Dispose();
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion

                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

                #region 填充内容
                foreach (DataColumn column in dtSource.Columns)
                {

                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            if ("".Equals(drValue))
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }
                            DateTime date = Convert.ToDateTime(drValue);
                            drValue = date.ToString("yyyy-MM-dd HH:mm:ss");
                            //DateTime dateV;
                            //DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(drValue);

                            //newCell.SetCellType(HSSFCellType.FORMULA);
                            //newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Dispose();

                return ms;
            }

        }

    }
}
