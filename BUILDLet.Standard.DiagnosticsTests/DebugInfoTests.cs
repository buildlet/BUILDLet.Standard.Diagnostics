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
using System.Globalization;        // for CultureInfo class

using BUILDLet.UnitTest.Utilities; // for TestParameter class

namespace BUILDLet.Standard.Diagnostics.Tests
{
    [TestClass]
    public class DebugInfoTests
    {
        [TestInitialize]
        public void InitializeDebugInfo() => DebugInfo.Init();


        // ----------------------------------------------------------------
        // Base Type of TestParameter for DebugInfo Class Tests
        // ----------------------------------------------------------------

        public abstract class DebugInfoTestParameter<T> : TestParameter<T>
        {
            public bool DefaultTest;
            public DebugInfoCallerNameFormat CallerNameFormat;
            public string CallerName;
            public string TimeStampFormat;
            public string CultureName;
            public string Delimiter;
        }


        // ----------------------------------------------------------------
        // Tests of TimeStampFormat Property
        // ----------------------------------------------------------------

        [DataTestMethod]
        [TestCategory("Exception")]
        [DataRow("!")]
        [ExpectedException(typeof(FormatException))]
        public void TimeStampFormatExceptionTest(string format) => DebugInfo.TimeStampFormat = format;


        // ----------------------------------------------------------------------------------------------------
        // Tests of Name, ShortName, FullName, ClassName and FullClassName Properties
        // ----------------------------------------------------------------------------------------------------

        // TestParameter for Name, ShortName, FullName, ClassName and FullClassName Properties Test
        public class CallerNamePropertiesTestParameter : DebugInfoTestParameter<string>
        {
            // ARRANGE: SET Expected
            public override void Arrange(out string expected) => expected = this.CallerName;

            // ACT: GET Actual
            public override void Act(out string actual) => actual = DebugInfoTests.CallerNamePropertiesTestMethod(this.CallerNameFormat);
        }

        // Get Caller Name
        public static string CallerNamePropertiesTestMethod(DebugInfoCallerNameFormat format) => format switch
        {
            DebugInfoCallerNameFormat.Name => DebugInfo.Name,
            DebugInfoCallerNameFormat.ShortName => DebugInfo.ShortName,
            DebugInfoCallerNameFormat.FullName => DebugInfo.FullName,
            DebugInfoCallerNameFormat.ClassName => DebugInfo.ClassName,
            DebugInfoCallerNameFormat.FullClassName => DebugInfo.FullClassName,
            _ => throw new InvalidOperationException(),
        };

        [DataTestMethod]
        [DataRow(DebugInfoCallerNameFormat.Name, nameof(CallerNamePropertiesTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(CallerNamePropertiesTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(CallerNamePropertiesTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ClassName, "DebugInfoTests")]
        [DataRow(DebugInfoCallerNameFormat.FullClassName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests")]
        public void CallerNamePropertiesTest(DebugInfoCallerNameFormat format, string caller)
        {
            // SET Parameter
            CallerNamePropertiesTestParameter param = new CallerNamePropertiesTestParameter()
            {
                Keyword = format.ToString(),
                CallerNameFormat = format,
                CallerName = caller
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of Date, Time DateTime Properties
        // ----------------------------------------------------------------

        // TestParameter for Date, Time DateTime Property Tests
        public class DateTimePropertiesTestParameter : DebugInfoTestParameter<string[]>
        {
            // ARRANGE
            public override void Arrange(out string[] expected) => expected = new string[]
            {
                // Data
                System.DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),

                // Time
                System.DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture),

                // DateTime
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            };

            // ACT
            public override void Act(out string[] actual) => actual = new string[]
            {
                // Data
                DebugInfo.Date,

                // Time
                DebugInfo.Time,

                // DateTime
                DebugInfo.DateTime,
            };
        }

        [TestMethod]
        public void DateTimePropertiesTest() => new DateTimePropertiesTestParameter().Validate();


        // ----------------------------------------------------------------
        // Tests of TimeStamp Property
        // ----------------------------------------------------------------

        // TestParameter for TimeStamp Property Tests
        public class TimeStampPropertyTestParameter : DebugInfoTestParameter<string>
        {
            // ARRANGE: SET Expected
            public override void Arrange(out string expected)
            {
                if (DefaultTest)
                {
                    expected = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + DebugInfo.Delimiter + this.CallerName;
                }
                else
                {
                    expected = System.DateTime.Now.ToString(this.TimeStampFormat, new CultureInfo(this.CultureName))
                        + (this.Delimiter is null ? DebugInfo.Delimiter : this.Delimiter)
                        + this.CallerName;
                }
            }

            // ACT
            public override void Act(out string actual)
            {
                // Initialize DebugInfo
                DebugInfo.Init();

                if (!DefaultTest)
                {
                    // SET TimeStampFormat
                    if (this.TimeStampFormat != null && this.TimeStampFormat != DebugInfo.TimeStampFormat)
                    {
                        DebugInfo.TimeStampFormat = this.TimeStampFormat;
                    }

                    // SET CultureInfo
                    if (this.CultureName != null && this.CultureName != DebugInfo.CultureInfo.Name)
                    {
                        DebugInfo.CultureInfo = new CultureInfo(this.CultureName);
                    }

                    // SET Delimiter
                    if (this.Delimiter != null && this.Delimiter != DebugInfo.Delimiter)
                    {
                        DebugInfo.Delimiter = this.Delimiter;
                    }

                    // SET CallerNameFormat
                    if (this.CallerNameFormat != DebugInfo.CallerNameFormat)
                    {
                        DebugInfo.CallerNameFormat = this.CallerNameFormat;
                    }
                }

                // GET Actual
                actual = DebugInfoTests.TimeStampPropertyTestMethod();
            }
        }

        // GET Caller Name by DebugInfo.TimeStamp Property
        public static string TimeStampPropertyTestMethod() => DebugInfo.TimeStamp;

        [DataTestMethod]
        [DataRow(true, null, null, null, null, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(TimeStampPropertyTestMethod))]
        [DataRow(false, "yyyy-MM-dd HH:mm:ss", "ja-JP", ",", DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(TimeStampPropertyTestMethod))]
        [DataRow(false, "G", "ja-JP", ",", DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(TimeStampPropertyTestMethod))]
        [DataRow(false, "g", "ja-JP", ",", DebugInfoCallerNameFormat.Name, nameof(TimeStampPropertyTestMethod))]
        [DataRow(false, "F", "ja-JP", null, DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(TimeStampPropertyTestMethod))]
        public void TimeStampPropertyTest(bool defaultTest, string timestampFormat, string culture, string delimiter, DebugInfoCallerNameFormat callernameFormat, string caller)
        {
            // SET Parameter
            TimeStampPropertyTestParameter param = new TimeStampPropertyTestParameter
            {
                DefaultTest = defaultTest,
                TimeStampFormat = timestampFormat,
                CultureName = culture,
                Delimiter = delimiter,
                CallerNameFormat = callernameFormat,
                CallerName = caller,
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of ToString Method
        // ----------------------------------------------------------------

        // TestParameter for ToString Tests
        public class ToStringMethodTestParameter : DebugInfoTestParameter<string>
        {
            // ARRANGE
            public override void Arrange(out string expected) =>
                expected =
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + ", " +
                "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(ToStringMethodTestMethod);

            // ACT
            public override void Act(out string actual) => actual = DebugInfoTests.ToStringMethodTestMethod();
        }

        // GET String
        public static string ToStringMethodTestMethod() => DebugInfo.ToString();

        [TestMethod]
        public void ToStringMethodTest() => new ToStringMethodTestParameter().Validate();
    }
}
