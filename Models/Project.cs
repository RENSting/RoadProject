using System;
using System.ComponentModel.DataAnnotations;

namespace RoadProject.Models
{
    public class Project
    {
        public int Id{get;set;}
        
        [Display(Name="项目名称"), Required(ErrorMessage="必须输入")]
        public string Name{get;set;}

        [Display(Name="建设单位")]
        public string ConstructionUnit{get;set;}

        [Display(Name="监理单位")]
        public string Supervisor{get;set;}

        [Display(Name="施工单位")]
        public string Builder{get;set;}

        [Display(Name="合同段")]
        public string ContractPart{get;set;}

        [Display(Name="所属单位工程")]
        public string ProjectItem{get;set;}

        [Display(Name="有效状态")]
        public bool IsActive{get;set;}

        [Display(Name="创建时间")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}")]
        public DateTime CreatedOn{get;set;}

    }
}