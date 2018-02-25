using System;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using System.Threading;
using InstaBotLibrary.User;
using InstaBotLibrary.Bound;
using InstaBotLibrary.Filter;
using InstaBotLibrary.Instagram;
using Microsoft.Extensions.Options;
using InstaBotLibrary.Tokens;

namespace InstaBotLibrary.TelegramBot
{
    public class TelegramBot : ITelegramService
    {
        private TelegramBotClient bot;
        //https://web. telegram.org/#/im?p=@DaWay_bot
        private IBoundRepository boundRepository;
        private IFilterRepository filterRepository;
        private ITokenGenerator tokenGenerator;

        public TelegramBot(IOptions<TelegramBotOptions> options, IUserRepository userRepository, IBoundRepository boundRepository, IFilterRepository filterRepository, ITokenGenerator generator)
        {
            bot = new TelegramBotClient(options.Value.Token);
            this.boundRepository = boundRepository;
            this.filterRepository = filterRepository;
            tokenGenerator = generator;
            bot.OnMessage += Bot_OnMessage;
        }

        public void Start()
        {
            bot.StartReceiving();
        }

        public void Stop()
        {
            bot.StopReceiving();
        }

        public void SendMessage(int boundId, string message)
        {
            long chatId = boundRepository.GetBoundInfo(boundId).TelegramChatId.Value;
            bot.SendTextMessageAsync(chatId, message);
        }

        public void SendPost(int boundId, Post post)
        {
            long chatId = boundRepository.GetBoundInfo(boundId).TelegramChatId.Value;


            bot.SendPhotoAsync(chatId, post.imageUrl, post.text ?? "");
        }


        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text == "/start")
            {
                long chatId = e.Message.Chat.Id;
                BoundModel bound = boundRepository.GetBoundByTelegramChatId(chatId);
                if (bound == null)
                {
                    bound = new BoundModel();
                    bound.TelegramAccount = e.Message.From.Username;
                    bound.TelegramChatId = e.Message.Chat.Id;
                    bound.TelegramToken = tokenGenerator.GenerateToken(40);
                    boundRepository.AddBound(bound);
                }
                (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "http://localhost:58687/Instagram/Login?token=" + bound.TelegramToken);

            }
            else if (e.Message.Text.Split(' ')[0] == "add")
            {
                string filterToAdd = e.Message.Text.Split(' ')[1];
                int boundId = boundRepository.GetBoundByTelegramChatId(e.Message.Chat.Id).Id;
                FilterModel filter = new FilterModel();
                filter.BoundId = boundId;
                filter.Filter = filterToAdd;
                if (filterRepository.CheckFilter(filter))
                {
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "Такой фильтр уже есть!");
                }
                else
                {
                    filterRepository.AddFilter(filter);
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "Фильтр добавлен!");
                }
            }
            else if (e.Message.Text.Split(' ')[0] == "delete")
            {
                string filterToDelete = e.Message.Text.Split(' ')[1];
                int boundId = boundRepository.GetBoundByTelegramChatId(e.Message.Chat.Id).Id;
                FilterModel filter = new FilterModel();
                filter.BoundId = boundId;
                filter.Filter = filterToDelete;
                if (!filterRepository.CheckFilter(filter))
                {
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "Такого фильтра нету!");
                }
                else
                {
                    filterRepository.DeleteFilter(filter);
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "Фильтр удален!");
                }
            }
            else if (e.Message.Text == "all")
            {
                int boundId = boundRepository.GetBoundByTelegramChatId(e.Message.Chat.Id).Id;
                var filters = filterRepository.getBoundFilters(boundId);
                foreach (var f in filters)
                {
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, f.Filter);
                }
            }
            else
            {
                (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "add {filter} - добавить фильтр");
                (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "delete {filter} - удалить фильтр");
                (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, "all - просмотреть фильтры");
            }

        }
    }
}
