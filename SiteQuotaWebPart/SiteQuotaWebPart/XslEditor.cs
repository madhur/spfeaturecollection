using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Xml;
using System.Web.UI.WebControls;

namespace SiteQuotaWebPart
{
    public class XslEditor : EditorPart
    {
        private TextBox _xslText;
      //  private Button _xslButton;

        public override bool ApplyChanges()
        {
            SiteQuotaWebPart quotaWebPart = this.WebPartToEdit as SiteQuotaWebPart;

            if (quotaWebPart != null)
            {
                this.EnsureChildControls();
                quotaWebPart.XslText = _xslText.Text;
            }

            return (quotaWebPart != null);
        }

        public override void SyncChanges()
        {
            SiteQuotaWebPart quotaWebPart = this.WebPartToEdit as SiteQuotaWebPart;
            if (quotaWebPart != null)
            {
                this.EnsureChildControls();
                _xslText.Text = quotaWebPart.XslText;
            }
        }

        protected override void CreateChildControls()
        {
            this._xslText = new TextBox();
            _xslText.TextMode = TextBoxMode.MultiLine;
            _xslText.Height = Unit.Pixel(200);
            this.Controls.Add(_xslText);

            
            base.CreateChildControls();
        }

        
 

    } 
}
