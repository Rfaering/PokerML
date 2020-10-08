using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Lib.Models.Strategies.Output
{
    public enum PlayerAction { 
        Fold = 1, 
        Check = 2,
        BB1Raise = 3,
        BB2Raise = 4,
        BB3Raise = 5,
        BB4Raise = 6,
        BB10Percent = 7,
        BB25Percent = 8,
        BB50Percent = 9,
        BB100Percent = 10
    }
}

