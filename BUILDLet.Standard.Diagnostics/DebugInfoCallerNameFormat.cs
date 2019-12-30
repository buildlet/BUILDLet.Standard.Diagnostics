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

namespace BUILDLet.Standard.Diagnostics
{
    /// <summary>
    /// <see cref="DebugInfo"/> クラスで呼び出し元メソッド名の表示形式を表します。
    /// </summary>
    public enum DebugInfoCallerNameFormat
    {
        // /// <summary>
        // /// クラス名もメソッド名も表示しません。
        // /// </summary>
        // None,

        /// <summary>
        /// メソッド名のみを表示します。
        /// </summary>
        Name,

        /// <summary>
        /// クラス名とメソッド名を表示します。
        /// </summary>
        ShortName,

        /// <summary>
        /// クラス名を含むメソッド名を完全修飾名で表示します。
        /// </summary>
        FullName,

        /// <summary>
        /// クラス名のみを表示します。
        /// </summary>
        ClassName,

        /// <summary>
        /// クラス名のみを完全修飾名で表示します。
        /// </summary>
        FullClassName
    }
}
