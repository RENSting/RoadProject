using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadProject.Models
{
    public enum LogLevel{
        [Display(Name="调试")]Debug, 
        [Display(Name="跟踪")]Inform, 
        [Display(Name="消息")]Message, 
        [Display(Name="错误")]Error
    }

    public class Log
    {
        public int Id{get;set;}

        [Display(Name="日志内容")]
        public string Message{get;set;}

        [Display(Name="分类")]
        public LogLevel Level{get;set;}

        [Display(Name="时间戳")]
        public DateTime CreatedOn{get;set;}
    }
}