using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace PetFeederFunctions
{
    public class Counter : TableEntity
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
}