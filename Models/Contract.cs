using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;

namespace WebApplication1.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public float Price { get; set; }

        public float Total { get; set; }


        //relation with user

        public User Provider { get; set; }
        public string ProviderId { get; set; }


        public string RequesterId { get; set; }
        public User Requester { get; set; }


    }
  
    public class Contractviewmode
    {
        public int ContractId { get; set; }

        public string Title { get; set; }
        public string Status { get; set; }
        public string name { get; set; }



    }
}
