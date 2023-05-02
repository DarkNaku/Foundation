using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OfficeOpenXml;

public static class EPPlusEx 
{
    public static string GetExcelColumn(this ExcelRangeBase erb)
    {
        int dividend = erb.End.Column;
        string columnName = string.Empty;
        int modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (int)((dividend - modulo) / 26);
        }

        return columnName;
    }
}
