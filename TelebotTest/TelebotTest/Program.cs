using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bots.Requests;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using IBWT.Framework.Abstractions;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types.InputFiles;
using System.Diagnostics;

namespace TelegramBotExperiments
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("Token");  /// Написать токент от BotFather
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null)
                {
                    if (message.Text.ToLower() == "/start")
                    {
                        Console.WriteLine($"{message.Chat.FirstName}    |    {message.Photo}" );
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Кинь фото я отредачу!");
                        return;
                    }
                }

                if (message.Photo != null)
                {
                    Console.WriteLine($"{message.Chat.FirstName}    |    {message.Photo}");

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Нормальное фото, но кинь документом!");
                    return;
                }
                if (message.Document != null)
                {
                    Console.WriteLine($"{message.Chat.FirstName}    |    {message.Photo}");
                   await botClient.SendTextMessageAsync(message.Chat.Id, "Погодите идёт обработка...");
                    var filed = update.Message.Document.FileId;
                    var fileinfo = await botClient.GetFileAsync(filed);
                    var filePath = fileinfo.FilePath;

                    string destinationFilePath = $"@{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/{message.Document.FileName}";
                    await using FileStream fileStream = System.IO.File.OpenRead(destinationFilePath);
                    await botClient.DownloadFileAsync(
                        filePath: filePath,
                        destination: fileStream);
                    fileStream.Close();

                    ///Обрабатывает

                    Process.Start(@".../droplet.exe", $@"""{destinationFilePath}"""); /// Путь дроплета фотошопа что бы изменял фото
                  await Task.Delay(1500);
                    /// отправляет обратно пользователю
                   await using Stream stream = System.IO.File.OpenRead(@destinationFilePath);
                    await botClient.SendDocumentAsync(message.Chat.Id, new  InputOnlineFile(stream, message.Document.FileName.Replace(".jpg", "(edited).jpg")));
                     


                    return;
                  }

               if (message.Text != null)
                {
                    Console.WriteLine($"{message.Chat.FirstName}    |    {message.Text}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, message.Text);
                    return;
                }



            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, 
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }

    

    }
}