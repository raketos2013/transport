using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class ClientLog
    {
        [Key]
        public long Id { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string? Values { get; set; }
        
        public DateTime Created { get; set; }
    }
}
