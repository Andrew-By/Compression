using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compression.Models
{
    public class TreeNode
    {
        public byte Byte { get; set; }
        public TreeNode Get0 { get; set; }
        public TreeNode Get1 { get; set; }
    }
}
