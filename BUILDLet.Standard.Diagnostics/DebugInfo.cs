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
using System.Security;      // for [DynamicSecurityMethod] Attributes
using System.Globalization; // for CultureInfo Class
using System.Diagnostics;   // for StackFrame Class

namespace BUILDLet.Standard.Diagnostics
{
    /// <summary>
    /// ログ出力などに埋め込むデバッグ情報のための文字列を提供します。
    /// </summary>
    /// <remarks>
    /// このクラスはデバッグ ビルドでのみ使用することを推奨します。
    /// このクラスは、<see cref="StackFrame"/> クラスを使用してクラス名やメソッド名の取得しているため、
    /// リリース ビルドでは、コンパイラの最適化のために正しいクラス名やメソッド名が取得できない可能性があります。
    /// </remarks>
    public static class DebugInfo
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------

        // Format string for TimeStamp
        private static string timestampFormat;

        // Default string of Delimiter
        private static string defaultDelimiter;


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="DebugInfo"/> クラスを初期化します。
        /// </summary>
        static DebugInfo() => DebugInfo.Init();


        // ----------------------------------------------------------------------------------------------------
        // Static Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 呼び出し元メソッド名の表示形式を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 既定の表示形式は <see cref="DebugInfoCallerNameFormat.FullName"/> です。
        /// </remarks>
        public static DebugInfoCallerNameFormat CallerNameFormat { get; set; } = DebugInfoCallerNameFormat.FullName;


        /// <summary>
        /// <see cref="DebugInfo.TimeStamp"/> メソッド (および <see cref="DebugInfo.ToString"/> メソッド) において、
        /// 現在時刻と呼び出し元メソッド名の区切り文字を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 既定の文字列は <c>", "</c> です。
        /// </remarks>
        public static string Delimiter { get; set; } = DebugInfo.defaultDelimiter = ", ";


        /// <summary>
        /// 表示する時刻のフォーマットの書式指定文字列を取得または設定します。
        /// </summary>
        public static string TimeStampFormat
        {
            get { return DebugInfo.timestampFormat; }
            set
            {
                try { System.DateTime.Now.ToString(DebugInfo.timestampFormat = value, DebugInfo.CultureInfo); }
                catch (Exception) { throw; }
            }
        }


        /// <summary>
        /// タイムスタンプ (<see cref="DebugInfo.TimeStampFormat"/>) を表示する際に使用される現在のカルチャー (<see cref="CultureInfo"/>) を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 既定のカルチャーは <see cref="CultureInfo.InvariantCulture"/> です。
        /// </remarks>
        public static CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;


        /// <summary>
        /// 呼び出し元のメソッド名のみを文字列として取得します。
        /// </summary>
        public static string Name => DebugInfo.GetCallerName(DebugInfoCallerNameFormat.Name, 2);


        /// <summary>
        /// 呼び出し元のクラス名とメソッド名のみを文字列として取得します。
        /// </summary>
        public static string ShortName => DebugInfo.GetCallerName(DebugInfoCallerNameFormat.ShortName, 2);


        /// <summary>
        /// 呼び出し元のメソッド名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullName => DebugInfo.GetCallerName(DebugInfoCallerNameFormat.FullName, 2);


        /// <summary>
        /// 呼び出し元のクラス名のみを文字列として取得します。
        /// </summary>
        public static string ClassName => DebugInfo.GetCallerName(DebugInfoCallerNameFormat.ClassName, 2);


        /// <summary>
        /// 呼び出し元のクラス名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullClassName => DebugInfo.GetCallerName(DebugInfoCallerNameFormat.FullClassName, 2);


        /// <summary>
        /// 現在時刻を、日付のみを含む文字列として取得します。
        /// </summary>
        /// <remarks>
        /// 書式指定文字列は <c>"yyyy-MM-dd"</c> です。
        /// </remarks>
        public static string Date => System.DateTime.Now.ToString("yyyy-MM-dd", DebugInfo.CultureInfo);


        /// <summary>
        /// 現在時刻を、時刻のみを含む文字列として取得します。
        /// </summary>
        /// <remarks>
        /// 書式指定文字列は <c>"HH:mm:ss"</c> です。
        /// </remarks>
        public static string Time => System.DateTime.Now.ToString("HH:mm:ss", DebugInfo.CultureInfo);


        /// <summary>
        /// 現在時刻を、日付と時刻を含む文字列として取得します。
        /// </summary>
        /// <remarks>
        /// 書式指定文字列は <c>"yyyy-MM-dd HH:mm:ss"</c> です。
        /// </remarks>
        public static string DateTime => System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DebugInfo.CultureInfo);


        /// <summary>
        /// 現在時刻、および、このメソッドの呼び出し元のメソッド名を文字列として取得します。
        /// </summary>
        /// <remarks>
        /// 現在時刻および呼び出し元のメソッド名は、それぞれ <see cref="DebugInfo.TimeStampFormat"/> および <see cref="DebugInfo.CallerNameFormat"/> でフォーマットされた文字列です。
        /// 現在時刻と呼び出し元のメソッド名は <see cref="DebugInfo.Delimiter"/> で区切られます。
        /// </remarks>
        public static string TimeStamp =>
            System.DateTime.Now.ToString(DebugInfo.TimeStampFormat, DebugInfo.CultureInfo) +
            DebugInfo.Delimiter +
            DebugInfo.GetCallerName(DebugInfo.CallerNameFormat, 2);


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="DebugInfo"/> クラスを初期化します。
        /// </summary>
        /// <param name="caller">
        /// <see cref="DebugInfo.CallerNameFormat"/> に設定するメソッド呼び出し元の表示形式を指定します。
        /// 既定の表示形式は <see cref="DebugInfoCallerNameFormat.FullName"/> です。
        /// </param>
        /// <param name="format">
        /// <see cref="DebugInfo.TimeStampFormat"/> に設定する書式指定文字列を指定します。
        /// 既定の文字列は "yyyy-MM-dd HH:mm:ss" です。
        /// </param>
        /// <remarks>
        /// タイムスタンプ (<see cref="DebugInfo.TimeStampFormat"/>) を表示する際に使用されるカルチャー (<see cref="CultureInfo"/>) については、
        /// <see cref="DebugInfo.CultureInfo"/> を参照してください。
        /// </remarks>
        public static void Init(DebugInfoCallerNameFormat caller = DebugInfoCallerNameFormat.FullName, string format = "yyyy-MM-dd HH:mm:ss")
        {
            DebugInfo.CallerNameFormat = caller;
            DebugInfo.TimeStampFormat = format;

            DebugInfo.Delimiter = DebugInfo.defaultDelimiter;
            DebugInfo.CultureInfo = CultureInfo.InvariantCulture;
        }


        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// 現在のオブジェクトを表す文字列文字列。
        /// </returns>
        public static new string ToString() =>
            System.DateTime.Now.ToString(DebugInfo.TimeStampFormat, DebugInfo.CultureInfo) +
            DebugInfo.Delimiter +
            DebugInfo.GetCallerName(DebugInfo.CallerNameFormat, 2);


        // ----------------------------------------------------------------------------------------------------
        // Private Methods
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このメソッドの呼び出し元のクラス名およびメソッド名を取得します。
        /// </summary>
        /// <param name="format">
        /// 取得する書式のクラス名およびメソッド名のフォーマット。
        /// </param>
        /// <param name="skipFrames">
        /// スキップするスタック上のフレーム数。
        /// <para>
        /// 既定のフレーム数は <c>1</c> です。
        /// フレーム数に <c>1</c> を指定することで、このメソッドをコールしたクラス名およびメソッド名を取得できます。
        /// </para>
        /// </param>
        /// <returns>
        /// このメソッドの呼び出し元のクラス名およびメソッド名。
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
    }
}
