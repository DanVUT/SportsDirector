using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace MFormat.Model
{
    /*
        Class Representing Action. It holds action name, recording filename, action start time in seconds (From the beginning of the record file), action end time in seconds (from the beginning of recording in seconds)
    */
    public class Action
    {
        public Action() { }
        public string name { get; set; } = "";
        public string filename { get; set; } = "";
        public int createTime { get; set; } = 0;
        public int before { get; set; } = 0;
        public int after { get; set; } = 0;
        public int startTime { get { return (createTime - before > 0) ? createTime - before : 0; }}
        public int endTime { get { return createTime + after; } }
        public BitmapSource _thumbnail;
        public BitmapSource thumbnail { get; set; }
    }
    //Singleton class Actions is wrapper containing all actions.
    public class Actions : INotifyPropertyChanged
    {
        //Single instance is created here
        private static readonly Actions instance = new Actions();
        //List of all actions
        private ObservableCollection<Action> actions = new ObservableCollection<Action>();
        //Number of actions used for creating names
        private int actionNumber = 1;
        //How many seconds before short action button was clicked should be considered
        public int ShortBefore { get; set; } = 1;
        //How many seconds after short action button was clicked should be considered
        public int ShortAfter { get; set; } = 3;
        //How many seconds before medium action button was clicked should be considered
        public int MediumBefore { get; set; } = 2;
        //How many seconds after medium action button was clicked should be considered
        public int MediumAfter { get; set; } = 5;
        //How many seconds before long action button was clicked should be considered
        public int LongBefore { get; set; } = 3;
        //How many seconds after long action button was clicked should be considered
        public int LongAfter { get; set; } = 10;

        private string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/SportsDirector";
        static Actions()
        {
        }

        private Actions()
        {
            this.LoadActionsSettings();
        }

        public static Actions Instance
        {
            get
            {
                return instance;
            }
        }
        //Method creating short action. It creates background worker (another thread) and starts it, so the main program does not lag during this process.
        public void AddShortAction(DateTime recordingStartTime, string recordingFileName)
        {
            List<object> arguments = new List<object>();
            arguments.Add(recordingStartTime);
            arguments.Add(recordingFileName);
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += AddShortActionThread;
            backgroundWorker.RunWorkerCompleted += ((s,e) => OnPropertyChanged());
            backgroundWorker.RunWorkerAsync(arguments);
        }
        //Thread body of short action. The main reason here is to create thumbnail in second thread, because it is a long taking operation which causes main thread to lag
        private void AddShortActionThread(object sender, DoWorkEventArgs e)
        {
            List<object> arguments = e.Argument as List<object>;
            DateTime recordingStartTime = (DateTime) arguments[0];
            string recordingFileName = arguments[1] as string;
            BitmapSource thumbnail = Thumbnailer.Instance.GetThumbnail(recordingFileName, (DateTime.Now - recordingStartTime).TotalSeconds);
            //After creating thumbnail creating new Action and adding it to the actions list must be invoked from the GUI thread.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Action shortAction = new Action();
                shortAction.createTime = (int)(DateTime.Now - recordingStartTime).TotalSeconds;
                shortAction.before = this.ShortBefore;
                shortAction.after = this.ShortBefore;
                shortAction.filename = recordingFileName;
                shortAction.name = "Short Action " + actionNumber++;
                shortAction.thumbnail = thumbnail;
                actions.Add(shortAction);
            });
        }
        //Method creating medium action. Again it creates background worker for creating a thumbnail
        public void AddMediumAction(DateTime recordingStartTime, string recordingFileName)
        {
            List<object> arguments = new List<object>();
            arguments.Add(recordingStartTime);
            arguments.Add(recordingFileName);
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += AddMediumActionThread;
            backgroundWorker.RunWorkerCompleted += ((s, e) => OnPropertyChanged());
            backgroundWorker.RunWorkerAsync(arguments);
        }
        //Thread body for creating medium action
        public void AddMediumActionThread(object sender, DoWorkEventArgs e)
        {
            List<object> arguments = e.Argument as List<object>;
            DateTime recordingStartTime = (DateTime)arguments[0];
            string recordingFileName = arguments[1] as string;
            BitmapSource thumbnail = Thumbnailer.Instance.GetThumbnail(recordingFileName, (DateTime.Now - recordingStartTime).TotalSeconds);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Action mediumAction = new Action();
                mediumAction.createTime = (int)(DateTime.Now - recordingStartTime).TotalSeconds;
                mediumAction.before = this.MediumBefore;
                mediumAction.after = this.MediumAfter;
                mediumAction.filename = recordingFileName;
                mediumAction.name = "Medium Action " + actionNumber++;
                mediumAction.thumbnail = thumbnail;
                actions.Add(mediumAction);
            });
        }
        //Method for creating long action
        public void AddLongAction(DateTime recordingStartTime, string recordingFileName)
        {
            List<object> arguments = new List<object>();
            arguments.Add(recordingStartTime);
            arguments.Add(recordingFileName);
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += AddLongActionThread;
            backgroundWorker.RunWorkerCompleted += ((s, e) => OnPropertyChanged());
            backgroundWorker.RunWorkerAsync(arguments);
        }
        //Thread body for creating long action
        public void AddLongActionThread(object sender, DoWorkEventArgs e)
        {
            List<object> arguments = e.Argument as List<object>;
            DateTime recordingStartTime = (DateTime)arguments[0];
            string recordingFileName = arguments[1] as string;
            BitmapSource thumbnail = Thumbnailer.Instance.GetThumbnail(recordingFileName, (DateTime.Now - recordingStartTime).TotalSeconds);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Action longAction = new Action();
                longAction.createTime = (int)(DateTime.Now - recordingStartTime).TotalSeconds;
                longAction.before = this.LongBefore;
                longAction.after = this.LongAfter;
                longAction.filename = recordingFileName;
                longAction.name = "Long Action " + actionNumber++;
                longAction.thumbnail = thumbnail;
                actions.Add(longAction);
            });
        }
        
        //Actions getter
        public ObservableCollection<Action> GetActions()
        {
            return this.actions;
        }

        public void DeleteAction(Action action)
        {
            this.actions.Remove(action);
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveActionsSettings()
        {
            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(this.documentsFolder + @"/actions.xml");
            }
            catch
            {
                MessageBox.Show("There was a problem with saving action settings into a file");
                return;
            }

            writer.WriteStartDocument();
            writer.WriteStartElement("actions");

            writer.WriteStartElement("action");
            writer.WriteAttributeString("name","Short Action");
            writer.WriteAttributeString("before", this.ShortBefore.ToString());
            writer.WriteAttributeString("after", this.ShortAfter.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("action");
            writer.WriteAttributeString("name", "Medium Action");
            writer.WriteAttributeString("before", this.MediumBefore.ToString());
            writer.WriteAttributeString("after", this.MediumAfter.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("action");
            writer.WriteAttributeString("name", "Long Action");
            writer.WriteAttributeString("before", this.LongBefore.ToString());
            writer.WriteAttributeString("after", this.LongAfter.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        public void LoadActionsSettings()
        {
            if (File.Exists(this.documentsFolder + @"/actions.xml"))
            {
                XmlReader reader = XmlReader.Create(this.documentsFolder + @"/actions.xml");
                try
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "action")
                            {
                                string actionName = reader.GetAttribute("name");
                                switch (actionName)
                                {
                                    case "Short Action":
                                        this.ShortBefore = int.Parse(reader.GetAttribute("before"));
                                        this.ShortAfter = int.Parse(reader.GetAttribute("after"));
                                        break;
                                    case "Medium Action":
                                        this.MediumBefore = int.Parse(reader.GetAttribute("before"));
                                        this.MediumAfter = int.Parse(reader.GetAttribute("after"));
                                        break;
                                    case "Long Action":
                                        this.LongBefore = int.Parse(reader.GetAttribute("before"));
                                        this.LongAfter = int.Parse(reader.GetAttribute("after"));
                                        break;
                                }
                            }
                        }
                    }
                    reader.Close();
                } catch
                {
                    MessageBox.Show("Problem with parsing actions settings XML file. Action settings are not loaded.");
                    if(reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        public void SaveHighlights(string filename)
        {
            if(actions.Count == 0)
            {
                MessageBox.Show("No highlights to save.");
                return;
            }
            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(filename);
            }
            catch
            {
                MessageBox.Show("Problem with creating highlights XML file. Saving process aborted.");
                return;
            }

            writer.WriteStartDocument();
            writer.WriteStartElement("highlights");

            foreach (Action action in actions)
            {
                writer.WriteStartElement("highlight");

                writer.WriteStartElement("name");
                writer.WriteString(action.name);
                writer.WriteEndElement();

                writer.WriteStartElement("createtime");
                writer.WriteString(action.createTime.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("before");
                writer.WriteString(action.before.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("after");
                writer.WriteString(action.after.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }


            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        public void LoadHighlights(string recordingFile, string highlightsFile)
        {
            if (!File.Exists(recordingFile) || !File.Exists(highlightsFile))
            {
                MessageBox.Show("Problem with loading higlights");
                return;
            }
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(highlightsFile);
            }
            catch
            {
                MessageBox.Show("Problem with opening highlights XML file. Highlights not loaded.");
                return;
            }
            Action highlight = new Action();
            this.actions.Clear();
            try
            {
                while (reader.Read())
                {
                    if (reader.Name == "highlight")
                    {
                        highlight = new Action();
                    }
                    if (reader.Name == "name")
                    {
                        reader.Read();
                        highlight.name = reader.Value;
                        reader.Skip();
                    }
                    if (reader.Name == "createtime")
                    {
                        reader.Read();
                        highlight.createTime = int.Parse(reader.Value);
                        reader.Skip();
                    }
                    if (reader.Name == "before")
                    {
                        reader.Read();
                        highlight.before = int.Parse(reader.Value);
                        reader.Skip();
                    }
                    if (reader.Name == "after")
                    {
                        reader.Read();
                        highlight.after = int.Parse(reader.Value);
                        highlight.filename = recordingFile;
                        highlight.thumbnail = Thumbnailer.Instance.GetThumbnail(recordingFile, highlight.startTime);
                        this.actions.Add(highlight);
                        reader.Skip();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Problem with parsing highlights from XML file. Operation terminated.");
                this.actions.Clear();
            }
            OnPropertyChanged();
        }
    }

}
