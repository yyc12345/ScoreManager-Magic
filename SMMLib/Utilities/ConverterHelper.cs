using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Utilities {
    public static class ConverterHelper {
        public static DateTime ConvertToDatetime(this long timestamp) {
            return new DateTime(timestamp * 10000000 + 621355968000000000, DateTimeKind.Utc);
        }

        public static long ConvertToTimestamp(this DateTime date, bool assumeUtc) {
            if (assumeUtc) return (date.Ticks - 621355968000000000) / 10000000;
            else return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static bool ConvertToBoolean(this int input) {
            return (input != 0);
        }

        public static bool ConvertToBoolean(this string input) {
            return input.ConvertToInt().ConvertToBoolean();
        }

        public static int ConvertToInt(this bool input) {
            if (input) return 1;
            else return 0;
        }

        public static int ConvertToInt(this string input) {
            try {
                return int.Parse(input);
            } catch {
                return 0;
            }
        }

        public static bool UniformBoolean(this bool? input) {
            if (input is null) return false;
            return input.Value;
        }

        public static string SRTimeFormat(this int time) {
            return string.Format("{0}:{1}.{2}",
                    (time / (1000 * 60)).ToString(),
                    (time % (1000 * 60) / 1000).ToString().PadLeft(2, '0'),
                    (time % 1000).ToString().PadLeft(3, '0'));
        }
    }
}
