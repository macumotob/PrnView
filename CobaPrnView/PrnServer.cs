using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrnView
{
    class PrnServer
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public PrnServer(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов
                //Listener.AcceptTcpClient();
                var client =Listener.AcceptTcpClient();
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(client);
            }
        }

        // Остановка сервера
        ~PrnServer()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }
        static void ClientThread(Object StateInfo)
        {
            PrnClient client = new PrnClient((TcpClient)StateInfo);
        }
    }
}
