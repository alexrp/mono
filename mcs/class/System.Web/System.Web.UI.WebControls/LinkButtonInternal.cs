/**
 * Namespace: System.Web.UI.WebControls
 * Class:     LinkButtonInternal
 * 
 * Author:  Gaurav Vaish
 * Maintainer: gvaish@iitk.ac.in
 * Contact: <my_scripts2001@yahoo.com>, <gvaish@iitk.ac.in>
 * Implementation: yes
 * Status:  100%
 * 
 * (C) Gaurav Vaish (2002)
 */

using System;
using System.Drawing;
using System.Web;
using System.Web.UI;

namespace System.Web.UI.WebControls
{
	internal class LinkButtonInternal : LinkButton
	{
		public LinkButtonInternal() : base()
		{
		}
		
		public override void Render(HtmlTextWriter writer)
		{
			SetForeColor();
			Render(writer);
		}
		
		private void SetForeColor()
		{
			if(!ControlStyle.IsSet(Style.FORECOLOR))
			{
				Control ctrl = this;
				Color   foreCol;
				int     ctr = 0;
				//FIXME: Should it be 3 or 2? this-> LinkButton-> WebControl-> Control
				// But control does not have any ForeColor. Need to test.
				while(ctr < 3)
				{
					ctrl = ctrl.Parent;
					foreCol = ctrl.ForeColor;
					if(foreCol != Color.Empty)
					{
						ForeColor = foreCol;
						return;
					}
					ctr++;
				}
			}
		}
	}
}
