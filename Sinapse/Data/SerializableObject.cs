/***************************************************************************
 *   Sinapse Neural Networking Tool         http://sinapse.googlecode.com  *
 *  ---------------------------------------------------------------------- *
 *   Copyright (C) 2006-2008 Cesar Roberto de Souza <cesarsouza@gmail.com> *
 *                                                                         *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 3 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.IO;


namespace Sinapse.Data
{

    [Serializable]
    internal class SerializableObject<T> where T : SerializableObject<T>
    {

        private string m_lastSavePath;

        internal event FileSystemEventHandler ObjectSaved;


        //---------------------------------------------


        #region Constructor
        protected SerializableObject()
        {
            this.m_lastSavePath = String.Empty;
        }
        #endregion

        //---------------------------------------------


        #region Properties
        public string LastSavePath
        {
            get { return this.m_lastSavePath; }
        }

        public bool IsSaved
        {
            get { return (this.m_lastSavePath != null && this.m_lastSavePath.Length > 0); }
        }
        #endregion


        //---------------------------------------------


        #region Events
        protected void OnObjectSaved(FileSystemEventArgs e)
        {
            if (this.ObjectSaved != null)
                this.ObjectSaved.Invoke(this, e);
        }
        #endregion


        //---------------------------------------------


        #region Static Methods
        public static void Serialize(SerializableObject<T> serializableObject, string path)
        {
            FileStream fileStream = null;
            bool success = true;

            try
            {
                fileStream = new FileStream(path, FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
          //      bf.Binder = new AnyVersionObjectBinder();
                bf.Serialize(fileStream, serializableObject);
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.WriteLine("Directory not found during serialization " + e.Message);
                success = false;
                throw e;
            }
            catch (SerializationException e)
            {
                Debug.WriteLine("Error occured during serialization: " + e.Message);
                success = false;
                throw e;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error saving object: " + e.Message);
                success = false;
                throw e;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();

                if (success)
                {
                    serializableObject.m_lastSavePath = path;
                    serializableObject.OnObjectSaved(new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(path), Path.GetFileName(path)));
                }
            }
        }

        public static T Deserialize(string path)
        {

            T serializableObject = null;
            FileStream fileStream = null;
            bool success = true;

            try
            {
                fileStream = new FileStream(path, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
         //       bf.Binder = new AnyVersionObjectBinder();
                serializableObject = (T)bf.Deserialize(fileStream);
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine("File not found during deserialization");
                success = false;
                throw e;
            }
            catch (SerializationException e)
            {
                Debug.WriteLine("Error occured during deserialization");
                success = false;
                throw e;
            }
            catch (Exception e)
            {
                success = false;
                throw e;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();

                if (success)
                    serializableObject.m_lastSavePath = path;
            }

            return serializableObject;
        }
        #endregion

    }


  /*  internal sealed class AnyVersionObjectBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Debug.WriteLine("Typename: " + typeName);
            Debug.WriteLine("Assembly: " + assemblyName);

            return System.Type.GetType(typeName + ", " + Assembly.GetExecutingAssembly().FullName);
        }
    }
   */
}