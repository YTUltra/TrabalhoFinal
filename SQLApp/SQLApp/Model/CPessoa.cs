using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace SQLApp.Model
{
    public class CPessoa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }
    }
}
