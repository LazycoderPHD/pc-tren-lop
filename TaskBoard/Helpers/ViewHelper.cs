using System;
using System.Globalization;

namespace TaskBoard.Helpers
{
    // Hàm hỗ trợ định dạng ngày giờ cho View
    public static class ViewHelper
    {
        // Định dạng ISO để JavaScript đọc được và tính đồng hồ đếm ngược
        public static string ToIso(DateTime? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static string ToDisplay(DateTime? value)
        {
            if (!value.HasValue)
            {
                return "Không có";
            }
            return value.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
