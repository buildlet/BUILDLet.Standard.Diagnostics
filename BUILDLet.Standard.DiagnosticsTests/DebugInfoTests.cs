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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Globalization; // for CultureInfo class

using BUILDLet.UnitTest.Utilities; // for TestParameter class

namespace BUILDLet.Standard.Diagnostics.Tests
{
    [TestClass]
    public class DebugInfoTests
    {
        [TestInitialize()]
        public void InitializeDebugInfo()
        {
            DebugInfo.Init();
        }


        // for CallerName Tests
        public class CallerNameTestParameter : TestParameter<string>
        {
            public DebugInfoCallerNameFormat Format;
            public string CallerName;

            public override string Expected => this.CallerName;
        }

        [DataTestMethod()]
        [DataRow(DebugInfoCallerNameFormat.Name, nameof(GetCallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(GetCallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(GetCallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.ClassName, "DebugInfoTests")]
        [DataRow(DebugInfoCallerNameFormat.FullClassName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests")]
        public void GetCallerNameTest(DebugInfoCallerNameFormat format, string caller)
        {
            // ARRANGE
            DebugInfo.Init(format);

            // ACT
            CallerNameTestParameter param = new CallerNameTestParameter()
            {
                Keyword = format.ToString(),
                Format = format,
                CallerName = caller,
                Actual = DebugInfo.GetCallerName(),
            };

            // ASSERT
            param.Assert();
        }

        [DataTestMethod()]
        [DataRow(DebugInfoCallerNameFormat.Name, nameof(CallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(CallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(CallerNameTest))]
        [DataRow(DebugInfoCallerNameFormat.ClassName, "DebugInfoTests")]
        [DataRow(DebugInfoCallerNameFormat.FullClassName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests")]
        public void CallerNameTest(DebugInfoCallerNameFormat format, string caller)
        {
            // ARRANGE
            DebugInfo.Init(format);

            CallerNameTestParameter param = new CallerNameTestParameter()
            {
                Keyword = format.ToString(),
                Format = format,
                CallerName = caller
            };

            // ACT
            switch (param.Format)
            {
                case DebugInfoCallerNameFormat.Name:
                    param.Actual = DebugInfo.Name;
                    break;

                case DebugInfoCallerNameFormat.ShortName:
                    param.Actual = DebugInfo.ShortName;
                    break;

                case DebugInfoCallerNameFormat.FullName:
                    param.Actual = DebugInfo.FullName;
                    break;

                case DebugInfoCallerNameFormat.ClassName:
                    param.Actual = DebugInfo.ClassName;
                    break;

                case DebugInfoCallerNameFormat.FullClassName:
                    param.Actual = DebugInfo.FullClassName;
                    break;

                default:
                    break;
            }

            // ASSERT
            param.Assert();
        }


        [TestMethod()]
        public static void CallerNameHierarchicalTest()
        {
            CallerNameHierarchicalTest1();
        }

        private static void CallerNameHierarchicalTest1()
        {
            CallerNameHierarchicalTest2();
        }

        private static void CallerNameHierarchicalTest2()
        {
            CallerNameHierarchicalTest3();
        }

        private static void CallerNameHierarchicalTest3()
        {
            // ARRANGE & ACT
            CallerNameTestParameter param = new CallerNameTestParameter()
            {
                Format = DebugInfoCallerNameFormat.ShortName,
                CallerName = "DebugInfoTests.CallerNameHierarchicalTest3",
                Actual = DebugInfo.ShortName,
            };

            // ASSERT
            param.Assert();
        }


        [TestMethod()]
        [TestCategory("Exception Test")]
        [ExpectedException(typeof(FormatException))]
        public void TimeStampFormatExceptionTest()
        {
            DebugInfo.TimeStampFormat = "!";
        }


        // for TimeStamp Tests
        public class TimeStampTestParameter : TestParameter<string>
        {
            public string Format;
            public CultureInfo CultureInfo;

            public override string Expected => DateTime.Now.ToString(
                (string.IsNullOrEmpty(this.Format)) ? "yyyy/MM/dd-HH:mm:ss" : this.Format,
                (this.CultureInfo is null) ? new CultureInfo("en-US") : this.CultureInfo);
        }

        [DataTestMethod()]
        [DataRow(null, null, DisplayName = "Default")]
        [DataRow(null, "ja-JP")]
        [DataRow("G", null)]
        [DataRow("G", "ja-JP")]
        [DataRow("D", "ja-JP")]
        [DataRow("F", "ja-JP")]
        public void GetTimeStampTest(string format, string cultureName)
        {
            // ARRANGE
            if (format is null)
            {
                if (cultureName is null)
                {
                    DebugInfo.Init();
                }
                else
                {
                    DebugInfo.Init(culture: new CultureInfo(cultureName));
                }
            }
            else
            {
                if (cultureName is null)
                {
                    DebugInfo.Init(format: format);
                }
                else
                {
                    DebugInfo.Init(format: format, culture: new CultureInfo(cultureName));
                }
            }

            // ACT
            TimeStampTestParameter param = new TimeStampTestParameter()
            {
                Keyword = (format is null ? "Default" : $"\"{format}\"") + " (" + (cultureName is null ? "Default" : $"\"{cultureName}\"") + ")",
                Format = format,
                CultureInfo = cultureName is null ? null : new CultureInfo(cultureName),
                Actual = DebugInfo.GetTimeStamp()
            };

            // ASSERT
            param.Assert();
        }


        // for Date & Time Tests
        public class DateTimeTestParameter : TimeStampTestParameter
        {
            public override string Expected => System.DateTime.Now.ToString(this.Format, this.CultureInfo);
        }

        [TestMethod()]
        public void DateTest()
        {
            // ARRANGE & ACT
            DateTimeTestParameter param = new DateTimeTestParameter()
            {
                Format = "yyyy/MM/dd",
                CultureInfo = new CultureInfo("en-US"),
                Actual = DebugInfo.Date
            };

            // ASSERT
            param.Assert();
        }

        [TestMethod()]
        public void TimeTest()
        {
            // ARRANGE & ACT
            DateTimeTestParameter param = new DateTimeTestParameter()
            {
                Format = "HH:mm:ss",
                CultureInfo = new CultureInfo("en-US"),
                Actual = DebugInfo.Time
            };

            // ASSERT
            param.Assert();
        }


        public class ToStringTestParameter : TestParameter<string>
        {
            public string CallerName;

            public override string Expected => System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss", new CultureInfo("en-US")) + "," + this.CallerName;
        }

        [TestMethod()]
        public void ToStringTest()
        {
            // ARRANGE & ACT
            ToStringTestParameter param = new ToStringTestParameter
            {
                CallerName = "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests.ToStringTest",
                Actual = DebugInfo.ToString(),
            };

            // ASSERT
            param.Assert();
        }
    }
}
