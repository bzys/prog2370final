/*
 *  Author:     Bill Sun        
 *  Comments:   enums for type of walls
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-11-2: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSunFinalProject
{
    public enum WALLTYPE
    {
        EMPTY, //empty : 0
        CONCRETE1X, //concrete wall : 1
        CONCRETE1XCORNER, //concrete corner wall : 2
        CONCRETE1XHALFCORNER, //concrete half corner wall : 3
        CONCRETE1XOUTSIDE, //concrete size 1 outside : 4
        CONCRETE2X, //concrete size 2 : 5
        CONCRETE2XOUTSIDE, //concrete size 2 outside : 6
        WOOD1XFULLDMG, //wood wall size 1, completely destroyed : 7
        WOOD1XHALFDMG, //wood wall size 1, half damaged : 8
        WOOD1XNODMG, //wood wall size 1, no damage : 9
        WOOD1XQUARTERDMG, //wood wall size 1, quarter damaged : 10
        WOOD2XFULLDMG, //wood wall size 2, completely destroyed : 11
        WOOD2XHALFDMG, //wood wall size 2, half damaged : 12
        WOOD2XNODMG, //wood wall size 2, no damage : 13
        WOOD2XQUARTERDMG //wood wall size 2, quarter damaged : 14
    }
}
