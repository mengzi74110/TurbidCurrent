using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class GameHelper
{
    static StringBuilder s_sb = new StringBuilder();
    public static bool TryParseFloat(this string strFloat, out float result)
    {
        result = 0;
        if (string.IsNullOrEmpty(strFloat))
            return false;

        int indexOfDot = strFloat.IndexOf('.');
        if (indexOfDot < 0)
            return float.TryParse(strFloat, out result);

        int curDecimalCount = strFloat.Length - indexOfDot - 1;
        int clipCount = curDecimalCount - 4;
        string str;
        if (clipCount > 0)
        {
            int newLen = strFloat.Length - clipCount;
            str = strFloat.Substring(0, newLen);
        }
        else if (clipCount == 0)
        {
            str = strFloat;
        }
        else
        {
            s_sb.Length = 0;
            s_sb.Append(strFloat);
            clipCount = -clipCount;
            for (int i = 0; i < clipCount; i++)
                s_sb.Append("0");
            str = s_sb.ToString();
        }
        str = str.Replace(".", "");

        if (int.TryParse(str, out int result2))
        {
            result = result2 / 10000f;
            return true;
        }

        return false;
    }
    public static Transform FincChildWithName(this Transform self,string childName)
    {
        if (self.name == childName)
            return self;
        if (self.childCount < 1)
        {
            return null;
        }
        Transform target = null;
        for (int i = 0; i < self.childCount; i++)
        {
            target = self.GetChild(i).FincChildWithName(childName);
            if (target != null)
                return target;
        }
        return target;
    }

}
