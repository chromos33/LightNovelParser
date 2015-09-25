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
        private void LightNovelDownloadForm_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            // Ensure the Form remains square (Height = Width).
            SupportedPageList.Height = control.Size.Height - 80;
            SupportedPageList.Width = ((control.Size.Width - 160) / 3) - 30;

            LightNovelAvailableField.Height = control.Size.Height - 80;
            LightNovelAvailableField.Width = ((control.Size.Width - 160) / 3) - 30;
            LightNovelAvailableField.Left = SupportedPageList.Right + 20;
            LightNovelListLabel.Left = SupportedPageList.Right + 20;

            LightNovelDownloadList.Height = control.Size.Height - 80;
            LightNovelDownloadList.Width = ((control.Size.Width - 160) / 3) - 30;
            LightNovelDownloadList.Left = LightNovelAvailableField.Right + 20;
            downloadlistlabel.Left = LightNovelAvailableField.Right + 20;


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LightNovelDownloadForm_Load(object sender, EventArgs e)
        {
          
        }
        

        private void searchpage_btn_Click(object sender, EventArgs e)
        {
            // Japtem
            if(SupportedPageList.CheckedItems.Count>0)
            {

                List<Webpage> webpages = new List<Webpage>();
                foreach (String item in SupportedPageList.CheckedItems)
                {
                    Webpage newpage = new Webpage();
                    newpage.html = new System.Net.WebClient().DownloadString(item);
                    newpage.link = item;
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
                    #region japtem
                    if(item.link.Contains("japtem"))
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(item.html);
                        var data = from tr in doc.DocumentNode.Descendants("div") where tr.Id.ToString() == "Label1" select tr;
                        HtmlAgilityPack.HtmlDocument innerhtml = new HtmlAgilityPack.HtmlDocument();
                        foreach (var innterhtmldata in data)
                        {
                            innerhtml.LoadHtml(innterhtmldata.InnerHtml);
                        }
                        var VNlinks = from tr in innerhtml.DocumentNode.Descendants("a") select tr.Attributes["href"];
                        var VNTitles = from tr in innerhtml.DocumentNode.Descendants("a") select tr;
                        for (int i = 0; i < VNlinks.Count() && i < VNTitles.Count(); i++)
                        {
                            if (!VNTitles.ToArray()[i].InnerHtml.ToString().Contains("<img"))
                            {
                                LightNovel tempnovel = new LightNovel(VNlinks.ToArray()[i].Value.ToString(), VNTitles.ToArray()[i].InnerHtml.ToString());
                                LightNovelAvailableField.Items.Add(tempnovel);
                            }
                        }
                        innerhtml = null;
                        doc = null;
                    }
                    #endregion
                    #region baka-tsuki
                    if (item.link.Contains("baka-tsuki"))
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(item.html);
                        var data = doc.DocumentNode.Descendants("div").Where(firstquery => firstquery.Attributes["class"] != null).Where(secondquery => secondquery.Attributes["class"].Value.Contains("mw-content-ltr")).Select(thirdquery => thirdquery.Descendants("a").Where((fourthquery => thirdquery.Attributes["Class"] != null)));
                        //var data = from tr in doc.DocumentNode.Descendants("div") where tr.Attributes["class"] != null select tr;
                        foreach (var dataitem in data)
                        {
                            foreach(var linkitem in dataitem)
                            {
                                if(!(linkitem.InnerHtml.ToLower().Contains("baka-tsuki") || linkitem.InnerHtml.ToLower().Contains("teaser project list")))
                                {
                                    LightNovel tempnovel = new LightNovel("https://www.baka-tsuki.org" + linkitem.Attributes["href"].Value, linkitem.InnerHtml);
                                    LightNovelAvailableField.Items.Add(tempnovel);
                                }
                            }
                            
                        }
                    }
                    #endregion

                }
            }
                
        }
        

        private void chapterscan_btn_Click(object sender, EventArgs e)
        {
            List<Webpage> webpages = new List<Webpage>();
            foreach (LightNovel item in LightNovelAvailableField.CheckedItems)
            {
                if (item.Link.Contains("japtem"))
                {
                    Webpage newpage = new Webpage();
                    newpage.Lightnovels.Add(item);
                    newpage.html = new System.Net.WebClient().DownloadString(item.Link + "?max-results=300");
                    webpages.Add(newpage);
                }
                if (item.Link.Contains("baka-tsuki"))
                {
                    
                    Webpage newpage = new Webpage();
                    newpage.Lightnovels.Add(item);
                    newpage.html = new System.Net.WebClient().DownloadString(item.Link);
                    newpage.link = item.Link;
                    webpages.Add(newpage);
                }

            }
            HTMLChapterListParse(webpages);
        }
        private void HTMLChapterListParse(List<Webpage> pages)
        {
            LightNovelDownloadList.Items.Clear();
            foreach (Webpage page in pages)
            {
                #region japtem
                if (page.link.Contains("japtem"))
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
                        for (int i = entrydatarray.Count() - 1; i >= 0; i--)
                        {
                            if (entrydatarray[i].InnerHtml.IndexOf("Chapter") > 0)
                            {
                                foreach (LightNovel chaptertest in page.Lightnovels)
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
                #endregion
                if (page.link.Contains("baka-tsuki"))
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(page.html);
                    var data = doc.DocumentNode.Descendants("dl").Select(a => a.Descendants("a"));
                    int volume = 1;
                    int i = 1;
                    foreach (var dataitems in data)
                    {
                        if(i % 2 != 0)
                        {
                            Chapter chapter = null;
                            foreach (var dataitem in dataitems)
                            {
                                if (dataitem.Attributes["class"] != null)
                                {
                                    if(dataitem.Attributes["class"].Value.Contains("external"))
                                    {
                                        chapter = new Chapter(dataitem.Attributes["href"].Value, "Volume " + volume + " " + dataitem.InnerHtml);
                                    }
                                    else
                                    {
                                        if (!(dataitem.Attributes["title"].Value.Contains("page does not exist")))
                                        {
                                            if(!(dataitem.Attributes["title"].Value.ToLower().Contains("user")))
                                            {
                                                chapter = new Chapter("https://www.baka-tsuki.org" + dataitem.Attributes["href"].Value, dataitem.Attributes["title"].Value);
                                            }
                                            
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    if (!(dataitem.Attributes["title"].Value.Contains("page does not exist")))
                                    {
                                        if (!(dataitem.Attributes["title"].Value.ToLower().Contains("user")))
                                            {
                                            chapter = new Chapter("https://www.baka-tsuki.org" + dataitem.Attributes["href"].Value, dataitem.Attributes["title"].Value);
                                        }
                                    }
                                }
                                if(chapter != null)
                                {
                                    foreach (LightNovel chaptertest in page.Lightnovels)
                                    {
                                        if (chaptertest.Title.ToLower().Contains(chapter.Title.ToLower().Split(':')[0]))
                                        {
                                            chaptertest.addChapter(chapter);
                                        }
                                    }
                                    LightNovelDownloadList.Items.Add(chapter);
                                }
                                chapter = null;
                                
                            }
                            volume++;

                        }
                        i++;

                    }
                }

            }
        }

        private void download_btn_Click(object sender, EventArgs e)
        {
            if (LightNovelDownloadList.CheckedItems.Count > 0)
            {

                List<Webpage> webpages = new List<Webpage>();

                foreach (Chapter item in LightNovelDownloadList.CheckedItems)
                {
                    Webpage newpage = new Webpage();
                    System.Diagnostics.Debug.WriteLine(item.Link);
                    newpage.html = new System.Net.WebClient().DownloadString(item.Link);
                    item.downloadscheduled = true;
                }
                foreach (LightNovel item in LightNovelAvailableField.CheckedItems)
                {
                    foreach (Chapter chapter in item.Chapter)
                    {
                        System.Net.WebClient client = new System.Net.WebClient();
                        client.Encoding = System.Text.Encoding.UTF8;
                        String html = client.DownloadString(chapter.Link);
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        if (chapter.Link.Contains("japtem"))
                        {
                            var data = from nodes in doc.DocumentNode.Descendants("div") where nodes.Attributes["class"] != null && nodes.Attributes["class"].Value.Contains("post-body entry-content") select nodes;
                            foreach (var innterhtmldata in data)
                            {
                                chapter.html = innterhtmldata.InnerHtml;
                            }
                            foreach (Chapter test in LightNovelDownloadList.CheckedItems)
                            {
                                if (test.Title == chapter.Title)
                                {
                                    chapter.downloadscheduled = true;
                                }
                            }
                        }
                        if(chapter.Link.Contains("baka-tsuki"))
                        {
                            var data = doc.DocumentNode.Descendants("div").Where(div => div.Attributes["id"] != null).Where(precisediv => precisediv.Attributes["id"].Value == "content" || precisediv.Attributes["id"].Value == "mw-content-text");
                            foreach (var dataitems in data)
                            {
                                chapter.html = dataitems.InnerHtml;
                            }
                            foreach (Chapter test in LightNovelDownloadList.CheckedItems)
                            {
                                if (test.Title == chapter.Title)
                                {
                                    chapter.downloadscheduled = true;
                                }
                            }
                        }

                        //put this back in after testin phase to lessen the load on webserver
                        //System.Threading.Thread.Sleep(2000);
                    }
                }
                System.IO.Directory.CreateDirectory("temp");
                string dir = Directory.GetCurrentDirectory();
                dir = dir + "\\temp\\";
                System.Threading.Thread.Sleep(1000);
                // loop through Selected Lightnovels here and generate Epub from chapterhtml
                foreach (LightNovel lightnovel in LightNovelAvailableField.CheckedItems)
                {
                    //loop chapters
                    List<Epub4Net.Chapter> chapters = new List<Epub4Net.Chapter>();
                    foreach (Chapter chapter in lightnovel.Chapter)
                    {
                        if (chapter.downloadscheduled)
                        {
                            string filename = chapter.Title.Replace(" ", "_") + ".html";
                            string file = filename.Replace(":", "");
                            filename = dir + filename.Replace(":", "");
                            string enhanced = "";
                            if (chapter.html.IndexOf("<table") >= 0)
                            {
                                enhanced = chapter.html.Substring(0, chapter.html.IndexOf("<table"));
                            }
                            else
                            {
                                enhanced = chapter.html;
                            }
                            
                            string htmlcode = "<?xml version='1.0' encoding='utf-8'?><html xmlns='http://www.w3.org/1999/xhtml' lang='de' xml:lang='de'><head><title>Unknown</title>< meta http - equiv = 'Content-Type' content = 'text/html; charset=utf-8'/></head><body>" + enhanced + "</body></html>";
                            System.IO.File.WriteAllText(filename, htmlcode, Encoding.UTF8);
                            chapters.Add(new Epub4Net.Chapter(filename, file, chapter.Title));
                        }
                    }
                    // TODO:Check why epub is invalid

                    Epub4Net.Epub epub = new Epub4Net.Epub(lightnovel.Title, "LightNovelParser", chapters);
                    epub.BookId = "1";
                    epub.Language = "en";
                    epub.Publisher = "LightNovelParser";
                    epub.Subject = "Entertainment";
                    Epub4Net.EPubBuilder builder = new Epub4Net.EPubBuilder();
                    var epubFilePath = builder.Build(epub);


                }
                string[] filePaths = Directory.GetFiles(dir);
                foreach (var path in filePaths)
                {
                    //File.Delete(path);
                }
                MessageBox.Show("finished");
            }
        }
    }
}
