using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileInfoFTP
    {
        /// <summary> Массив байт
        public byte[] Data { get; set; }
        /// <summary> Имя файла
        public string Name { get; set; }
        /// <summary> Конструктор для заполнения класса
        public FileInfoFTP(byte[] Data, string Name)
        {
            this.Data = Data;
            this.Name = Name;
        }
    }
}
