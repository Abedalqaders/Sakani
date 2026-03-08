using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PropertyType:byte
    {
        Residential = 1, // سكني (شقق، فلل) 
        Commercial = 2,  // تجاري (مكاتب، محلات) 
        Industrial = 3,  // صناعي (مستودعات، مصانع) 
        MixedUse = 4     // متعدد الاستخدامات 
    }
}
