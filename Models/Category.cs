using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadProject.Models
{
    public class Category
    {
        public int Id{get;set;}

        [Display(Name="目录")]
        public string Name{get;set;}

        public ICollection<Document> Documents{get;set;} = new List<Document>();
    }
}