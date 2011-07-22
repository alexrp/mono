// 
// event-004.il
//  
// Author:
//       Alex Rønne Petersen <alex@alexrp.com>
// 
// Copyright (c) 2011 Alex Rønne Petersen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

.assembly extern mscorlib
{
}

.class test004_cls
{
	.method void add_test004 (class [mscorlib]System.Action act)
	{
		ret
	}

	.method void remove_test004 (class [mscorlib]System.Action act)
	{
		ret
	}

	.method void other_test004_0 ()
	{
		ret
	}

	.method void other_test004_1 ()
	{
		ret
	}

	.method void other_test004_2 ()
	{
		ret
	}

	.event [mscorlib]System.Action test003
	{
		.addon void test004_cls::add_test004 (class [mscorlib]System.Action act)
		.removeon void test004_cls::remove_test004 (class [mscorlib]System.Action act)
		.other void test004_cls::other_test004_0 ()
		.other void test004_cls::other_test004_1 ()
		.other void test004_cls::other_test004_2 ()
	}
}
