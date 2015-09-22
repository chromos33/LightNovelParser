using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace LightNovelHTMLParser
{
    public partial class LightNovelDownloadForm : Form
    {
        List<LightNovel> lightnovels;
        public LightNovelDownloadForm()
        {
            InitializeComponent();
            lightnovels = new List<LightNovel>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LightNovelDownloadForm_Load(object sender, EventArgs e)
        {

        }
        private void LightNovelDownloadForm_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            
            // Ensure the Form remains square (Height = Width).
            SupportedPageList.Height = control.Size.Height - 80;
            SupportedPageList.Width = ((control.Size.Width - 160) / 3) - 30;

            LightNovelAvailableField.Height = control.Size.Height - 80;
            LightNovelAvailableField.Width = ((control.Size.Width - 160) / 3) - 30;
            LightNovelAvailableField.Left = SupportedPageList.Right+20;
            LightNovelListLabel.Left = SupportedPageList.Right + 20;

            LightNovelDownloadList.Height = control.Size.Height - 80;
            LightNovelDownloadList.Width = ((control.Size.Width - 160) / 3)-30;
            LightNovelDownloadList.Left = LightNovelAvailableField.Right + 20;
            downloadlistlabel.Left = LightNovelAvailableField.Right + 20;


        }

        private void searchpage_btn_Click(object sender, EventArgs e)
        {
            // Japtem
            if(SupportedPageList.SelectedItems.Count>0)
            {

                List<Webpage> webpages = new List<Webpage>();
                foreach (String item in SupportedPageList.SelectedItems)
                {
                    Webpage newpage = new Webpage();
                    newpage.html = new System.Net.WebClient().DownloadString(item);
                    webpages.Add(newpage);
                }
                HTMLLightNovelLinkParse(webpages);
            }
        }
        private void HTMLLightNovelLinkParse(List<Webpage> pages)
        {
            LightNovelAvailableField.Items.Clear();
            if (pages.Count > 0)
            {
                foreach(Webpage item in pages)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(item.html);
                    var data = from tr in doc.DocumentNode.Descendants("div") where tr.Id.ToString() == "Label1" select tr;
                    HtmlAgilityPack.HtmlDocument innerhtml = new HtmlAgilityPack.HtmlDocument();
                    foreach(var innterhtmldata in data)
                    {
                        innerhtml.LoadHtml(innterhtmldata.InnerHtml);
                    }
                    var VNlinks = from tr in innerhtml.DocumentNode.Descendants("a") select tr.Attributes["href"];
                    var VNTitles = from tr in innerhtml.DocumentNode.Descendants("a") select tr;
                    for(int i = 0; i< VNlinks.Count() && i< VNTitles.Count();i++)
                    {
                        if(!VNTitles.ToArray()[i].InnerHtml.ToString().Contains("<img"))
                        {
                            LightNovel tempnovel = new LightNovel(VNlinks.ToArray()[i].Value.ToString(), VNTitles.ToArray()[i].InnerHtml.ToString());
                            LightNovelAvailableField.Items.Add(tempnovel.Title.ToString());
                            lightnovels.Add(tempnovel);
                        }
                    }
                    innerhtml = null;
                    doc = null;
                }
            }
                
        }
        

        private void download_btn_Click(object sender, EventArgs e)
        {
            // download visual novel content here
        }

        private void chapterscan_btn_Click(object sender, EventArgs e)
        {
            List<Webpage> webpages = new List<Webpage>();
            foreach (var item in LightNovelAvailableField.SelectedIndices)
            {
                Webpage newpage = new Webpage();
                newpage.html = new System.Net.WebClient().DownloadString(lightnovels.ToArray()[Int32.Parse(item.ToString())].Link);
                newpage.id = Int32.Parse(item.ToString());
                webpages.Add(newpage);
            }
            HTMLChapterListParse(webpages);
        }
        private void HTMLChapterListParse(List<Webpage> pages)
        {
            foreach(Webpage page in pages)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page.html);
                var data = from nodes in doc.DocumentNode.Descendants("article") where nodes.Attributes["class"].Value.ToString().Contains("post hentry") select nodes;
                foreach(var dataentry in data)
                {
                    HtmlAgilityPack.HtmlDocument entry = new HtmlAgilityPack.HtmlDocument();
                    entry.LoadHtml(dataentry.InnerHtml);
                    var entrydata = from innernodes in entry.DocumentNode.Descendants("a") where innernodes.Attributes["href"] != null select innernodes;
                    foreach(var datanode in entrydata)
                    {
                            if(datanode.InnerHtml.IndexOf("Chapter") > 0)
                            {
                            lightnovels[page.id].addChapter(new Chapter(datanode.Attributes["href"].Value, datanode.InnerHtml));
                        }
                        
                    }
                    entry = null;         
                }
                doc = null;

            }
            LightNovelDownloadList.Items.Clear();
            foreach (LightNovel LNitem in lightnovels)
            {
                foreach(Chapter chapter in LNitem.Chapter)
                {
                    LightNovelDownloadList.Items.Add(chapter.Title);
                }
            }
            
        }
    }
}
