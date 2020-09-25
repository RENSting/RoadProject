using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadProject.Models
{
    public class Document
    {
        public int Id{get;set;}

        [Display(Name="文件名称")]
        public string FileName{get;set;}

        [Display(Name="所属目录")]
        public int CategoryId{get;set;}
        public Category Category{get;set;}
    }
}