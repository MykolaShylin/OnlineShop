using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Storages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot;
using IModel = RabbitMQ.Client.IModel;
using Telegram.Bot.Types.Enums;
using OnlineShop.DB;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApp.Services
{
    public class TelegramService
    {
        private readonly IChatBotAPI chatBotApi;
        private readonly ITelegramBot userDbRepository;
        private readonly IPurchases ordersRepository;

        private IConnection rabbitConnectionPublisher;
        private IModel rabbitChannelPublisher;

        private IConnection rabbitConnectionConsumer;
        private IModel rabbitChannelConsumer;
        public TelegramService(IChatBotAPI chatBotApi, ITelegramBot userDbRepository,IPurchases ordersRepository)
        {
            this.chatBotApi = chatBotApi;

            chatBotApi.Init();            

            this.userDbRepository = userDbRepository;
            this.ordersRepository = ordersRepository;

            ordersRepository.OrderStatusUpdatedEvent += OrdersRepository_OrderStatusUpdatedEvent;

            ordersRepository.NewComfirmedOrderEvent += OrdersRepository_NewComfirmedOrderEvent;

            InitRabbit();
        }

        void InitRabbit()
        {
            ConnectionFactory factory = new ConnectionFactory();

            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.DispatchConsumersAsync = true;

            // this name will be shared by all connections instantiated by
            // this factory
            factory.ClientProvidedName = "online shop"; //"app:audit component:event-consumer";

            rabbitConnectionPublisher = factory.CreateConnection();
            rabbitConnectionConsumer = factory.CreateConnection();

            rabbitChannelPublisher = rabbitConnectionPublisher.CreateModel();
            {
                rabbitChannelPublisher.ExchangeDeclare("dev-ex-to-telegram", ExchangeType.Direct, true);
                rabbitChannelPublisher.QueueDeclare("dev-queue-to-telegram", true, false, false, null);
                rabbitChannelPublisher.QueueBind("dev-queue-to-telegram", "dev-ex-to-telegram", "", null);
            }

            rabbitChannelConsumer = rabbitConnectionConsumer.CreateModel();
            {
                rabbitChannelConsumer.ExchangeDeclare("dev-ex-to-web", ExchangeType.Direct, true);
                rabbitChannelConsumer.QueueDeclare("dev-queue-to-web", true, false, false, null);
                rabbitChannelConsumer.QueueBind("dev-queue-to-web", "dev-ex-to-web", "", null);
            }

            var consumer = new AsyncEventingBasicConsumer(rabbitChannelConsumer);

            consumer.Received += Consumer_Received;

            // this consumer tag identifies the subscription
            // when it has to be cancelled
            string consumerTag = rabbitChannelConsumer.BasicConsume("dev-queue-to-web", false, consumer);
        }

        /// <summary>
        /// Получили сообщение из рэбита с дессириализуем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.Body.ToArray());

            var message = JsonConvert.DeserializeObject<QueueMessageModel>(json);

            await MessageReceivedAsync(message);

            rabbitChannelConsumer.BasicAck(e.DeliveryTag, false);
        }

        /// <summary>
        /// Получаем сообщение, что статус заказа поменялся из проекта onlina shopDB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrdersRepository_OrderStatusUpdatedEvent(object sender, OrderStatusUpdatedEventArgs e)
        {
            var message = new QueueMessageModel()
            {
                ChatId = e.User.TelegramUserId!.Value,
                MessageReceive = BuildOrderUpdateStatusMessage(e)
            };

            var jsonMessage = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(jsonMessage);

            rabbitChannelPublisher.BasicPublish(exchange: "dev-ex-to-telegram", routingKey: "", basicProperties: null, body: body);
        }

        private void OrdersRepository_NewComfirmedOrderEvent(object sender, NewComfirmedOrderEventArgs e)
        {
            var message = new QueueMessageModel()
            {
                ChatId = e.User.TelegramUserId!.Value,
                MessageReceive = BuildNewOrdersMessage(e)
            };

            var jsonMessage = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(jsonMessage);

            rabbitChannelPublisher.BasicPublish(exchange: "dev-ex-to-telegram", routingKey: "", basicProperties: null, body: body);
        }

        /// <summary>
        /// Функционал бота
        /// </summary>
        /// <param name="message"></param>
        private async Task MessageReceivedAsync(QueueMessageModel message)
        {
            if (message.MessageType == MessageType.Text)
            {                
                var existingUser = await userDbRepository.TryGetByTelegramUserIdAsync(message.UserId);

                if (existingUser != null)
                {
                    if (message.MessageReceive == "Список заказов")
                    {
                        var msg = await BuildOrdersMessageAsync(existingUser);
                        await chatBotApi.SendKeyboard(message.ChatId, msg);
                    }
                    else if (message.MessageReceive == "Активные заказы")
                    {
                        var msg = await BuildOrderStatusMessageAsync(existingUser);
                        await chatBotApi.SendKeyboard(message.ChatId, msg);
                    }
                    else if (message.MessageReceive == "Наши контакты")
                    {
                        await chatBotApi.SendKeyboard(message.ChatId,
                            $"Будем рады видеть Вас в нашем главном магазине.\r\n" +
                            $"Адрес: г.Киев, пр. Мира 2/3. Телефон:+38(097) 526-96-88");
                    }
                    else if (message.MessageReceive == "Спецпредложения")
                    {
                        await chatBotApi.SendKeyboard(message.ChatId,
                            $"При заказе от 4000 грн доставка за наш счет!!!\r\n" +
                            $"В течении месяца, при заказе 10 батончиков фирмы Sporter - получите батончик в подарок");
                    }
                    else
                    {
                        await chatBotApi.SendKeyboard(message.ChatId, "Введите команду");
                    }
                }
                else
                {
                    await chatBotApi.SendContactRequest(message.ChatId);
                }
            }
            else if (message.MessageType == MessageType.Contact)
            {
                var result = await userDbRepository.UpdateTelegramUserIdAsync(message.Phone, message.UserId);

                if (result)
                {
                    var existingUser = await userDbRepository.TryGetByTelegramUserIdAsync(message.UserId);

                    await chatBotApi.SendWelcomeMessage(message.ChatId, $"{existingUser.RealName}");
                }
                else
                {
                    await chatBotApi.SendKeyboard(message.ChatId, $"Пожалуйста, перейдите по ссылке ... , чтобы зарегестрироваться");
                }
            }
        }

        /// <summary>
        /// Отправляет пользователю подробный список заказов
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> BuildOrdersMessageAsync(User user)
        {
            var orders = await ordersRepository.TryGetByUserIdAsync(user.Id);
            if (orders.Count() != 0)
            {
                var sb = new StringBuilder();
                foreach (var order in orders)
                {
                    sb.AppendLine($"Заказ № {order.Id.ToString().Substring(0, 10)}");
                    foreach (var item in order.Items)
                    {
                        var productCost = decimal.Ceiling(((item.ProductInfo.Cost * 100) * (100 - item.ProductInfo.DiscountPercent) / 100) / 100);
                        sb.AppendLine($"{item.Product.Name} x {item.Amount}шт. - {productCost}грн");
                    }

                    sb.AppendLine($"Статус: {EnumHelper.GetDisplayName(order.orderStatus)}");
                    sb.AppendLine("---------------------------");
                }

                return sb.ToString();
            }
            else
            {
                return "У Вас пока нет заказов";
            }
        }

        /// <summary>
        /// Отправляет пользователю обновленный статус заказа
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> BuildOrderStatusMessageAsync(User user)
        {
            var orders = await ordersRepository.GetAllActiveByUserIdAsync(user.Id);
            if (orders.Count() != 0)
            {
                var sb = new StringBuilder();
                foreach (var order in orders)
                {
                    sb.AppendLine($"Заказ № {order.Id.ToString().Substring(0, 10)}");
                    sb.AppendLine($"Статус: {EnumHelper.GetDisplayName(order.orderStatus)}");
                    sb.AppendLine("---------------------------");
                }

                return sb.ToString();
            }
            else
            {
                return "У Вас пока нет заказов";
            }
        }

        public string BuildOrderUpdateStatusMessage(OrderStatusUpdatedEventArgs e)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Заказа № {e.Order.Id.ToString().Substring(0, 10)}");
            foreach (var item in e.Order.Items)
            {
                sb.AppendLine($"{item.Product.Brand} - {item.Product.Name}");
            }
            sb.AppendLine($"Статус изменен с: {EnumHelper.GetDisplayName(e.OldStatus).ToUpper()}");
            sb.AppendLine($"На: {EnumHelper.GetDisplayName(e.NewStatus).ToUpper()}");

            return sb.ToString();

        }

        public string BuildNewOrdersMessage(NewComfirmedOrderEventArgs e)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Благодарим за заказ на нашем сайте.");
            sb.AppendLine("Наш менеджер вскоре свяжится с вами");
            sb.AppendLine("Ваш заказ: ");

            decimal totalOrderCost = 0;

            foreach(var item in e.Order.Items)
            {
                sb.AppendLine($"{@EnumHelper.GetDisplayName(item.Product.Brand)} - {item.Product.Name}");
                sb.AppendLine($"Количеств, шт: {item.Amount}");
                if(item.ProductInfo.DiscountPercent == 0)
                {
                    var productCost = item.ProductInfo.Cost * item.Amount;
                    totalOrderCost += productCost;
                    sb.AppendLine($"Цена, грн: {productCost}");
                }
                else
                {
                    var productCost = decimal.Ceiling(((item.ProductInfo.Cost * 100) * (100 - item.ProductInfo.DiscountPercent) / 100) / 100);
                    totalOrderCost += productCost;
                    sb.AppendLine($"Цена (с учетом скидки {item.ProductInfo.DiscountPercent}), грн: {productCost}");
                }
                sb.AppendLine("--------------------");
            }
            sb.AppendLine($"Общая сумма заказа, грн: {totalOrderCost}");
            sb.AppendLine("Если возникнут вопросы звоните нам:");
            sb.AppendLine("Пн-Пт 10:00−20:00, Сб-Вс 10:00−19:00");
            sb.AppendLine("Телефон: 0 800 33 97 12");

            return sb.ToString();

        }

    }
}
