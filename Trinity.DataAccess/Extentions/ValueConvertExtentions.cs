﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Trinity.DataAccess.Logging;

namespace Trinity.DataAccess.Extentions
{
    public static class ValueConvertExtentions
    {
        public static T TryGetDataValue<T>(IDataReader reader, string name)
        {
            try
            {
                bool fieldFound = false;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var field = reader.GetName(i);
                    if (field == name)
                    {
                        fieldFound = true;
                        var objectValue = reader.GetValue(i);

                        try
                        {
                            if (objectValue != DBNull.Value)
                            {
                                return (T)objectValue;
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }

                if (fieldFound == false)
                {
                    LoggingService.SendToLog("TryGetDataValue", string.Format("Cant find field {0} ", name),
                        LogType.Error);
                }
            }
            catch (Exception exception)
            {
                LoggingService.SendToLog("TryGetDataValue cant convert",
                    name + " " + exception.Message + " " + exception.StackTrace, LogType.Error);
            }
            return default;
        }





        public static float? ToFloatNullable(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value.ToStringValue() != string.Empty)
                return value.ToFloat();
            return 0;
        }

        public static int ToInt(this object value)
        {

            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            if (value.ToStringValue() != string.Empty)
            {

                int tempValue = 0;
                if (int.TryParse(value.ToStringValue(), out tempValue))
                {
                    return tempValue;
                }
            }
            return 0;
        }

        public static int? ToIntNullable(this object value)
        {

            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            if (value.ToString() != string.Empty)
                return int.Parse(value.ToString());
            return 0;
        }

        public static string ToAnsiStringFixedLength(this object value)
        {
            return value.ToStringValue();
        }

        public static int ToInt32(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            if (value.ToString() != string.Empty)
            {

                int tempValue = 0;
                if (int.TryParse(value.ToStringValue(), out tempValue))
                {
                    return tempValue;
                }
            }
            return 0;
        }

        public static long ToInt64(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            if (value.ToString() != string.Empty)
            {

                long tempValue = 0;
                if (long.TryParse(value.ToStringValue(), out tempValue))
                {
                    return tempValue;
                }
            }
            return 0;


        }




        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            return value.ToString();
        }

        public static string ToStringValue(this object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            return value.ToString();
        }


        public static bool IsNullOrEmpty(this object value)
        {
            if (value == null)
                return true;


            if (string.IsNullOrEmpty(value.ToStringValue()))
                return true;


            return false;
        }

        public static bool IsNotNullOrEmpty(this object value)
        {
            if (value == null)
                return false;


            if (string.IsNullOrEmpty(value.ToStringValue()))
                return false;


            return true;
        }


        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream fileStream = null;
            try
            {
                if (!file.IsReadOnly)
                    fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                return true;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return false;
        }

        public static string ToStringValue(this object value, string defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;

            string tempString = Convert.ToString(value);
            if (string.IsNullOrEmpty(tempString)) return defaultValue;

            return value.ToString();
        }

        public static string ToAnsiString(this object value)
        {
            return value.ToStringValue();
        }

        public static string ToStringFixedLength(this object value)
        {
            return value.ToStringValue();
        }

        public static XmlDocument ToXml(this object value)
        {
            return (XmlDocument)value;
        }

        public static DateTime? ToDateTimeNullable(this object value)
        {
            if (value == null || value == DBNull.Value) return null;
            try
            {
                DateTime myD = DateTime.Parse(value.ToString(), CultureInfo.CurrentCulture,
                    DateTimeStyles.AssumeLocal);
                return myD;

            }
            catch (Exception)
            {
                try
                {
                    DateTime dateTime = Convert.ToDateTime(value);
                    return dateTime;
                }
                catch (Exception)
                {
                    try
                    {
                        DateTime dateTime = DateTime.FromOADate(value.ToDouble());
                        return dateTime;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            DateTime dateTime = DateTime.FromBinary(value.ToLong());
                            return dateTime;
                        }
                        catch (Exception exception)
                        {

                        }

                    }

                }

            }
            return null;
        }

        public static DateTime ToDateTime(this object value)
        {
            if (value == null || value == DBNull.Value)
                return
                    new DateTime(1753, 1, 1);


            DateTime myD = new DateTime(1753, 1, 1);
            try
            {
                myD = DateTime.Parse(value.ToStringValue(), CultureInfo.CurrentCulture,
                    DateTimeStyles.AssumeLocal);

            }
            catch (Exception)
            {
                try
                {
                    myD = DateTime.ParseExact(value.ToStringValue(),
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    try
                    {
                        myD = ConvertStringToDateTime(value.ToStringValue());
                    }
                    catch (Exception)
                    {
                        myD = new DateTime(1753, 1, 1);
                    }
                }
            }
            return myD;
        }


        private static DateTime ConvertStringToDateTime(string datetime)
        {
            string[] strDate = datetime.Split('T');
            string date = strDate[0];
            string time = strDate[1].Substring(0, strDate[1].LastIndexOf('+'));
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;

            int.TryParse(date.Substring(0, 4), out year);
            int.TryParse(date.Substring(4, 2), out month);
            int.TryParse(date.Substring(6, 2), out day);

            int.TryParse(time.Substring(0, 2), out hour);
            int.TryParse(time.Substring(2, 2), out min);
            int.TryParse(time.Substring(4, 2), out sec);

            DateTime dtmDateTime = new DateTime(year, month, day, hour, min, sec, Calendar.CurrentEra);
            return dtmDateTime;
        }


        public static DateTime? ToDateTimeNull(this object value, int addDays)
        {
            if (value == null || value == DBNull.Value)
                return null;

            DateTime myD;
            if (DateTime.TryParse(value.ToString(), CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal, out myD))
            {
                if (addDays != 0)
                {
                    return myD.AddDays(addDays);
                }
            }
            return myD;
        }

        public static string ToDateString(this object value)
        {
            var date = value.ToDateTime();

            return date.ToShortDateString();
        }

        public static Guid ToGuid(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return Guid.NewGuid();
            }
            return (Guid)value;
        }


        public static Guid? ToGuidNullable(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            return (Guid)value;
        }


        public static short ToShort(this object value)
        {
            short myT;
            if (value == null || value == DBNull.Value)
                return 0;
            short.TryParse(value.ToString(), out myT);
            return myT;
        }

        public static double? ToDoubleNullable(this object value)
        {
            double myT;
            if (value == null || value == DBNull.Value)
                return null;
            if (value.ToString().Contains(","))
            {
                value = value.ToString().Replace(".", "");
            }
            double.TryParse(value.ToString(), out myT);
            return myT;
        }

        public static long ToLong(this object value)
        {
            long myT;
            if (value == null || value == DBNull.Value)
                return 0;
            long.TryParse(value.ToString(), out myT);
            return myT;
        }

        public static double ToDouble(this object value)
        {
            double myT;
            if (value == null || value == DBNull.Value)
                return 0;
            if (value.ToString().Contains(","))
            {
                value = value.ToString().Replace(".", "");
            }
            double.TryParse(value.ToString(), out myT);
            return myT;
        }
        public static float ToFloat(this object value)
        {
            float myD;
            if (value == null || value == DBNull.Value)
                return 0;
            if (value.ToString().Contains(","))
            {
                value = value.ToString().Replace(".", "");
            }
            float.TryParse(value.ToString(), out myD);

            if (float.IsPositiveInfinity(myD))
            {

                myD = float.MaxValue;

            }
            else if (float.IsNegativeInfinity(myD))
            {

                myD = float.MinValue;
            }

            return myD;
        }


        public static bool ToBool(this object value)
        {
            if (value == null || value == DBNull.Value)
                return false;
            if (value is string)
            {

                var stringValue = value.ToStringValue().ToLower();

                if (stringValue == "yes" || stringValue == "1" || stringValue == "ja" || stringValue == "y" || stringValue == "j")
                {
                    return true;
                }
                return false;
            }

            bool myb = false;
            try
            {
                myb = Convert.ToBoolean(value);
            }
            catch (Exception exception)
            {


            }


            return myb;

            //return (bool)value;
        }

        public static decimal ToDecimal(this object value)
        {
            decimal myD;
            if (value == null || value == DBNull.Value)
                return 0;
            if (value.ToString().Contains(","))
            {
                value = value.ToString().Replace(".", "").Replace("+", "");

            }
            decimal.TryParse(value.ToString(), out myD);
            return myD;
        }

        public static byte ToByte(this object value)
        {
            if (value == null || value == DBNull.Value)
                return new byte();
            return (byte)value;
        }

        public static byte[] ToByteArray(this object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            return (byte[])value;
        }
        public static byte[] ToBinary(object value)
        {
            return value.ToByteArray();
        }
        public static bool ToBoolean(this object value)
        {
            return value.ToBool();
        }

        public static decimal ToCurrency(this object value)
        {
            return value.ToDecimal();
        }

        public static object ToObject(this object value)
        {
            return value;
        }

        public static char ToChar(this object value)
        {
            char temp;

            if (value == null || value == DBNull.Value)
                return new char();

            char.TryParse(value.ToStringValue(), out temp);
            return temp;
        }

        public static T ToEnumValue<T>(this object value, object defaultValue)
        {

            try
            {
                return (T)Enum.Parse(typeof(T), value.ToStringValue(), true);
            }
            catch (Exception exception)
            {

            }

            return (T)defaultValue;

        }

        public static T BaseConvert<T>(this object source)
        {
            var result = source;
            Type DestType = typeof(T);

            if (!(source.GetType() == DestType) && DestType != typeof(object))
            {
                var converter = TypeDescriptor.GetConverter(DestType);
                result = converter.ConvertFrom(source);
            }
            return (T)result;
        }

        public static object ConvertValue(this object value, PropertyInfo prop)
        {

            if (prop == null) return value;

            var propertyType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            var returnType = underlyingType ?? propertyType;
            var typeCode = Type.GetTypeCode(returnType);

            switch (typeCode)
            {
                case TypeCode.DBNull:
                case TypeCode.Object:
                case TypeCode.Empty:
                    return value;
                case TypeCode.SByte:
                    return value.BaseConvert<sbyte>();
                case TypeCode.Byte:
                    return value.ToByte();
                case TypeCode.Boolean:
                    return value.ToBool();
                case TypeCode.Char:
                    return value.ToChar();
                case TypeCode.Int16:
                    return value.ToInt();
                case TypeCode.UInt16:
                    return value.ToInt();
                case TypeCode.Int32:
                    return value.ToInt32();
                case TypeCode.UInt32:
                    return value.ToInt();
                case TypeCode.Int64:
                    return value.ToInt64();
                case TypeCode.UInt64:
                    return value.ToInt();
                case TypeCode.Single:
                    return value.ToFloat();
                case TypeCode.Double:
                    return value.ToDouble();
                case TypeCode.Decimal:
                    return value.ToDecimal();
                case TypeCode.DateTime:
                    return value.ToDateTime();
                case TypeCode.String:
                    return value.ToStringValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static string ToSqlDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static DateTime EndOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
        public static DateTime BeginningOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
        public static DateTime GetNextWorkingDay(this DateTime Current)
        {
            if (Current.DayOfWeek == DayOfWeek.Friday)
            {
                Current = Current.AddDays(3);
            }
            else
            {
                Current = Current.AddDays(1);
            }
            return Current;
        }
        public static DateTime GetLastWorkingDay(this DateTime Current)
        {
            if (Current.DayOfWeek == DayOfWeek.Monday)
            {
                Current = Current.AddDays(-3);
            }
            else
            {
                Current = Current.AddDays(-1);
            }
            return Current;
        }
        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Monday)
            {
                candidateDate = candidateDate.AddDays(-1);
            }
            return candidateDate;
        }
        public static int GetWeekNumber(this DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
        public static double MetersToMiles(double? meters)
        {
            if (meters == null)
                return 0F;

            return meters.Value * 0.000621371192;
        }
        public static double MilesToMeters(double? miles)
        {
            if (miles == null)
                return 0;

            return miles.Value * 1609.344;
        }
        public static int GetAge(this DateTime birthday)
        {
            DateTime now = DateTime.Now;
            int Years = now.Year - birthday.Year;

            if (Years > 0)
            {
                if (now.Month < birthday.Month)
                {
                    Years = Years - 1;
                }
                else if (now.Month == birthday.Month)
                {
                    if (now.Day < birthday.Day)
                    {
                        Years = Years - 1;
                    }
                }
                return Years;
            }
            else if (Years < 0)
            {
                throw new Exception("error ");
            }
            return 0;
        }
        public static DateTime SetHours(this DateTime date, string strHour)
        {
            date = DateTime.Parse(string.Format("{0} {1}", date.ToShortDateString(), strHour));
            return date;
        }
        public static int ToInt(this bool value)
        {
            return value == true ? 1 : 0;
        }
        public static string ToHex(this string text)
        {
            long input;
            bool isNumeric;
            string hexValue;
            Console.WriteLine("Enter a numeric : ");
            isNumeric = long.TryParse(text.Trim(), out input);
            if (isNumeric)
            {
                hexValue = string.Format("0*{0:X}", input);
                return hexValue;
            }
            else
            {
                Console.WriteLine("invalid input ! ");
                return null;
            }
        }
        public static string makeValidFileName(this string text)
        {
            string pattern = @"[&?:\/*""<>|]";
            string replacestring = "_";
            return Regex.Replace(text, pattern, replacestring);
        }
        public static string ChangePathToInternetURL(this string strFilePath)
        {
            string RetVal = "";
            RetVal = Regex.Replace(strFilePath, @"\\", "/");

            return RetVal = "file:///" + RetVal;
        }
        public static string AddZeroPrefix(this string s, int normaleLengte)
        {
            while (s.Length < normaleLengte)
            {
                s = "0" + s;
            }
            return s;
        }
        public static string ToCSVstring(this string[] SingleArray)
        {
            return string.Join(", ", SingleArray);
        }
        static double RoundToSignificantDigits(this double d, int digits)
        {
            double scale = Math.Pow(10, Math.Floor(Math.Log10(d)) + 1);
            return scale * Math.Round(d / scale, digits);
        }
        public static double GetPercent(this double value, double max)
        {
            return value * 100 / max;
        }
        public static int GetPercent(this int value, int max)
        {
            return value * 100 / max;
        }

        public static List<T> ToList<T>(this object value)
        {
            var list = new List<T>();
            list.Add((T)value);
            return list;
        }


    }
}