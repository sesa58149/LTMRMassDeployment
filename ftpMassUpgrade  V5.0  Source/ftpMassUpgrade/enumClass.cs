using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftpMassUpgrade
{
    public enum actionOnControl : int
    {
        PROGRESS_BAR,
        LEBEL,
        DATA_GRID,
        BUTTON,
        TEXT_BOX,
        PROCESS_COMPLETED,
        NO_ACTION=0XFF
    }
    public enum LTRM_ComercialRef:int
    { 
        LTMR_08,
        LTMR_27,
        LTMR_100,
        NONE
    }

}
