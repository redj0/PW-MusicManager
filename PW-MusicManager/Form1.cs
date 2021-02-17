using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;



namespace PW_MusicManager
{

    public partial class MainWindow : Form
    {
        public struct InGameDetails
        {
            public string Name;
            public string ID;
            public string GlobalName;
        }

        // ID, global.txt Name, Name, 
        List<InGameDetails> IngameSongsList = new List<InGameDetails>();
        string InstallFolder;
        string GlobalTxtFile = @"\Pistol Whip_Data\StreamingAssets\Audio\GeneratedSoundBanks\Windows\Global.txt";
        readonly string Base_Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PW-MusicManager\";
        List<string> ComboBoxItems = new List<string> { };


        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }
        private void Setup()
        {

            List<string> Paths = new List<string> { "Imported", "Backup" };

            // Incase we decide on more paths
            foreach (string path in Paths)
            {
                string p = Base_Path + path;
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }

            // Add all songs to DataGridView
            SongDataGrid.Columns.Add("Song", "Song");

            // Add current list of Customizable songs to ComboBox
            DataGridViewComboBoxColumn ReplacementSong = new DataGridViewComboBoxColumn();
            ComboBoxItems.Add("Original");
            ComboBoxItems.Add("Replaced With Unknown");
            foreach (string file in Directory.GetFiles(Base_Path + "Imported\\"))
            {
                ComboBoxItems.Add(Path.GetFileName(file));
            }
            ReplacementSong.Name = "Replacement Song";
            ReplacementSong.DataSource = ComboBoxItems;
            ReplacementSong.MinimumWidth = 400;
            ReplacementSong.DefaultCellStyle.NullValue = ComboBoxItems[0];


            SongDataGrid.Columns.Add(ReplacementSong);

            

            InstallFolder = GetInstallFolder();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            PopulateInGameSongs();
            IngameSongsList.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));
            foreach (InGameDetails InGameSong in IngameSongsList)
            {
                //System.Diagnostics.Debug.WriteLine(InGameSong.GlobalName + "\t" + InGameSong.Name + "\t" + InGameSong.ID );
                this.SongDataGrid.Rows.Add(InGameSong.Name);
            }
            
            // Make sure we're aware of backup state
            foreach(DataGridViewRow row in SongDataGrid.Rows)
            {
                string RefName = row.Cells[0].Value.ToString();
                var InGameSong = IngameSongsList.Find(x => x.Name == RefName);
                // If the file is in backup, assume its currently replaced with something
                if (File.Exists(Base_Path + @"Backup\" + InGameSong.ID + ".wem"))
                {
                    row.Cells[1].Value = ComboBoxItems[1];
                }
            }
        }

        private string GetInstallFolder()
        {
            string NormalInstallFolder = @"C:\Program Files (x86)\Steam\steamapps\common\Pistol Whip";
            
            if (Directory.Exists(NormalInstallFolder))
            {
                if (File.Exists(NormalInstallFolder + GlobalTxtFile))
                {
                    return NormalInstallFolder;
                }
            } else {
                while (true)
                {
                    // Set up message box
                    string message = @"Cannot find Pistol Whip installation. Press OK to browse and select installation folder.";
                    string caption = "Pistol Whip Installation not found";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);

                    FolderBrowserDialog OpenFolderDialogInstallFolder = new FolderBrowserDialog
                    {
                        Description = "Pistol Whip Install Folder"
                    };
                    // return selected folder if we can find \Pistol Whip_Data\StreamingAssets\Audio\GeneratedSoundBanks\Windows\Global.txt
                    if (OpenFolderDialogInstallFolder.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(OpenFolderDialogInstallFolder.SelectedPath + GlobalTxtFile))
                        {
                            return OpenFolderDialogInstallFolder.SelectedPath;
                        }
                    }
                }
            }
            System.Environment.Exit(0);
            return null;
        }

        private void PopulateInGameSongs()
        {
            //Populate the List<string> IngameSongs = new List<string> { };
            // We're going to parse the global.txt file here

            var LineList = new List<string>();
            int StreamedAudioSection = 0;

            // Read the file and display it line by line.
            using (var file = new StreamReader(InstallFolder + GlobalTxtFile))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    // Read file into List
                    LineList.Add(line);
                }
                file.Close();
            }
            foreach (string line in LineList)
            {
                // Parse
                var delimiters = new char[] { '\t' };
                var segments = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    if (StreamedAudioSection == 1)
                    {
                        // We're in the streamed audio section lets populate

                        // segments[0] should be id
                        // segments[1] should be name
                        string name = ConvertSong(segments[1]);
                        if (name != null)
                        {
                            // System.Diagnostics.Debug.WriteLine(name + " " + segments[1]);
                            // "name" : { "ID": 123, "GlobalName": "Whatever" }
                            InGameDetails tmp_igd = new InGameDetails { Name = name, ID = segments[0], GlobalName = segments[1] };
                            IngameSongsList.Add(tmp_igd);
                        }

                    }
                    if (segments[0] == "Streamed Audio")
                    {
                        //We've reached the Streamed audio section
                        StreamedAudioSection = 1;
                    }
                }
            }
        }



        private void ImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Browse Audio Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = ".wem",
                Filter = "Audio files (*.wem)|*.wem",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<string> SongsPaths = new List<string> { };
                foreach (String file in OpenFileDialog1.FileNames)
                {
                    string ext = Path.GetExtension(file);
                    if (ext == ".wem")
                    {
                        // Add to New List
                        SongsPaths.Add(file);
                        // Copy to Foler
                        string fName = Path.GetFileName(file);
                        System.Diagnostics.Debug.WriteLine(Base_Path + @"Imported\" + fName);
                        File.Copy(file, Base_Path + fName, true);
                        // Add new ones to ComboBoxItems + Combobox
                        ComboBoxItems.Add(fName);
                    }
                }
            }

        }
        
        private void ReplaceSong(string FileToBeMoved, string IDtobereplaced, int Isbackup)
        {
            string GameAudioToReplace = InstallFolder + Path.GetDirectoryName(GlobalTxtFile) + @"\" + IDtobereplaced + ".wem";

            string BackupPath = Base_Path + @"Backup\";
            // Move old song to Base_Path/Backups/ if a backup doesnt already exist
            if (!File.Exists(BackupPath + IDtobereplaced + ".wem")) 
            {
                File.Copy(GameAudioToReplace, BackupPath + IDtobereplaced + ".wem");
            }
            // Get rid of current song
            if(File.Exists(GameAudioToReplace))
            {
                File.Delete(GameAudioToReplace);
            }
            // Move New song to Game Audio Folder/{ID}
            if(File.Exists(FileToBeMoved))
            {
                File.Copy(FileToBeMoved, GameAudioToReplace);
                if(Isbackup == 1)
                {
                    // Ditch the backup if we're going back
                    File.Delete(FileToBeMoved);
                }
            }
        }


        private void SongDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void SongDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // combobox column is the second one so 1
            DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)SongDataGrid.Rows[e.RowIndex].Cells[1];
            if (cb.Value == null)
            {
                // Return Song to original
                string RefName = SongDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                var InGameSong = IngameSongsList.Find(x => x.Name == RefName);
                string FileToMove = Base_Path + @"Backup\" + InGameSong.ID + ".wem";
                System.Diagnostics.Debug.WriteLine(FileToMove + "->" + InGameSong.ID);
                ReplaceSong(FileToMove, InGameSong.ID, 1);
                SongDataGrid.Invalidate();
                return;
            }
            else if (cb.Value.ToString() == "Replaced With Unknown")
            {
                // Do nothing 
                return;
            } else
            {
                // Do stuff with cb.Value
                string RefName = SongDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                var InGameSong = IngameSongsList.Find(x => x.Name == RefName);
                string FileToMove = Base_Path + @"Imported\" + cb.Value;
                ReplaceSong(FileToMove, InGameSong.ID, 0);
                SongDataGrid.Invalidate();
            }
        }

        private void SongDataGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (SongDataGrid.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                SongDataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // Big horrible switch statement to determine user friendly names
        private string ConvertSong(string inputstring)
        {
            return inputstring switch
            {
                "mus_core_thehighpriestess" => "The High Priestess",
                "mus_cnk_anotherday" => "Another Day",
                "mus_tutorial" => "Tutorial",
                "TheGrave_Edit_01" => "The Grave",
                "mus_core_darkskies" => "Dark Skies",
                "BlackMagic_Edit_01" => "Black Magic",
                "mus_cnk_embers" => "Embers",
                "mus_core_fullthrottle" => "Full Throttle",
                "mus_cnk_lettinggo" => "Letting Go",
                "mus_core_religion" => "Religion",
                "Replicants_Edit_01" => "Replicants",
                "mus_core_death" => "Death",
                "mus_core_downloadthefuture" => "Download The Future",
                "mus_core_lilith" => "Lillith",
                "mus_core_requiem" => "Requiem",
                "mus_core_thefall" => "The Fall",
                "mus_core_ruafraid" => "R U Afraid",
                "mus_core_akuma" => "Akuma",
                "Revelations_Edit_01" => "Revelations",
                _ => null,
            };
        }
    }

}
