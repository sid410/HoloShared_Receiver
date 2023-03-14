using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//catalogues all utensils (also can be extended for extra factors)


namespace UtensilExtension
{
    public enum UtensilType
    {
        UNDETECTED = 0, SPOON = 1, FORK = 2, CUP = 3, DISH = 6, KNIFE = 4, MINISPOON = 5, BOTTLE = 8, GLASS = 7
    }

    public static class UtensilUtil
    {
        public static UtensilType getUtensilFromStr(string utensilStr)
        {
            try
            {
                int utensilInt = Int32.Parse(utensilStr);
                return (UtensilType)utensilInt;
            }
            catch
            {
                //if any unexpected error happens, we return a 0
                Debug.LogError("Error when trying to get a utensilType from a string");
                return UtensilType.UNDETECTED;
            }
        }
    }

   
}

