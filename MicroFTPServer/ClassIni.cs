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
 * ****************************************************************************
 * History :
 *  - 08/07/2008 : rename all variable,
 *                 use String.Equals(),
 * ****************************************************************************
 * Variables names :
 *  xyZZZZZ :
 *            x : l : local variable
 *                g : global variable/public variable
 *                p : private/protected variable
 *                a : argument variable
 *                
 *            y : s : string
 *                i : integer
 *                f : fload
 *                d : double
 *                a : array
 *                l : list<>
 *                o : object
 *                b : bool
 *                c : char
 *                l : long
 *                
 *           ZZZZ : name of variable
 * ****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace ConsoleApplication1
{
    class ClassIniReader
    {
        private List<String> plSections = new List<String>();
        private List<List<String>> plKeys = new List<List<String>>();
        private List<List<String>> plValues = new List<List<String>>();
        private String psFileName;

        private bool pbFileExists = false;

        public bool FileExists
        {
            get
            {
                return pbFileExists;
            }
        }

        /*
         * Constructor
         */
        public ClassIniReader(String asFileName)
        {
            psFileName = asFileName;

            if (File.Exists(asFileName) == true)
            {
                ReadIniFile();
                pbFileExists = true;
            }
        }

        /*
         * Lit un fichier INI
         */
        private void ReadIniFile()
        {
            FileStream loFs;
            StreamReader loSr;
            String lsLigne;
            int liIndex = -1;
            String lsSectionName;
            int liPos;
            String lsValue;
            List<String> llCurrentSectionValues = new List<String>();
            List<String> llCurrentSectionKeys = new List<String>();

            loFs = File.OpenRead(psFileName);
            loSr = new StreamReader(loFs, Encoding.Default);

            try
            {
                while (loSr.EndOfStream == false)
                {
                    lsLigne = loSr.ReadLine();

                    lsLigne.Trim();

                    if (String.IsNullOrEmpty(lsLigne) == false)
                    {
                        if (lsLigne.Substring(0, 1).Equals("[") == true)
                        {
                            liIndex++;

                            lsSectionName = lsLigne.Substring(1, lsLigne.Length - 2);

                            plSections.Add(lsSectionName.ToLower());

                            if (liIndex > 0)
                            {
                                plValues.Add(llCurrentSectionValues);
                                plKeys.Add(llCurrentSectionKeys);

                                llCurrentSectionValues = new List<String>();
                                llCurrentSectionKeys = new List<String>();
                            }
                        }
                        else if (lsLigne.Substring(0, 1).Equals(";") == true)
                        {
                            /* Commentaire, on ne fait rien */
                        }
                        else
                        {
                            liPos = lsLigne.IndexOf('=');

                            if (liPos != -1)
                            {
                                llCurrentSectionKeys.Add(lsLigne.Substring(0, liPos).ToLower().Trim());

                                lsValue = lsLigne.Substring(liPos + 1, lsLigne.Length - (liPos + 1)).Trim();

                                if (String.IsNullOrEmpty(lsValue) == false)
                                {
                                    /* supprimer " et ' */
                                    if ((lsValue.Substring(0, 1).Equals("\"") == true) || (lsValue.Substring(0, 1).Equals("'") == true))
                                    {
                                        lsValue = lsValue.Substring(1, lsValue.Length - 2);
                                    }
                                }

                                llCurrentSectionValues.Add(lsValue);
                            }
                        }
                    }
                }

                plValues.Add(llCurrentSectionValues);
                plKeys.Add(llCurrentSectionKeys);

                loSr.Close();
            }
            finally
            {
            }
        }

        /*
         * Retourne la valeur d'un couple Section/Key
         */
        public String GetValue(String asSection, String asKey)
        {
            int liIndexOfSection;
            int liIndexOfKey;
            List<String> llValeur;
            String lsResultat;

            lsResultat = "";

            liIndexOfSection = plSections.IndexOf(asSection.ToLower());

            if (liIndexOfSection != -1)
            {
                llValeur = plKeys[liIndexOfSection];

                liIndexOfKey = llValeur.IndexOf(asKey.ToLower());

                if (liIndexOfKey != -1)
                {
                    llValeur = plValues[liIndexOfSection];

                    lsResultat = llValeur[liIndexOfKey];
                }
            }

            return lsResultat;
        }

    }
}
