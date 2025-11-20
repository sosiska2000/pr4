using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ViewModelSend
    {
        /// <summary> Сообщение отправляемое сервером
        public string Message { get; set; }
        /// <summary> Код пользователя
        public int Id { get; set; }
        /// <summary> Конструктор для заполнения класса
        public ViewModelSend(string message, int id) 
        {
            this.Message = message;
            this.Id = id;
        }
    }
}
