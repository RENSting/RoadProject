using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;

using RoadProject.Models;
using NPOI.XWPF.UserModel;

namespace RoadProject
{
    public static class Helper
    {
        public static void CreateTemplateInBuffer(Stream buffer, Project project, string dir, string file)
        {
            //TODO: 打开文件，写入项目信息，返回文件字节结果。
            string path = $"Templates/{dir}/{file}";

            using var source = File.Open(path, FileMode.Open, FileAccess.ReadWrite);

            if (path.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                var doc = new XWPFDocument(source);
                if (dir.Contains("A表"))
                {
                    if (file.StartsWith("施工报审用表"))
                    {
                        doc.ReplaceTagsInDoc(project, A01_47);
                    }
                    else
                    {
                        throw new NotImplementedException($"{path} 此文档的模板尚未实现，请先使用其它模板文件。");
                    }
                }
                else
                {
                    throw new NotImplementedException($"{dir} 此文件夹的模板尚未实现，请先使用其它模板。");
                }
                doc.Write(buffer);
            }
            else
            {
                IWorkbook book;
                if (path.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    book = new HSSFWorkbook(source);
                }
                else if (path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    book = new XSSFWorkbook(source);
                }
                else
                {
                    throw new Exception($"只能处理Word2007或Excel文件格式的模板，传入的{path}格式不对");
                }

                if (dir.Contains("D表"))
                {
                    if (file.Contains("05 "))
                    {
                        //D表 05 排水工程检查记录表
                        D05(project, book);
                    }
                    else if (file.Contains("04 "))
                    {
                        D04(project, book);
                    }
                    else
                    {
                        throw new NotImplementedException($"{path} 此表格的模板尚未实现，请先使用其它模板文件。");
                    }
                }
                else if (dir.Contains("E表"))
                {
                    if (file.Contains("04 "))
                    {
                        E04(project, book);
                    }
                    else
                    {
                        throw new NotImplementedException($"{path} 此表格的模板尚未实现，请先使用其它模板文件。");
                    }
                }
                else
                {
                    throw new NotImplementedException($"{dir} 此文件夹里的模板尚未实现，请先使用其它模板。");
                }

                book.Write(buffer);
            }
        }

        /// <summary>
        /// Extension function on NPOI word document object,
        /// to replace tags using proper 'replace' action
        /// </summary>
        /// <param name="document"></param>
        /// <param name="project"></param>
        /// <param name="replace"></param>
        public static void ReplaceTagsInDoc(this XWPFDocument document, Project project,
            Action<XWPFParagraph, Project> replace)
        {
            foreach (var p in document.Paragraphs)
            {
                replace(p, project);
            }
            foreach (var t in document.Tables)
            {
                foreach (var r in t.Rows)
                {
                    foreach (var c in r.GetTableCells())
                    {
                        foreach (var p in c.Paragraphs)
                        {
                            replace(p, project);
                        }
                    }
                }
            }
        }
        private static void A01_47(XWPFParagraph p, Project project)
        {
            if (p.ParagraphText.Contains("{$Name}"))
            {
                p.ReplaceText("{$Name}", project.Name);
            }
            if (p.ParagraphText.Contains("{$Builder}"))
            {
                p.ReplaceText("{$Builder}", project.Builder);
            }
            if (p.ParagraphText.Contains("{$Supervisor}"))
            {
                p.ReplaceText("{$Supervisor}", project.Supervisor);
            }
            if (p.ParagraphText.Contains("{$ContractPart}"))
            {
                p.ReplaceText("{$ContractPart}", project.ContractPart);
            }
            if (p.ParagraphText.Contains("{$ProjectItem}"))
            {
                p.ReplaceText("{$ProjectItem}", project.ProjectItem);
            }
            if (p.ParagraphText.Contains("{$Constructor}"))
            {
                p.ReplaceText("{$Constructor}", project.ConstructionUnit);
            }
        }

        private static void D04(Project project, IWorkbook book)
        {
            IFont fUB16 = book.CreateFont();
            fUB16.FontName = "黑体";
            fUB16.FontHeightInPoints = 16;
            fUB16.Underline = FontUnderlineType.Single;
            IFont fB16 = book.CreateFont();
            fB16.FontName = "黑体";
            fB16.FontHeightInPoints = 16;
            IRichTextString title;
            int underLineLength = project.Name.Length;
            if (book is HSSFWorkbook)
            {
                title = new HSSFRichTextString($"{project.Name}公路");
            }
            else
            {
                title = new XSSFRichTextString($"{project.Name}公路");
            }
            title.ApplyFont(fB16);
            title.ApplyFont(0, underLineLength, fUB16);

            for (int i = 1; i < book.NumberOfSheets; i++)
            {
                ISheet sheet = book.GetSheetAt(i);
                if (sheet == null)
                {
                    continue;
                }
                sheet.GetRow(0).GetCell(0).SetCellValue(title);
                sheet.GetRow(2).GetCell(0).SetCellValue($"施工单位：{project.Builder}");
            }
        }

        private static void D05(Project project, IWorkbook book)
        {
            IFont fUB14 = book.CreateFont();
            fUB14.FontName = "黑体";
            fUB14.FontHeightInPoints = 14;
            fUB14.Underline = FontUnderlineType.Single;
            IFont fB14 = book.CreateFont();
            fB14.FontName = "黑体";
            fB14.FontHeightInPoints = 14;
            IRichTextString title;
            int underLineLength = project.Name.Length;
            if (book is HSSFWorkbook)
            {
                title = new HSSFRichTextString($"{project.Name}公路");
            }
            else
            {
                title = new XSSFRichTextString($"{project.Name}公路");
            }
            title.ApplyFont(fB14);
            title.ApplyFont(0, underLineLength, fUB14);

            for (int i = 1; i < book.NumberOfSheets; i++)
            {
                ISheet sheet = book.GetSheetAt(i);
                if (sheet == null)
                {
                    continue;
                }
                sheet.GetRow(0).GetCell(0).SetCellValue(title);
                sheet.GetRow(2).GetCell(0).SetCellValue($"施工单位：{project.Builder}");
            }
        }

        private static void E04(Project project, IWorkbook book)
        {
            for (int i = 1; i < book.NumberOfSheets; i++)
            {
                ISheet sheet = book.GetSheetAt(i);
                if (sheet == null)
                {
                    continue;
                }
                string empty = new string(' ', 20);
                sheet.GetRow(1).GetCell(0).SetCellValue(
                    $"  分项工程名称：{empty}工程部位：{empty}{empty}所属建设项目：{project.Name}");
                sheet.GetRow(2).GetCell(0).SetCellValue(
                    $"  所属分部工程名称：{empty}所属单位工程：{project.ProjectItem}  施工单位：{project.Builder}  分项工程编号：");
            }
        }
    }
}