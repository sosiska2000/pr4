using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

public static class Program
{
    /// <summary> IP-адрес сервера </summary>
    public static IPAddress IpAdress;
    /// <summary> Порт сервера </summary>
    public static int Port;
    /// <summary> Код пользователя </summary>
    public static int Id = -1;

    /// <summary> Функция проверки команд </summary>
    public static bool CheckCommand(string message)
    {
        // Создаём переменную говорящую о том, что команда неверная
        bool BCommand = false;
        // Разбиваем сообщение пользователя на массив
        string[] DataMessage = message.Split(new string[] { " " }, StringSplitOptions.None);

        // Если длина данных больше 0
        if (DataMessage.Length > 0)
        {
            // проверяем первую строку на наличие команды
            // существующие команды : connect cd get set
            string Command = DataMessage[0];
            // Если команда подключение
            if (Command == "connect")
            {
                // Проверяем что у нас отправляется три данных, команда, логин, пароль
                if (DataMessage.Length != 3)
                {
                    // Меняем цвет текста в командной строке
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Выводим текст
                    Console.WriteLine("Использование: connect [login] [password]\nПример: connect User1 P@sswOrd");
                    // Говорим что команда не верная
                    BCommand = false;
                }
                else
                    // Если всё правильно, команда верная
                    BCommand = true;
            }
            // Если команда на переход по директориям
            else if (Command == "cd")
            {
                // Команда верная
                BCommand = true;
            }
            // Если команда получения файла
            else if (Command == "get")
            {
                // Если длина сообщения более одного
                if (DataMessage.Length == 1)
                {
                    // Меняем цвет текста в командной строке
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Выводим текст
                    Console.WriteLine("Использование: get [NameFile]\nПример: get Test.txt");
                    // Говорим что команда не верная
                    BCommand = false;
                }
                else
                    // Команда верная
                    BCommand = true;
            }
            // Если команда отправки файла
            else if (Command == "set")
            {
                // Если длина сообщения более одного
                if (DataMessage.Length == 1)
                {
                    // Меняем цвет текста в командной строке
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Выводим текст
                    Console.WriteLine("Использование: set [NameFile]\nПример: set Test.txt");
                    // Говорим что команда не верная
                    BCommand = false;
                }
                else
                    // Команда верная
                    BCommand = true;
            }
        }
        return BCommand;
    }

    public static void ConnectServer()
    {
        try
        {
            // Создание конечной точки
            IPEndPoint endPoint = new IPEndPoint(IpAdress, Port);
            // Создание сокета
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Подключение к серверу
            socket.Connect(endPoint);

            // Если подключение успешно
            if (socket.Connected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string message = Console.ReadLine();

                if (CheckCommand(message))
                {
                    // Создание объекта для отправки
                    object viewBackSend = new { Message = message, Id = Id };

                    // Если команда "set"
                    if (message.Split(new string[] { " " }, StringSplitOptions.None)[0] == "set")
                    {
                        string[] DataMessage = message.Split(new string[] { " " }, StringSplitOptions.None);

                        // Получение имени файла
                        string Namefile = "";
                        for (int i = 1; i < DataMessage.Length; i++)
                        {
                            if (Namefile == "")
                                Namefile = DataMessage[i];
                            else
                                Namefile += " " + DataMessage[i];
                        }

                        // Проверка существования файла
                        if (File.Exists(Namefile))
                        {
                            FileInfo fileInfo = new FileInfo(Namefile);
                            // Создание объекта с данными файла
                            var fileData = new { Data = File.ReadAllBytes(Namefile), Name = fileInfo.Name };
                            viewBackSend = new { Message = JsonConvert.SerializeObject(fileData), Id = Id };
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Файл не существует");
                        }
                    }

                    // Отправка сообщения
                    byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewBackSend));
                    socket.Send(messageByte);

                    // Получение ответа
                    byte[] bytes = new byte[5760];
                    int bytesRec = socket.Receive(bytes);
                    string messageServer = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Десериализация ответа
                    var viewBackMessage = JsonConvert.DeserializeObject<dynamic>(messageServer);

                    // Обработка команд от сервера
                    if (viewBackMessage.Command == "autorization")
                    {
                        // Обработка авторизации
                        if (int.TryParse(viewBackMessage.Data.ToString(), out int result))
                        {
                            Id = result;
                        }
                    }
                    else if (viewBackMessage.Command == "message")
                    {
                        Console.WriteLine(viewBackMessage.Data);
                    }
                    else if (viewBackMessage.Command == "cd")
                    {
                        List<string> FolderFiles = JsonConvert.DeserializeObject<List<string>>(viewBackMessage.Data.ToString());

                        foreach (string name in FolderFiles)
                        {
                            Console.WriteLine(name);
                        }
                    }
                    else if (viewBackMessage.Command == "file")
                    {
                        string[] DataMessage = ((string)viewBackSend.GetType().GetProperty("Message").GetValue(viewBackSend)).Split(new string[] { " " }, StringSplitOptions.None);

                        // Получение имени файла
                        string getFile = "";
                        for (int i = 1; i < DataMessage.Length; i++)
                        {
                            if (getFile == "")
                                getFile = DataMessage[i];
                            else
                                getFile += " " + DataMessage[i];
                        }

                        byte[] byteFile = JsonConvert.DeserializeObject<byte[]>(viewBackMessage.Data.ToString());
                        File.WriteAllBytes(getFile, byteFile);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неизвестная команда.");
                    }
                }

                socket.Close();
            }
        }
        catch (Exception exp)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка подключения: " + exp.Message);
        }
    }

    static void Main(string[] args)
    {
        // Просим ввести пользователя IP-адрес
        Console.Write("Введите IP адрес сервера: ");
        string sIpAdress = Console.ReadLine();
        // Просим пользователя ввести порт
        Console.Write("Введите порт: ");
        string sPort = Console.ReadLine();
        // Проверяем что пользователь ввёл адрес и порт корректно
        if (int.TryParse(sPort, out Port) && IPAddress.TryParse(sIpAdress, out IpAdress))
        {
            // Меням цвет текста в консоле
            Console.ForegroundColor = ConsoleColor.Green;
            // Выводим надпись о том что подключены к серверу
            Console.WriteLine("Данные успешно введены. Подключаюсь к сервер.");
            while (true)
            {
                // Запускаем подключение
                ConnectServer();
            }
        }
    }
}

// Вспомогательный класс для передачи файлов
public class FileInfoFTP
{
    public byte[] Data { get; set; }
    public string Name { get; set; }

    public FileInfoFTP(byte[] data, string name)
    {
        Data = data;
        Name = name;
    }
}