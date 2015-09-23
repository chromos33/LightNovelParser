using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;

namespace LightNovelHTMLParser
{
    public partial class LightNovelDownloadForm : Form
    {
        public LightNovelDownloadForm()
        {
            InitializeComponent();
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
                            LightNovelAvailableField.Items.Add(tempnovel);
                        }
                    }
                    innerhtml = null;
                    doc = null;
                }
            }
                
        }
        

        private void download_btn_Click(object sender, EventArgs e)
        {
            if (LightNovelDownloadList.SelectedItems.Count > 0)
            {

                List<Webpage> webpages = new List<Webpage>();

                foreach (Chapter item in LightNovelDownloadList.SelectedItems)
                {
                    Webpage newpage = new Webpage();
                    newpage.html = new System.Net.WebClient().DownloadString(item.Link);
                    item.downloadscheduled = true;
                }
                foreach(LightNovel item in LightNovelAvailableField.SelectedItems)
                {
                    foreach(Chapter chapter in item.Chapter)
                    {
                        String html = new System.Net.WebClient().DownloadString(chapter.Link);
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        var data = from nodes in doc.DocumentNode.Descendants("div") where nodes.Attributes["class"] != null && nodes.Attributes["class"].Value.Contains("post-body entry-content") select nodes;
                        foreach (var innterhtmldata in data)
                        {
                            chapter.html = innterhtmldata.InnerHtml;
                        }
                        foreach(Chapter test in LightNovelDownloadList.SelectedItems)
                        {
                            //TODO: Check why test.Title is not set sometimes
                            System.Diagnostics.Debug.WriteLine(test.Title);
                            System.Diagnostics.Debug.WriteLine(chapter.Title);
                            if (test.Title == chapter.Title)
                            {
                                chapter.downloadscheduled = true;
                            }
                        }
                        
                        //put this back in after testin phase to lessen the load on webserver
                        //System.Threading.Thread.Sleep(2000);
                    }
                }
                System.IO.Directory.CreateDirectory("temp");
                string dir = Directory.GetCurrentDirectory();
                dir = dir +"\\temp\\";
                System.Threading.Thread.Sleep(1000);
                // loop through Selected Lightnovels here and generate Epub from chapterhtml
                foreach(LightNovel lightnovel in LightNovelAvailableField.SelectedItems)
                {
                    //loop chapters
                    List<Epub4Net.Chapter> chapters = new List<Epub4Net.Chapter>();
                    foreach(Chapter chapter in lightnovel.Chapter)
                    {
                        if(chapter.downloadscheduled)
                        {
                            string filename = chapter.Title.Replace(" ", "_") + ".html";
                            string file = filename.Replace(":", "");
                            filename = dir + filename.Replace(":", "");
                            string htmlcode = "<html><head></head><body>"+chapter.html+"</body></html>";
                            System.IO.File.WriteAllText(filename, htmlcode, Encoding.UTF8);
                            //chapters.Add(new Epub4Net.Chapter(filename, file, chapter.Title));
                        }
                    }
                    // TODO:Check why epub is invalid
                    /*
                    Epub4Net.Epub epub = new Epub4Net.Epub(lightnovel.Title, "LightNovelParser", chapters);
                    epub.BookId = "1";
                    epub.Language = "en";
                    epub.Publisher = "LightNovelParser";
                    epub.Subject = "Entertainment";
                    Epub4Net.EPubBuilder builder = new Epub4Net.EPubBuilder();
                    var epubFilePath = builder.Build(epub);
                    */
                    
                }
                //string[] filePaths = Directory.GetFiles(dir);
                //foreach(var path in filePaths)
                //{
                    //File.Delete(path);
                //}
                MessageBox.Show("finished");
            }
        }

        private void chapterscan_btn_Click(object sender, EventArgs e)
        {
            List<Webpage> webpages = new List<Webpage>();
            foreach (LightNovel item in LightNovelAvailableField.SelectedItems)
            {
                Webpage newpage = new Webpage();
                newpage.Lightnovels.Add(item);
                newpage.html = new System.Net.WebClient().DownloadString( item.Link + "?max-results=300");
                webpages.Add(newpage);
            }
            HTMLChapterListParse(webpages);
        }
        private void HTMLChapterListParse(List<Webpage> pages)
        {
            LightNovelDownloadList.Items.Clear();
            foreach (Webpage page in pages)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page.html);
                var data = from nodes in doc.DocumentNode.Descendants("article") where nodes.Attributes["class"].Value.ToString().Contains("post hentry") select nodes;
                HtmlAgilityPack.HtmlNode[] dataarray = data.ToArray();
                for (int j = dataarray.Count() - 1; j >= 0; j--)
                {
                    HtmlAgilityPack.HtmlDocument entry = new HtmlAgilityPack.HtmlDocument();
                    entry.LoadHtml(dataarray[j].InnerHtml);
                    var entrydata = from innernodes in entry.DocumentNode.Descendants("a") where innernodes.Attributes["href"] != null select innernodes;
                    HtmlAgilityPack.HtmlNode[] entrydatarray = entrydata.ToArray();
                    for(int i = entrydatarray.Count()-1;i >=0;i--)
                    {
                        if(entrydatarray[i].InnerHtml.IndexOf("Chapter") > 0)
                        {
                            foreach(LightNovel chaptertest in page.Lightnovels)
                            {
                                Console.WriteLine(chaptertest.Title.ToLower());
                                Console.WriteLine(entrydata.ToArray()[i].InnerHtml.ToLower());
                                if (chaptertest.Title.ToLower().Contains(entrydata.ToArray()[i].InnerHtml.ToLower().Split(':')[0]))
                                {
                                    chaptertest.addChapter(new Chapter(entrydatarray[i].Attributes["href"].Value, entrydata.ToArray()[i].InnerHtml));
                                }
                            }
                            LightNovelDownloadList.Items.Add(new Chapter(entrydatarray[i].Attributes["href"].Value, entrydata.ToArray()[i].InnerHtml));
                        }
                    }
                    entry = null;         
                }
                doc = null;
            }
        }
    }
}
