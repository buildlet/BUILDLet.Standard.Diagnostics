/***************************************************************************************************
The MIT License (MIT)

Copyright 2019 Daiki Sakamoto

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************************************/

using System;

using System.Security; // for [DynamicSecurityMethod] Attributes
using System.Globalization; // for CultureInfo Class
using System.Diagnostics; // for StackFrame Class

namespace BUILDLet.Standard.Diagnostics
{
    /// <summary>
    /// ログ出力などに埋め込むためのデバッグ情報の文字列を実装します。
    /// <para>
    /// このクラスは静的クラスです。
    /// 必要に応じて <see cref="DebugInfo.Init()"/> または <see cref="DebugInfo.Init(DebugInfoCallerNameFormat, string, CultureInfo)"/> メソッドでプロパティの値を初期化してください。
    /// </para>
    /// </summary>
    /// <remarks>
    /// このクラスはデバッグ ビルドでのみ使用することを推奨します。
    /// <see cref="StackFrame"/> クラスを使用してクラス名やメソッド名の取得しているため、
    /// リリース ビルドでは、コンパイラの最適化のために正しいクラス名やメソッド名が取得できない場合があります。
    /// </remarks>
    public static class DebugInfo
    {
        // Format string for TimeStamp
        private static string format;


        /// <summary>
        /// <see cref="DebugInfo"/> クラスの静的コンストラクター
        /// </summary>
        static DebugInfo() { DebugInfo.Init(); }


        /// <summary>
        /// <see cref="DebugInfo"/> クラスを初期化します。
        /// </summary>
        public static void Init()
        {
            DebugInfo.CallerNameFormat = DebugInfo.DefaultCallerNameFormat;
            DebugInfo.TimeStampFormat = DebugInfo.DefaultTimeStampFormat;
            DebugInfo.CultureInfo = DebugInfo.DefaultCultureInfo;
        }

        /// <summary>
        /// <see cref="DebugInfo"/> クラスを初期化します。
        /// </summary>
        /// <param name="caller">
        /// <see cref="DebugInfo.CallerNameFormat"/> に設定するクラス名およびメソッド名の形式を指定します。
        /// 既定では <see cref="DebugInfo.DefaultCallerNameFormat"/> です。
        /// </param>
        /// <param name="format">
        /// <see cref="DebugInfo.TimeStampFormat"/> に設定する書式指定文字列を指定します。
        /// 既定では <see cref="DebugInfo.DefaultTimeStampFormat"/> です。
        /// </param>
        /// <param name="culture">
        /// タイムスタンプを表示する際に使用されるカルチャー (<see cref="CultureInfo"/>) を指定します。
        /// null を指定すると、既定のカルチャー （<see cref="DebugInfo.DefaultCultureInfo"/>) が設定されます。
        /// 既定では null です。
        /// </param>
        public static void Init(DebugInfoCallerNameFormat caller = DebugInfo.DefaultCallerNameFormat, string format = DebugInfo.DefaultTimeStampFormat, CultureInfo culture = null)
        {
            DebugInfo.CallerNameFormat = caller;
            DebugInfo.TimeStampFormat = format;
            DebugInfo.CultureInfo = culture ?? DebugInfo.DefaultCultureInfo;
        }


        /// <summary>
        /// 呼び出し元メソッド名の既定の表示形式を表します。
        /// <see cref="DebugInfoCallerNameFormat.FullName"/> です。
        /// </summary>
        public const DebugInfoCallerNameFormat DefaultCallerNameFormat = DebugInfoCallerNameFormat.FullName;


        /// <summary>
        /// タイムスタンプの既定の表示形式の書式指定文字列を表します。
        /// 書式指定文字列は "yyyy/MM/dd-HH:mm:ss" です。
        /// </summary>
        public const string DefaultTimeStampFormat = "yyyy/MM/dd-HH:mm:ss";


        /// <summary>
        /// タイムスタンプの書式指定に使用される既定のカルチャー (<see cref="CultureInfo"/>) を表します。
        /// 既定のカルチャーの名前は "en-US" です。
        /// </summary>
        public static CultureInfo DefaultCultureInfo { get; } = new CultureInfo("en-US");


        /// <summary>
        /// 表示するクラス名およびメソッド名の形式を設定または取得します。
        /// 既定の設定は <see cref="DebugInfo.DefaultCallerNameFormat"/> です。
        /// </summary>
        public static DebugInfoCallerNameFormat CallerNameFormat { get; set; } = DebugInfo.DefaultCallerNameFormat;


        /// <summary>
        /// 表示する時刻のフォーマットの書式指定文字列を取得または設定します。
        /// </summary>
        public static string TimeStampFormat
        {
            get { return DebugInfo.format; }
            set
            {
                try { System.DateTime.Now.ToString(DebugInfo.format = value, DebugInfo.CultureInfo); }
                catch (Exception) { throw; }
            }
        }


        /// <summary>
        /// 現在の <see cref="DebugInfo.TimeStampFormat"/> の <see cref="CultureInfo"/> を取得または設定します。
        /// </summary>
        public static CultureInfo CultureInfo { get; set; }


        /// <summary>
        /// デバッグ情報として、呼び出し元のメソッド名のみを文字列として取得します。
        /// </summary>
        public static string Name
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerNameFormat.Name, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名とメソッド名のみを文字列として取得します。
        /// </summary>
        public static string ShortName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerNameFormat.ShortName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のメソッド名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerNameFormat.FullName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名のみを文字列として取得します。
        /// </summary>
        public static string ClassName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerNameFormat.ClassName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullClassName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerNameFormat.FullClassName, 2);
            }
        }


        /// <summary>
        /// 現在時刻を、日付のみを含む文字列として取得します。
        /// 書式指定文字列は "yyyy/MM/dd" です。
        /// </summary>
        public static string Date
        {
            get
            {
                return System.DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            }
        }


        /// <summary>
        /// 現在時刻を、時刻のみを含む文字列として取得します。
        /// 書式指定文字列は "HH:mm:ss" です。
        /// </summary>
        public static string Time
        {
            get
            {
                return System.DateTime.Now.ToString("HH:mm:ss", new CultureInfo("en-US"));
            }
        }


        /// <summary>
        /// 現在時刻を、現在の <see cref="DebugInfo.TimeStampFormat"/> および <see cref="DebugInfo.CultureInfo"/> でフォーマットされた文字列として取得します。
        /// </summary>
        /// <returns>
        /// 現在時刻の文字列
        /// </returns>
        public static string GetTimeStamp()
        {
            return System.DateTime.Now.ToString(DebugInfo.TimeStampFormat, DebugInfo.CultureInfo);
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のメソッド名を文字列として取得します。
        /// 表示形式は <see cref="DebugInfo.CallerNameFormat"/> です。
        /// </summary>
        /// <returns>
        /// 呼び出し元メソッド名の文字列
        /// </returns>
        public static string GetCallerName()
        {
            return DebugInfo.GetCallerName(DebugInfo.CallerNameFormat, 2);
        }


        /// <summary>
        /// 書式のクラス名およびメソッド名を取得します。
        /// </summary>
        /// <param name="format">
        /// 取得する書式のクラス名およびメソッド名のフォーマットを指定します。
        /// </param>
        /// <param name="skipFrames">
        /// スキップするスタック上のフレーム数
        /// <para>
        /// 既定のフレーム数は 1 です。
        /// フレーム数 1 を指定することで、このメソッドをコールしたクラス名およびメソッド名を取得できます。
        /// </para>
        /// </param>
        /// <returns>
        /// 書式のクラス名およびメソッド名
        /// </returns>
        [DynamicSecurityMethod]
        private static string GetCallerName(DebugInfoCallerNameFormat format, int skipFrames = 1)
        {
            StackFrame sf = new StackFrame(skipFrames);
            string fullClassName = sf.GetMethod().ReflectedType.FullName;
            string className = sf.GetMethod().ReflectedType.Name;
            string methodName = sf.GetMethod().Name;

            switch (format)
            {
                // case DebugInfoCallerFormat.None:
                //     break;

                case DebugInfoCallerNameFormat.Name:
                    return methodName;

                case DebugInfoCallerNameFormat.ShortName:
                    return className + "." + methodName;

                case DebugInfoCallerNameFormat.FullName:
                    return fullClassName + "." + methodName;

                case DebugInfoCallerNameFormat.ClassName:
                    return className;

                case DebugInfoCallerNameFormat.FullClassName:
                    return fullClassName;

                default:
                    throw new InvalidOperationException();
            }
        }


        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// デバッグ情報の文字列
        /// </returns>
        public static new string ToString()
        {
            return DebugInfo.GetTimeStamp() + "," + DebugInfo.GetCallerName(DebugInfo.CallerNameFormat, 2);
        }
    }
}
