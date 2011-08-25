// 
// QualifiedName.cs
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono.ILAsm {
	internal sealed class QualifiedName {
		public QualifiedName ()
		{
			Name = string.Empty;
			Namespaces = new List<string> ();
			Nestings = new List<string> ();
		}
		
		public string Name { get; set; }
		
		public List<string> Namespaces { get; private set; }
		
		public List<string> Nestings { get; private set; }
		
		public string FullNamespace {
			get {
				var ns = new StringBuilder ();
				
				for (var i = 0; i < Namespaces.Count; i++) {
					ns.Append (Namespaces [i]);
					
					if (i != Namespaces.Count - 1)
						ns.Append (".");
				}
				
				return ns.ToString ();
			}
		}
		
		public string FullName {
			get {
				var name = new StringBuilder (FullNamespace);
				if (name.Length > 0)
					name.Append (".");
				
				foreach (var nesting in Nestings) {
					name.Append (nesting);
					name.Append ("/");
				}
				
				name.Append (Name);
				
				return name.ToString ();
			}
		}
		
		public override string ToString ()
		{
			return FullName;
		}
	}
}
