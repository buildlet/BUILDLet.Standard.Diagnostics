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
        public abstract class CallerNameTestParameterBase : TestParameter<string>
        {
            public DebugInfoCallerNameFormat Format;
            public string ExpectedCallerName;
        }

        public class GetCallerNameTestParameter : CallerNameTestParameterBase
        {
            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = this.ExpectedCallerName;

                // Initialize DebugInfo
                DebugInfo.Init(this.Format);
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = new DebugInfoTests().GetCallerNameTestMethod();
            }
        }

        public string GetCallerNameTestMethod() => DebugInfo.GetCallerName();

        public class CallerNameTestParameter : CallerNameTestParameterBase
        {
            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = this.ExpectedCallerName;
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = new DebugInfoTests().CallerNameTestMethod(this);
            }
        }

        public string CallerNameTestMethod(CallerNameTestParameter param) => param.Format switch
        {
            DebugInfoCallerNameFormat.Name => DebugInfo.Name,
            DebugInfoCallerNameFormat.ShortName => DebugInfo.ShortName,
            DebugInfoCallerNameFormat.FullName => DebugInfo.FullName,
            DebugInfoCallerNameFormat.ClassName => DebugInfo.ClassName,
            DebugInfoCallerNameFormat.FullClassName => DebugInfo.FullClassName,
            _ => throw new InvalidOperationException(),
        };

        [DataTestMethod()]
        [DataRow(DebugInfoCallerNameFormat.Name, nameof(GetCallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(GetCallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(GetCallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ClassName, "DebugInfoTests")]
        [DataRow(DebugInfoCallerNameFormat.FullClassName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests")]
        public void GetCallerNameTest(DebugInfoCallerNameFormat format, string caller)
        {
            // SET Parameter
            GetCallerNameTestParameter param = new GetCallerNameTestParameter
            {
                Keyword = format.ToString(),
                Format = format,
                ExpectedCallerName = caller,
            };

            // ASSERT
            param.Assert();
        }

        [DataTestMethod()]
        [DataRow(DebugInfoCallerNameFormat.Name, nameof(CallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ShortName, "DebugInfoTests." + nameof(CallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.FullName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests." + nameof(CallerNameTestMethod))]
        [DataRow(DebugInfoCallerNameFormat.ClassName, "DebugInfoTests")]
        [DataRow(DebugInfoCallerNameFormat.FullClassName, "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests")]
        public void CallerNameTest(DebugInfoCallerNameFormat format, string caller)
        {
            // SET Parameter
            CallerNameTestParameter param = new CallerNameTestParameter()
            {
                Keyword = format.ToString(),
                Format = format,
                ExpectedCallerName = caller
            };

            // ASSERT
            param.Assert();
        }


        [TestMethod()]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void TimeStampFormatExceptionTest()
        {
            DebugInfo.TimeStampFormat = "!";
        }


        // for TimeStamp Tests
        public abstract class TimeStampTestParameterBase : TestParameter<string>
        {
            public string Format;
            public string CultureName;
        }

        // for GetTimeStamp Tests
        public class GetTimeStampTestParameter : TimeStampTestParameterBase
        {
            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = DateTime.Now.ToString(
                    (string.IsNullOrEmpty(this.Format)) ? "yyyy/MM/dd-HH:mm:ss" : this.Format,
                    (this.CultureName is null) ? new CultureInfo("en-US") : new CultureInfo(this.CultureName));


                // Initialize DebugInfo
                if (this.Format is null)
                {
                    if (this.CultureName is null)
                    {
                        DebugInfo.Init();
                    }
                    else
                    {
                        DebugInfo.Init(culture: new CultureInfo(this.CultureName));
                    }
                }
                else
                {
                    if (this.CultureName is null)
                    {
                        DebugInfo.Init(format: this.Format);
                    }
                    else
                    {
                        DebugInfo.Init(format: this.Format, culture: new CultureInfo(this.CultureName));
                    }
                }
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = DebugInfo.GetTimeStamp();
            }
        }

        // for Date Tests
        public class DateTestParameter : TimeStampTestParameterBase
        {
            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = System.DateTime.Now.ToString(this.Format, new CultureInfo(this.CultureName));
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = DebugInfo.Date;
            }
        }

        // for Time Tests
        public class TimeTestParameter : TimeStampTestParameterBase
        {
            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = System.DateTime.Now.ToString(this.Format, new CultureInfo(this.CultureName));
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = DebugInfo.Time;
            }
        }

        // for ToString Tests
        public class ToStringTestParameter : TimeStampTestParameterBase
        {
            public string CallerName;

            public override void Arrange(out string expected)
            {
                // SET Expected
                expected = System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss", new CultureInfo("en-US")) + "," + this.CallerName;
            }

            public override void Act(out string actual)
            {
                // GET Actual
                actual = new DebugInfoTests().ToStringTestMethod();
            }
        }

        public string ToStringTestMethod() => DebugInfo.ToString();


        [DataTestMethod()]
        [DataRow(null, null, DisplayName = "Default")]
        [DataRow(null, "ja-JP")]
        [DataRow("G", null)]
        [DataRow("G", "ja-JP")]
        [DataRow("D", "ja-JP")]
        [DataRow("F", "ja-JP")]
        public void GetTimeStampTest(string format, string cultureName)
        {
            // SET Parameter
            GetTimeStampTestParameter param = new GetTimeStampTestParameter
            {
                Keyword = (format is null ? "Default" : $"\"{format}\"") + " (" + (cultureName is null ? "Default" : $"\"{cultureName}\"") + ")",
                Format = format,
                CultureName = cultureName,
            };

            // ASSERT
            param.Assert();
        }


        [TestMethod()]
        public void DateTest()
        {
            // SET Parameter
            DateTestParameter param = new DateTestParameter
            {
                Format = "yyyy/MM/dd",
                CultureName = "en-US",
            };

            // ASSERT
            param.Assert();
        }


        [TestMethod()]
        public void TimeTest()
        {
            // SET Parameter
            TimeTestParameter param = new TimeTestParameter()
            {
                Format = "HH:mm:ss",
                CultureName = "en-US",
            };

            // ASSERT
            param.Assert();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            // ARRANGE & ACT
            ToStringTestParameter param = new ToStringTestParameter
            {
                CallerName = "BUILDLet.Standard.Diagnostics.Tests.DebugInfoTests.ToStringTestMethod",
            };

            // ASSERT
            param.Assert();
        }
    }
}
