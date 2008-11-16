using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
namespace SiteQuotaWebPart
{
    public class SiteQuotaWebPart:WebPart
    {
        [Personalizable(PersonalizationScope.Shared)]
        public string XslText
        {
            get;
            set;
        }


        protected override void CreateChildControls()
        {
            int quotaUse = 0;

            SPSite mySite = SPContext.Current.Site;
            long maxLevel=0;
            long usage=0;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite elevatedSite = new SPSite(mySite.ID))
                {
                    maxLevel = elevatedSite.Quota.StorageMaximumLevel;
                    usage = elevatedSite.Usage.Storage;

                    if (maxLevel != 0)
                    {
                        quotaUse = Convert.ToInt32(usage * 100 / maxLevel);
                    }



                }
            });

            //MemoryStream ms = new MemoryStream();
            //StringBuilder sb=new StringBuilder();
            StringWriter sw = new StringWriter();
            XmlTextWriter textWriter = new XmlTextWriter(sw);
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("SPWeb");
            textWriter.WriteElementString("StorageMaximumLeveL", maxLevel.ToString());
            textWriter.WriteElementString("Storage", usage.ToString());
            textWriter.WriteEndDocument();
            textWriter.Close();


          

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(sw.ToString());

            XslCompiledTransform xmlTransform = new XslCompiledTransform();
            Stream xsltStream=this.GetType().Assembly.GetManifestResourceStream("SiteQuotaWebPart.Default.xslt");

            XPathDocument xPathDoc = new XPathDocument(xsltStream);

            
            xmlTransform.Load(xPathDoc);
            MemoryStream ms=new MemoryStream();
            StringWriter sww=new StringWriter();
            XmlTextWriter xw=new XmlTextWriter(sww);
            xmlTransform.Transform(xmldoc, xw);

            this.Controls.Add(new LiteralControl(sww.ToString()));
            this.SetPersonalizationDirty();
            //textWriter.WriteString(maxLevel.ToString());
            

            base.CreateChildControls();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            XslEditor xsl = new XslEditor();
            xsl.Title = "Edit Xsl";
            xsl.ID = "xslid";
            editors.Add(xsl);

            return new EditorPartCollection(editors);
            //return base.CreateEditorParts();
        }

        
    }
}
