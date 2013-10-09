﻿using System;
using System.Text;
using System.IO;
using System.Drawing;

namespace Utility
{
    //=========================================================================
    /// <summary>
    /// This is the configuration settings object.
    /// Add fields here to store extra settings for the application.
    /// </summary>
    [Serializable()]
    public class Settings
    {
        public Point MainFormLocation;
        public Size MainFormSize;
        public System.Windows.Forms.FormWindowState MainFormWindowState;
    }

    //=========================================================================
    /// <summary>
    /// Handle the reading and writing of the configuration settings file
    /// </summary>
    public class Configuration
    {
        public string Version { get { return "1.0"; } }  //this could be obtained from elsewhere in the application
        public Settings Settings { get; set; }
        private string ConfigurationFile;

        /// <summary>
        /// Constructor
        /// </summary>
        public Configuration()
        {
            //On Linux and Mac the path will be .config/
            ConfigurationFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                                             "ApsimInitiative",
                                             "ApsimX",
                                             "ApsimX.xml");
            //deserialise the file
            if (File.Exists(ConfigurationFile))
            {
                System.Xml.Serialization.XmlSerializer xmlreader = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                StreamReader filereader = new StreamReader(ConfigurationFile);
                Settings = new Settings();
                Settings = (Settings)xmlreader.Deserialize(filereader);
                filereader.Close();
            }
            else
            {
                Settings = new Settings() { MainFormSize = new Size(640, 480), 
                                            MainFormWindowState = System.Windows.Forms.FormWindowState.Maximized };
            }
        }

        /// <summary>
        /// Store the configuration settings to file
        /// </summary>
        public void Save()
        {
            string ConfigPath = Path.GetDirectoryName(ConfigurationFile);
            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            StreamWriter filewriter = new StreamWriter(ConfigurationFile);
            System.Xml.Serialization.XmlSerializer xmlwriter = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            xmlwriter.Serialize(filewriter, Settings);
            filewriter.Close();
        }
    }
}