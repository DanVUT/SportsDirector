using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace MFormat.Model
{
    //Singleton class holding program settings
    public class Settings : INotifyPropertyChanged
    {
        //Folder in documents containing settings file
        private string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/SportsDirector";
        //Property holding folder of videos
        private string videosFolder = null;
        public string VideosFolder
        { 
            get { return videosFolder; } 
            set {
                if (value != this.videosFolder)
                {
                    this.videosFolder = value;
                    OnPropertyChanged("VideosFolder");
                }
            } 
        }
        //Property holding folder of images
        private string imagesFolder = null;
        public string ImagesFolder 
        { 
            get { return imagesFolder; } 
            set {
                if (value != this.imagesFolder)
                {
                    this.imagesFolder = value;
                    OnPropertyChanged("ImagesFolder");
                }
            } 
        }
        //Property holding destination for overlays
        private string overlayFolder = null;
        public string OverlayFolder { get { return overlayFolder; } set { this.overlayFolder = value; OnPropertyChanged("OverlayFolder"); } }

        #region SINGLETON DECLARATION
        static Settings()
        {
        }
        private Settings()
        {
            if(!Directory.Exists(this.documentsFolder))
            {
                Directory.CreateDirectory(documentsFolder);
            }
            LoadSettings();
        }
        public static Settings Instance {get;} = new Settings();
        #endregion

        //Method saving settings into XML
        public void SaveSettings()
        {
            try
            {
                XmlWriter writer = XmlWriter.Create(this.documentsFolder + @"/settings.xml");
                writer.WriteStartDocument();

                writer.WriteStartElement("settings");

                if (VideosFolder != null)
                {
                    writer.WriteStartElement("videosfolder");
                    writer.WriteString(this.VideosFolder);
                    writer.WriteEndElement();
                }

                if (ImagesFolder != null)
                {
                    writer.WriteStartElement("imagesfolder");
                    writer.WriteString(this.ImagesFolder);
                    writer.WriteEndElement();
                }

                if (OverlayFolder != null)
                {
                    writer.WriteStartElement("overlayfolder");
                    writer.WriteString(this.OverlayFolder);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Problem creating XML settings file");
            }
        }
        //Method loading settings on program startup from XML
        public void LoadSettings()
        {
            if (File.Exists(this.documentsFolder + @"/settings.xml")) {
                try
                {
                    XmlReader reader = XmlReader.Create(this.documentsFolder + @"/settings.xml");


                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "videosfolder":
                                    reader.Read();
                                    this.VideosFolder = reader.Value;
                                    break;
                                case "imagesfolder":
                                    reader.Read();
                                    this.ImagesFolder = reader.Value;
                                    break;
                                case "overlayfolder":
                                    reader.Read();
                                    this.OverlayFolder = reader.Value;
                                    break;
                            }
                        }
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("XML Parsing problem");
                }
            }
        }

        #region onPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged; 

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
