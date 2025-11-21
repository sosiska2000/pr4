using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ViewModelMessage
    {
        /// <summary> Команда
        public string Command { get; set; }
        /// <summary> Текст ответа
        public string Data{ get; set; }
        /// <summary> Конструктор для заполнения данных
        public ViewModelMessage(string Command, string Data)
        {
            this.Command = Command;
            this.Data = Data;
        }
    }
}
