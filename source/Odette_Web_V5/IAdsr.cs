using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCIA
{
    interface IAdsr
    {
         DateTime StartDate
        {
          
            get;
            set;
        }
         DateTime EndDate
        {
            get;
            set;
        }
         bool GenerateOldData
        {
            get;
            set;
        }
         string LastPooledDate
        {
            get;
            set;
        }
         string TittleHeader
        {
            get;
            set;
        }
         string SegmentHeader
        {
            get;
            set;
        }
         void GenerateFile();
       
    }
}
