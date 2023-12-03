using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ContractViewModel
    {
        public string Title { get; set; }

        public string FullName { get; set; }

    }
    public class ContractCreateViewModel
    {
        public int ContractId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public float Price { get; set; }

        public float Total { get; set; }
        public string ProviderName { get; set; }
        public string RequesterName { get; set; }

    }


    public class ContractPostViewModel
    {
        public int ContractId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        public float Total { get; set; }
        public string Providername { get; set; }
        public string Requestername { get; set; }

    }
    public class ContracStatus
    {
        public int ContractId { get; set; }

        public string Status { get; set; }

    }



}
