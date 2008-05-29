/*
 * µLeechFTP
 * 
 * A lite clone of LeechFTP under dot Net technology
 * 
 * CopyRight MARTINEAU Emeric (C) 2008
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.See the GNU GENERAL PUBLIC LICENSE for more
 * details.
 *
 * You should have received a copy of the GNU GENERAL PUBLIC LICENSE along
 * with this program; if not, write to the Free Software Foundation, Inc., 59
 * Temple Place, Suite 330, Boston, MA 02111-1307 USA.
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MicroFTPServeurGUI
{
    class ClassIniReader
    {
        private List<String> Sections = new List<String>();
        private List<List<String>> Keys = new List<List<String>>();
        private List<List<String>> Values = new List<List<String>>();
        private String _FileName;

        private bool _FileExists = false;

        public bool FileExists
        {
            get
            {
                return _FileExists;
            }
        }

        /*
         * Constructor
         */
        public ClassIniReader(String FileName)
        {
            _FileName = FileName;

            if (File.Exists(FileName) == true)
            {
                ReadIniFile();
                _FileExists = true;
            }
        }

        /*
         * Lit un fichier INI
         */
        private void ReadIniFile()
        {
            FileStream fs;
            StreamReader sr;
            String Ligne;
            int index = -1;
            String SectionName;
            int pos;
            String value;
            List<String> CurrentSectionValues = new List<String>();
            List<String> CurrentSectionKeys = new List<String>();

            fs = File.OpenRead(_FileName);
            sr = new StreamReader(fs, Encoding.Default);

            try
            {
                while (sr.EndOfStream == false)
                {
                    Ligne = sr.ReadLine();

                    Ligne.Trim();

                    if (Ligne != "")
                    {
                        if (Ligne.Substring(0, 1) == "[")
                        {
                            index++;

                            SectionName = Ligne.Substring(1, Ligne.Length - 2);

                            Sections.Add(SectionName.ToLower());

                            if (index > 0)
                            {
                                Values.Add(CurrentSectionValues);
                                Keys.Add(CurrentSectionKeys);

                                CurrentSectionValues = new List<String>();
                                CurrentSectionKeys = new List<String>();
                            }
                        }
                        else if (Ligne.Substring(0, 1) == ";")
                        {
                            /* Commentaire, on ne fait rien */
                        }
                        else
                        {
                            pos = Ligne.IndexOf('=');

                            if (pos != -1)
                            {
                                CurrentSectionKeys.Add(Ligne.Substring(0, pos).ToLower().Trim());

                                value = Ligne.Substring(pos + 1, Ligne.Length - (pos + 1)).Trim();

                                if (value != "")
                                {
                                    /* supprimer " et ' */
                                    if ((value.Substring(0, 1) == "\"") || (value.Substring(0, 1) == "'"))
                                    {
                                        value = value.Substring(1, value.Length - 2);
                                    }
                                }

                                CurrentSectionValues.Add(value);
                            }
                        }
                    }
                }

                Values.Add(CurrentSectionValues);
                Keys.Add(CurrentSectionKeys);

                sr.Close();
            }
            finally
            {
            }
        }

        /*
         * Retourne la valeur d'un couple Section/Key
         */
        public String GetValue(String Section, String Key)
        {
            int indexOfSection;
            int indexOfKey;
            List<String> Valeur;
            String Resultat;

            Resultat = "";

            indexOfSection = Sections.IndexOf(Section.ToLower());

            if (indexOfSection != -1)
            {
                Valeur = Keys[indexOfSection];

                indexOfKey = Valeur.IndexOf(Key.ToLower());

                if (indexOfKey != -1)
                {
                    Valeur = Values[indexOfSection];

                    Resultat = Valeur[indexOfKey];
                }
            }

            return Resultat;
        }

    }

}
