using System;
using System.IO;
using System.Xml.Serialization;

namespace SearchSystem
{
    /// <summary>
    /// Класс для работы с txt-файлом
    /// </summary>
    public class FileReader
    {
        /// <summary>
        /// Расположение файла
        /// </summary>
        private string FilePath;
        /// <summary>
        /// Поток для считывания строк
        /// </summary>
        private StreamReader stream;

        public FileReader(string filePath)
        {
            FilePath = filePath;
            StreamReader _stream = OpenFile(filePath);
            if (_stream != null)
                stream = _stream;
        }

        private StreamReader OpenFile(string filePath)
        {
            try
            {
                Console.WriteLine("Open -> " + filePath);
                return new StreamReader(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при открытии файла: " + e.Message);
                return null;
            }
            finally
            {
                Console.WriteLine("Открытие файла " + filePath);
            }
        }

        public string ReadLine()
        {
            string line = "";
            try
            {
                if (stream != null)
                {
                    line = stream.ReadLine();
                    return line;
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);
            }
            finally
            {
                Console.WriteLine($"--------------------------- Считывание строки --------------------------- \n::::->{line}<-:::");
            }
            return null;
        }

        public void FileClose()
        {
            try
            {
                if (stream != null)
                    stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при закрытии файла: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Закрытие файла " + FilePath);
            }
        }
    }
}